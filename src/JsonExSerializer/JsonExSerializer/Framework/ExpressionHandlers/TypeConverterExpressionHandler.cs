using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Expressions;
using JsonExSerializer.MetaData;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.Framework.ExpressionHandlers
{
    /// <summary>
    /// Expression handler that uses TypeConverters for serialization and deserialization
    /// </summary>
    public class TypeConverterExpressionHandler : ExpressionHandlerBase
    {
        /// <summary>
        /// Initializes a default instance with no Serialization Context
        /// </summary>
        public TypeConverterExpressionHandler()
            : base()
        {
        }

        /// <summary>
        /// Initializes an instance with a Serialization Context
        /// </summary>
        public TypeConverterExpressionHandler(IConfiguration config)
            : base(config)
        {
        }


        /// <summary>
        /// Gets an expression for a value by first converting it with its registered type converter and then calling Serialize
        /// </summary>
        /// <param name="value">the value to generate an expression for</param>
        /// <param name="currentPath">the current path to the value</param>
        /// <param name="serializer">serializer instance</param>
        /// <returns>an expression for the value</returns>
        public override Expression GetExpression(object value, JsonPath currentPath, IExpressionBuilder serializer)
        {
            IJsonTypeConverter converter = GetConverter(value);
            return GetExpression(value, converter, currentPath, serializer);
        }

        private IJsonTypeConverter GetConverter(object value)
        {
            TypeData handler = Config.TypeHandlerFactory[value.GetType()];
            IJsonTypeConverter converter = (handler.HasConverter ? handler.TypeConverter : (IJsonTypeConverter)value);
            return converter;
        }

        /// <summary>
        /// Gets an expression for a value by first converting it with a specific type converter and then calling Serialize.  This
        /// method can be called directly when using a Property Converter
        /// </summary>
        /// <param name="value">the value to generate an expression for</param>
        /// <param name="converter">the type converter to use for conversion</param>
        /// <param name="currentPath">the current path to the value</param>
        /// <param name="serializer">serializer instance</param>
        /// <returns>an expression for the value</returns>
        public Expression GetExpression(object value, IJsonTypeConverter converter, JsonPath currentPath, IExpressionBuilder serializer)
        {
            //TODO: Cast for now to avoid breaking compatibility
            object convertedObject = converter.ConvertFrom(value, (SerializationContext) Config);
            // call serialize again in case the new type has a converter
            Expression expr = serializer.Serialize(convertedObject, currentPath, null);
            serializer.SetCanReference(value);   // can't reference inside the object
            return expr;
        }

        /// <summary>
        /// Determines whether this handler can serialize an object of the specified type.  The type
        /// must have a converter assigned to it, or implement IJsonTypeConverter.
        /// </summary>
        /// <param name="objectType">the object type to check</param>
        /// <returns>true if this handler handles the type</returns>
        public override bool CanHandle(Type objectType)
        {
            TypeData handler = Config.TypeHandlerFactory[objectType];
            return handler.HasConverter || typeof(IJsonTypeConverter).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Converts an expression to an object by first Evaluating the expression as its converted type and
        /// then converting that result using a type converter.
        /// </summary>
        /// <param name="expression">the expression to convert</param>
        /// <param name="deserializer">deserializer instance</param>
        /// <returns>an object created from the expression</returns>
        public override object Evaluate(Expression expression, IDeserializerHandler deserializer)
        {
            Type sourceType = expression.ResultType;
            TypeData handler = Config.TypeHandlerFactory[sourceType];
            IJsonTypeConverter converter;
            if (typeof(IJsonTypeConverter).IsAssignableFrom(sourceType))
            {
                converter = (IJsonTypeConverter) Activator.CreateInstance(sourceType);
            }
            else
            {
                converter = handler.TypeConverter;
            }

            return Evaluate(expression, deserializer, converter);
        }


        /// <summary>
        /// Converts an expression to an object by first Evaluating the expression as its converted type and
        /// then converting that result using the specified type converter.
        /// </summary>
        /// <param name="expression">the expression to convert</param>
        /// <param name="deserializer">deserializer instance</param>
        /// <param name="converter">the converter to use to convert the object</param>
        /// <returns>an object created from the expression</returns>
        public object Evaluate(Expression expression, IDeserializerHandler deserializer, IJsonTypeConverter converter)
        {
            Type sourceType = expression.ResultType;
            Type destType = converter.GetSerializedType(sourceType);
            expression.ResultType = destType;
            object tempResult = deserializer.Evaluate(expression);
            //TODO: Cast for now to avoid breaking compatibility
            object result = converter.ConvertTo(tempResult, sourceType, (SerializationContext) Config);
            expression.OnObjectConstructed(result);
            if (result is IDeserializationCallback)
            {
                ((IDeserializationCallback)result).OnAfterDeserialization();
            }
            return result;
        }

        /// <summary>
        /// This method is invalid for TypeConverterExpressionHandler
        /// </summary>
        /// <exception cref="NotSupportedException">Evaluating an existing object is not supported by TypeConverterExpressionHandler</exception>
        public override object Evaluate(Expression expression, object existingObject, IDeserializerHandler deserializer)
        {
            //TODO: possibly allow this if the type implements IJsonTypeConverter itself
            throw new NotSupportedException("Cannot convert an existing object.");
        }

        /// <summary>
        /// This method is invalid for TypeConverterExpressionHandler
        /// </summary>
        /// <exception cref="NotSupportedException">Evaluating an existing object is not supported by TypeConverterExpressionHandler</exception>
        public void Evaluate(Expression valueExpression, object result, IDeserializerHandler deserializer, IJsonTypeConverter converter)
        {
            //TODO: possibly allow this if the type implements IJsonTypeConverter itself
            throw new NotSupportedException("Cannot convert an existing object.");
        }

        public override bool IsReferenceable(object value)
        {
            return IsReferenceable(value, GetConverter(value));
        }

        public virtual bool IsReferenceable(object value, IJsonTypeConverter converter)
        {
            //TODO: Cast for now to avoid breaking compatibility
            return converter.SupportsReferences(value.GetType(), (SerializationContext) this.Config);
        }
    }
}
