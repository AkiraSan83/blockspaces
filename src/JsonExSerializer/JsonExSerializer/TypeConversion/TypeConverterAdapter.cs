/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace JsonExSerializer.TypeConversion
{

    /// <summary>
    /// TypeConverter that utilizes the System.ComponentModel.TypeConverter for
    /// a given type.
    /// </summary>
    public class TypeConverterAdapter : JsonConverterBase, IJsonTypeConverter
    {
        private TypeConverter _converter;

        /// <summary>
        /// Gets a TypeConverterAdapter instace for the type if there is a valid
        /// System.ComponentModel.TypeConverter for the type.  Otherwise
        /// it returns null
        /// </summary>
        /// <param name="ForType"></param>
        /// <returns></returns>
        public static TypeConverterAdapter GetAdapter(Type ForType)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(ForType);
            if (tc.CanConvertFrom(typeof(string)) && tc.CanConvertTo(typeof(string)))
                return new TypeConverterAdapter(tc);
            else
                return null;
        }
        public TypeConverterAdapter(TypeConverter converter) {
            _converter = converter;
        }

        public override object ConvertFrom(object item, SerializationContext serializationContext)
        {
            return _converter.ConvertToString(item);
        }

        public override object ConvertTo(object item, Type sourceType, SerializationContext serializationContext)
        {
            return _converter.ConvertFromString((string) item);
        }

        public override Type GetSerializedType(Type sourceType)
        {
            return typeof(string);
        }
    }
}
