﻿using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Serializing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace DD4T.Serialization
{
    public class JSONSerializerService : BaseSerializerService
    {


        private JsonSerializer _serializer = null;
        public JsonSerializer Serializer
        {
            get
            {
                if (_serializer == null)
                {  
                    _serializer = new JsonSerializer();
                    _serializer.NullValueHandling = NullValueHandling.Ignore;
                    _serializer.Converters.Add(new FieldConverter());
                }

                return _serializer;
            }
        }

        public override string Serialize<T>(T input)
        {
            string result = string.Empty;
            using (StringWriter stringWriter = new StringWriter())
            {
                JsonWriter jsonWriter = new JsonTextWriter(stringWriter);
                Serializer.Serialize(jsonWriter, input);
                result = stringWriter.ToString();
                result = ((SerializationProperties)SerializationProperties).CompressionEnabled ? Compressor.Compress(result) : result;
            }
            return result;
        }

        public override T Deserialize<T>(string input) 
        {
            if (((SerializationProperties)SerializationProperties).CompressionEnabled)
            {
                input = Compressor.Decompress(input);
            }
            using (var inputValueReader = new StringReader(input))
            {
                JsonTextReader reader = new JsonTextReader(inputValueReader);
                return (T)Serializer.Deserialize<T>(reader);
            }
        }

        public override bool IsAvailable()
        {
            return Type.GetType("Newtonsoft.Json.JsonSerializer") != null;
        }
    }

    public class FieldConverter : CustomCreationConverter<IField>
    {
        public override IField Create(Type objectType)
        {
            return new Field();
        }
    }
}
