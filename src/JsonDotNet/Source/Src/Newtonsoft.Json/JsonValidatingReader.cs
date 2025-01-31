﻿#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Utilities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Newtonsoft.Json
{
  /// <summary>
  /// Represents a reader that provides <see cref="JsonSchema"/> validation.
  /// </summary>
  public class JsonValidatingReader : JsonReader, IJsonLineInfo
  {
    private class SchemaScope
    {
      private readonly JTokenType _tokenType;
      private readonly IList<JsonSchemaModel> _schemas;
      private readonly Dictionary<string, bool> _requiredProperties;

      public string CurrentPropertyName { get; set; }
      public int ArrayItemCount { get; set; }

      public IList<JsonSchemaModel> Schemas
      {
        get { return _schemas; }
      }

      public Dictionary<string, bool> RequiredProperties
      {
        get { return _requiredProperties; }
      }

      public JTokenType TokenType
      {
        get { return _tokenType; }
      }

      public SchemaScope(JTokenType tokenType, IList<JsonSchemaModel> schemas)
      {
        _tokenType = tokenType;
        _schemas = schemas;

        _requiredProperties = schemas.SelectMany(GetRequiredProperties).Distinct().ToDictionary(p => p, p => false);
      }

      private IEnumerable<string> GetRequiredProperties(JsonSchemaModel schema)
      {
        if (schema == null || schema.Properties == null)
          return Enumerable.Empty<string>();

        return schema.Properties.Where(p => p.Value.Required).Select(p => p.Key);
      }
    }

    private readonly JsonReader _reader;
    private readonly Stack<SchemaScope> _stack;
    private JsonSchema _schema;
    private JsonSchemaModel _model;
    private SchemaScope _currentScope;

    /// <summary>
    /// Sets an event handler for receiving schema validation errors.
    /// </summary>
    public event ValidationEventHandler ValidationEventHandler;

    /// <summary>
    /// Gets the text value of the current Json token.
    /// </summary>
    /// <value></value>
    public override object Value
    {
      get { return _reader.Value; }
    }

    /// <summary>
    /// Gets the depth of the current token in the JSON document.
    /// </summary>
    /// <value>The depth of the current token in the JSON document.</value>
    public override int Depth
    {
      get { return _reader.Depth; }
    }

    /// <summary>
    /// Gets the quotation mark character used to enclose the value of a string.
    /// </summary>
    /// <value></value>
    public override char QuoteChar
    {
      get { return _reader.QuoteChar; }
      protected internal set { }
    }

    /// <summary>
    /// Gets the type of the current Json token.
    /// </summary>
    /// <value></value>
    public override JsonToken TokenType
    {
      get { return _reader.TokenType; }
    }

    /// <summary>
    /// Gets The Common Language Runtime (CLR) type for the current Json token.
    /// </summary>
    /// <value></value>
    public override Type ValueType
    {
      get { return _reader.ValueType; }
    }

    private void Push(SchemaScope scope)
    {
      _stack.Push(scope);
      _currentScope = scope;
    }

    private SchemaScope Pop()
    {
      SchemaScope poppedScope = _stack.Pop();
      _currentScope = (_stack.Count != 0)
        ? _stack.Peek()
        : null;

      return poppedScope;
    }

    private IEnumerable<JsonSchemaModel> CurrentSchemas
    {
      get { return _currentScope.Schemas; }
    }

    private IEnumerable<JsonSchemaModel> CurrentMemberSchemas
    {
      get
      {
        if (_currentScope == null)
          return new List<JsonSchemaModel>(new [] { _model });

        if (_currentScope.Schemas == null || _currentScope.Schemas.Count == 0)
          return Enumerable.Empty<JsonSchemaModel>();

        switch (_currentScope.TokenType)
        {
          case JTokenType.None:
            return _currentScope.Schemas;
          case JTokenType.Object:
            {
              if (_currentScope.CurrentPropertyName == null)
                throw new Exception("CurrentPropertyName has not been set on scope.");

              IList<JsonSchemaModel> schemas = new List<JsonSchemaModel>();

              foreach (JsonSchemaModel schema in CurrentSchemas)
              {
                JsonSchemaModel propertySchema;
                if (schema.Properties != null && schema.Properties.TryGetValue(_currentScope.CurrentPropertyName, out propertySchema))
                {
                  schemas.Add(propertySchema);
                }
                if (schema.PatternProperties != null)
                {
                  foreach (KeyValuePair<string, JsonSchemaModel> patternProperty in schema.PatternProperties)
                  {
                    if (Regex.IsMatch(_currentScope.CurrentPropertyName, patternProperty.Key))
                    {
                      schemas.Add(patternProperty.Value);
                    }
                  }
                }

                if (schemas.Count == 0 && schema.AllowAdditionalProperties && schema.AdditionalProperties != null)
                  schemas.Add(schema.AdditionalProperties);
              }

              return schemas;
            }
          case JTokenType.Array:
            {
              IList<JsonSchemaModel> schemas = new List<JsonSchemaModel>();
              
              foreach (JsonSchemaModel schema in CurrentSchemas)
              {
                if (!CollectionUtils.IsNullOrEmpty(schema.Items))
                {
                  if (schema.Items.Count == 1)
                    schemas.Add(schema.Items[0]);

                  if (schema.Items.Count > (_currentScope.ArrayItemCount - 1))
                    schemas.Add(schema.Items[_currentScope.ArrayItemCount - 1]);
                }

                if (schema.AllowAdditionalProperties && schema.AdditionalProperties != null)
                  schemas.Add(schema.AdditionalProperties);
              }

              return schemas;
            }
          case JTokenType.Constructor:
            return Enumerable.Empty<JsonSchemaModel>();
          default:
            throw new ArgumentOutOfRangeException("TokenType", "Unexpected token type: {0}".FormatWith(CultureInfo.InvariantCulture, _currentScope.TokenType));
        }
      }
    }

    private void RaiseError(string message, JsonSchemaModel schema)
    {
      IJsonLineInfo lineInfo = this;

      string exceptionMessage = (lineInfo.HasLineInfo())
                                  ? message + " Line {0}, position {1}.".FormatWith(CultureInfo.InvariantCulture, lineInfo.LineNumber, lineInfo.LinePosition)
                                  : message;

      OnValidationEvent(new JsonSchemaException(exceptionMessage, null, lineInfo.LineNumber, lineInfo.LinePosition));
    }

    private void OnValidationEvent(JsonSchemaException exception)
    {
      ValidationEventHandler handler = ValidationEventHandler;
      if (handler != null)
        handler(this, new ValidationEventArgs(exception));
      else
        throw exception;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonValidatingReader"/> class that
    /// validates the content returned from the given <see cref="JsonReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from while validating.</param>
    public JsonValidatingReader(JsonReader reader)
    {
      ValidationUtils.ArgumentNotNull(reader, "reader");
      _reader = reader;
      _stack = new Stack<SchemaScope>();
    }

    /// <summary>
    /// Gets or sets the schema.
    /// </summary>
    /// <value>The schema.</value>
    public JsonSchema Schema
    {
      get { return _schema; }
      set
      {
        if (TokenType != JsonToken.None)
          throw new Exception("Cannot change schema while validating JSON.");

        _schema = value;
        _model = null;
      }
    }

    /// <summary>
    /// Gets the <see cref="JsonReader"/> used to construct this <see cref="JsonValidatingReader"/>.
    /// </summary>
    /// <value>The <see cref="JsonReader"/> specified in the constructor.</value>
    public JsonReader Reader
    {
      get { return _reader; }
    }

    private void ValidateInEnumAndNotDisallowed(JsonSchemaModel schema)
    {
      if (schema == null)
        return;

      JToken value = new JValue(_reader.Value);

      if (schema.Enum != null)
      {
        if (!schema.Enum.ContainsValue(value, new JTokenEqualityComparer()))
          RaiseError("Value {0} is not defined in enum.".FormatWith(CultureInfo.InvariantCulture, value),
                     schema);
      }

      JsonSchemaType? currentNodeType = GetCurrentNodeSchemaType();
      if (currentNodeType != null)
      {
        if (JsonSchemaGenerator.HasFlag(schema.Disallow, currentNodeType.Value))
          RaiseError("Type {0} is disallowed.".FormatWith(CultureInfo.InvariantCulture, currentNodeType), schema);
      }
    }

    private JsonSchemaType? GetCurrentNodeSchemaType()
    {
      switch (_reader.TokenType)
      {
        case JsonToken.StartObject:
          return JsonSchemaType.Object;
        case JsonToken.StartArray:
          return JsonSchemaType.Array;
        case JsonToken.Integer:
          return JsonSchemaType.Integer;
        case JsonToken.Float:
          return JsonSchemaType.Float;
        case JsonToken.String:
          return JsonSchemaType.String;
        case JsonToken.Boolean:
          return JsonSchemaType.Boolean;
        case JsonToken.Null:
          return JsonSchemaType.Null;
        default:
          return null;
      }
    }

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:Byte[]"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:Byte[]"/> or a null reference if the next JSON token is null.
    /// </returns>
    public override byte[] ReadAsBytes()
    {
      byte[] data = _reader.ReadAsBytes();

      ValidateCurrentToken();
      return data;
    }

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="Nullable{Decimal}"/>.
    /// </summary>
    /// <returns>A <see cref="Nullable{Decimal}"/>.</returns>
    public override decimal? ReadAsDecimal()
    {
      decimal? d = _reader.ReadAsDecimal();

      ValidateCurrentToken();
      return d;
    }

#if !NET20
    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="Nullable{DateTimeOffset}"/>.
    /// </summary>
    /// <returns>A <see cref="Nullable{DateTimeOffset}"/>.</returns>
    public override DateTimeOffset? ReadAsDateTimeOffset()
    {
      DateTimeOffset? dateTimeOffset = _reader.ReadAsDateTimeOffset();

      ValidateCurrentToken();
      return dateTimeOffset;
    }
#endif

    /// <summary>
    /// Reads the next JSON token from the stream.
    /// </summary>
    /// <returns>
    /// true if the next token was read successfully; false if there are no more tokens to read.
    /// </returns>
    public override bool Read()
    {
      if (!_reader.Read())
        return false;

      if (_reader.TokenType == JsonToken.Comment)
        return true;

      ValidateCurrentToken();
      return true;
    }

    private void ValidateCurrentToken()
    {
      // first time validate has been called. build model
      if (_model == null)
      {
        JsonSchemaModelBuilder builder = new JsonSchemaModelBuilder();
        _model = builder.Build(_schema);
      }

      //ValidateValueToken();

      switch (_reader.TokenType)
      {
        case JsonToken.StartObject:
          ProcessValue();
          IList<JsonSchemaModel> objectSchemas = CurrentMemberSchemas.Where(ValidateObject).ToList();
          Push(new SchemaScope(JTokenType.Object, objectSchemas));
          break;
        case JsonToken.StartArray:
          ProcessValue();
          IList<JsonSchemaModel> arraySchemas = CurrentMemberSchemas.Where(ValidateArray).ToList();
          Push(new SchemaScope(JTokenType.Array, arraySchemas));
          break;
        case JsonToken.StartConstructor:
          Push(new SchemaScope(JTokenType.Constructor, null));
          break;
        case JsonToken.PropertyName:
          foreach (JsonSchemaModel schema in CurrentSchemas)
          {
            ValidatePropertyName(schema);
          }
          break;
        case JsonToken.Raw:
          break;
        case JsonToken.Integer:
          ProcessValue();
          foreach (JsonSchemaModel schema in CurrentMemberSchemas)
          {
            ValidateInteger(schema);
          }
          break;
        case JsonToken.Float:
          ProcessValue();
          foreach (JsonSchemaModel schema in CurrentMemberSchemas)
          {
            ValidateFloat(schema);
          }
          break;
        case JsonToken.String:
          ProcessValue();
          foreach (JsonSchemaModel schema in CurrentMemberSchemas)
          {
            ValidateString(schema);
          }
          break;
        case JsonToken.Boolean:
          ProcessValue();
          foreach (JsonSchemaModel schema in CurrentMemberSchemas)
          {
            ValidateBoolean(schema);
          }
          break;
        case JsonToken.Null:
          ProcessValue();
          foreach (JsonSchemaModel schema in CurrentMemberSchemas)
          {
            ValidateNull(schema);
          }
          break;
        case JsonToken.Undefined:
          break;
        case JsonToken.EndObject:
          foreach (JsonSchemaModel schema in CurrentSchemas)
          {
            ValidateEndObject(schema);
          }
          Pop();
          break;
        case JsonToken.EndArray:
          foreach (JsonSchemaModel schema in CurrentSchemas)
          {
            ValidateEndArray(schema);
          }
          Pop();
          break;
        case JsonToken.EndConstructor:
          Pop();
          break;
        case JsonToken.Date:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void ValidateEndObject(JsonSchemaModel schema)
    {
      if (schema == null)
        return;

      Dictionary<string, bool> requiredProperties = _currentScope.RequiredProperties;

      if (requiredProperties != null)
      {
        List<string> unmatchedRequiredProperties =
          requiredProperties.Where(kv => !kv.Value).Select(kv => kv.Key).ToList();

        if (unmatchedRequiredProperties.Count > 0)
          RaiseError("Required properties are missing from object: {0}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", unmatchedRequiredProperties.ToArray())), schema);
      }
    }

    private void ValidateEndArray(JsonSchemaModel schema)
    {
      if (schema == null)
        return;

      int arrayItemCount = _currentScope.ArrayItemCount;

      if (schema.MaximumItems != null && arrayItemCount > schema.MaximumItems)
        RaiseError("Array item count {0} exceeds maximum count of {1}.".FormatWith(CultureInfo.InvariantCulture, arrayItemCount, schema.MaximumItems), schema);

      if (schema.MinimumItems != null && arrayItemCount < schema.MinimumItems)
        RaiseError("Array item count {0} is less than minimum count of {1}.".FormatWith(CultureInfo.InvariantCulture, arrayItemCount, schema.MinimumItems), schema);
    }

    private void ValidateNull(JsonSchemaModel schema)
    {
      if (schema == null)
        return;

      if (!TestType(schema, JsonSchemaType.Null))
        return;

      ValidateInEnumAndNotDisallowed(schema);
    }

    private void ValidateBoolean(JsonSchemaModel schema)
    {
      if (schema == null)
        return;

      if (!TestType(schema, JsonSchemaType.Boolean))
        return;

      ValidateInEnumAndNotDisallowed(schema);
    }

    private void ValidateString(JsonSchemaModel schema)
    {
      if (schema == null)
        return;

      if (!TestType(schema, JsonSchemaType.String))
        return;

      ValidateInEnumAndNotDisallowed(schema);

      string value = _reader.Value.ToString();

      if (schema.MaximumLength != null && value.Length > schema.MaximumLength)
        RaiseError("String '{0}' exceeds maximum length of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.MaximumLength), schema);

      if (schema.MinimumLength != null && value.Length < schema.MinimumLength)
        RaiseError("String '{0}' is less than minimum length of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.MinimumLength), schema);

      if (schema.Patterns != null)
      {
        foreach (string pattern in schema.Patterns)
        {
          if (!Regex.IsMatch(value, pattern))
            RaiseError("String '{0}' does not match regex pattern '{1}'.".FormatWith(CultureInfo.InvariantCulture, value, pattern), schema);
        }
      }
    }

    private void ValidateInteger(JsonSchemaModel schema)
    {
      if (schema == null)
        return;

      if (!TestType(schema, JsonSchemaType.Integer))
        return;

      ValidateInEnumAndNotDisallowed(schema);
      
      long value = Convert.ToInt64(_reader.Value, CultureInfo.InvariantCulture);

      if (schema.Maximum != null)
      {
        if (value > schema.Maximum)
          RaiseError("Integer {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.Maximum), schema);
        if (schema.ExclusiveMaximum && value == schema.Maximum)
          RaiseError("Integer {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, value, schema.Maximum), schema);
      }

      if (schema.Minimum != null)
      {
        if (value < schema.Minimum)
          RaiseError("Integer {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.Minimum), schema);
        if (schema.ExclusiveMinimum && value == schema.Minimum)
          RaiseError("Integer {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, value, schema.Minimum), schema);
      }

      if (schema.DivisibleBy != null && !IsZero(value % schema.DivisibleBy.Value))
        RaiseError("Integer {0} is not evenly divisible by {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.DivisibleBy), schema);
    }

    private void ProcessValue()
    {
      if (_currentScope != null && _currentScope.TokenType == JTokenType.Array)
      {
        _currentScope.ArrayItemCount++;

        foreach (JsonSchemaModel currentSchema in CurrentSchemas)
        {
          if (currentSchema != null && currentSchema.Items != null && currentSchema.Items.Count > 1 && _currentScope.ArrayItemCount >= currentSchema.Items.Count)
            RaiseError("Index {0} has not been defined and the schema does not allow additional items.".FormatWith(CultureInfo.InvariantCulture, _currentScope.ArrayItemCount), currentSchema);
        }
      }
    }

    private void ValidateFloat(JsonSchemaModel schema)
    {
      if (schema == null)
        return;

      if (!TestType(schema, JsonSchemaType.Float))
        return;

      ValidateInEnumAndNotDisallowed(schema);
      
      double value = Convert.ToDouble(_reader.Value, CultureInfo.InvariantCulture);

      if (schema.Maximum != null)
      {
        if (value > schema.Maximum)
          RaiseError("Float {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Maximum), schema);
        if (schema.ExclusiveMaximum && value == schema.Maximum)
          RaiseError("Float {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Maximum), schema);
      }

      if (schema.Minimum != null)
      {
        if (value < schema.Minimum)
          RaiseError("Float {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Minimum), schema);
        if (schema.ExclusiveMinimum && value == schema.Minimum)
          RaiseError("Float {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Minimum), schema);
      }

      if (schema.DivisibleBy != null && !IsZero(value % schema.DivisibleBy.Value))
        RaiseError("Float {0} is not evenly divisible by {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.DivisibleBy), schema);
    }

    private static bool IsZero(double value)
    {
      double epsilon = 2.2204460492503131e-016;

      return Math.Abs(value) < 10.0 * epsilon;
    }

    private void ValidatePropertyName(JsonSchemaModel schema)
    {
      if (schema == null)
        return;

      string propertyName = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);

      if (_currentScope.RequiredProperties.ContainsKey(propertyName))
        _currentScope.RequiredProperties[propertyName] = true;

      if (!schema.AllowAdditionalProperties)
      {
        bool propertyDefinied = IsPropertyDefinied(schema, propertyName);

        if (!propertyDefinied)
          RaiseError("Property '{0}' has not been defined and the schema does not allow additional properties.".FormatWith(CultureInfo.InvariantCulture, propertyName), schema);
      }

      _currentScope.CurrentPropertyName = propertyName;
    }

    private bool IsPropertyDefinied(JsonSchemaModel schema, string propertyName)
    {
      if (schema.Properties != null && schema.Properties.ContainsKey(propertyName))
        return true;

      if (schema.PatternProperties != null)
      {
        foreach (string pattern in schema.PatternProperties.Keys)
        {
          if (Regex.IsMatch(propertyName, pattern))
            return true;
        }
      }

      return false;
    }

    private bool ValidateArray(JsonSchemaModel schema)
    {
      if (schema == null)
        return true;

      return (TestType(schema, JsonSchemaType.Array));
    }

    private bool ValidateObject(JsonSchemaModel schema)
    {
      if (schema == null)
        return true;

      return (TestType(schema, JsonSchemaType.Object));
    }

    private bool TestType(JsonSchemaModel currentSchema, JsonSchemaType currentType)
    {
      if (!JsonSchemaGenerator.HasFlag(currentSchema.Type, currentType))
      {
        RaiseError("Invalid type. Expected {0} but got {1}.".FormatWith(CultureInfo.InvariantCulture, currentSchema.Type, currentType), currentSchema);
        return false;
      }

      return true;
    }

    bool IJsonLineInfo.HasLineInfo()
    {
      IJsonLineInfo lineInfo = _reader as IJsonLineInfo;
      return (lineInfo != null) ? lineInfo.HasLineInfo() : false;
    }

    int IJsonLineInfo.LineNumber
    {
      get
      {
        IJsonLineInfo lineInfo = _reader as IJsonLineInfo;
        return (lineInfo != null) ? lineInfo.LineNumber : 0;
      }
    }

    int IJsonLineInfo.LinePosition
    {
      get
      {
        IJsonLineInfo lineInfo = _reader as IJsonLineInfo;
        return (lineInfo != null) ? lineInfo.LinePosition : 0;
      }
    }
  }
}