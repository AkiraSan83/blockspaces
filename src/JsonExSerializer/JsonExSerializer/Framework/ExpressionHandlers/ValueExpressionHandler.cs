using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Expressions;
using System.Globalization;

namespace JsonExSerializer.Framework.ExpressionHandlers
{
    public class ValueExpressionHandler : ExpressionHandlerBase, IExpressionHandler
    {
        public ValueExpressionHandler()
        {
        }

        public override Expression GetExpression(object data, JsonPath currentPath, IExpressionBuilder serializer)
        {
            return new ValueExpression(data);
        }

        public override object Evaluate(Expression expression, IDeserializerHandler deserializer)
        {
            ValueExpression value = (ValueExpression)expression;
            if (value.ResultType.IsEnum)
                return Enum.Parse(value.ResultType, value.StringValue);
            else if (value.ResultType == typeof(object))
                return value.StringValue;
            else if (value.ResultType == typeof(string))
                return value.StringValue;
            else
                return Convert.ChangeType(value.Value, value.ResultType, CultureInfo.InvariantCulture);
        }

        public override object Evaluate(Expression expression, object existingObject, IDeserializerHandler deserializer)
        {
            throw new InvalidOperationException("Value types can not be updated");
        }

        public override bool CanHandle(Type ObjectType)
        {
            return (typeof(string).IsAssignableFrom(ObjectType)
                || typeof(char).IsAssignableFrom(ObjectType)
                || typeof(DateTime).IsAssignableFrom(ObjectType));
        }
    }
}
