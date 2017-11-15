using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using JsonApiDotNetCore.Builders;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Internal.Generics;
using JsonApiDotNetCore.Models;
using JsonApiDotNetCore.Serialization;
using JsonApiDotNetCore.Services;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Benchmarks.Serialization {
    [MarkdownExporter]
    public class JsonApiDeserializer_Benchmarks {
        private const string TYPE_NAME = "simple-types";
        private static readonly string Content = JsonConvert.SerializeObject(new Document {
            Data = new DocumentData {
                Type = TYPE_NAME,
                    Id = "1",
                    Attributes = new Dictionary<string, object> {
                        {
                            "name",
                            Guid.NewGuid().ToString()
                        }
                    }
            }
        });

        private readonly JsonApiDeSerializer _jsonApiDeSerializer;

        public JsonApiDeserializer_Benchmarks() {
            var contextGraphBuilder = new ContextGraphBuilder();
            contextGraphBuilder.AddResource<SimpleType>(TYPE_NAME);
            var contextGraph = contextGraphBuilder.Build();

            var jsonApiContextMock = new Mock<IJsonApiContext>();
            jsonApiContextMock.SetupAllProperties();
            jsonApiContextMock.Setup(m => m.ContextGraph).Returns(contextGraph);
            jsonApiContextMock.Setup(m => m.AttributesToUpdate).Returns(new Dictionary<AttrAttribute, object>());

            var jsonApiOptions = new JsonApiOptions();
            jsonApiOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonApiContextMock.Setup(m => m.Options).Returns(jsonApiOptions);

            var genericProcessorFactoryMock = new Mock<IGenericProcessorFactory>();

            _jsonApiDeSerializer = new JsonApiDeSerializer(jsonApiContextMock.Object, genericProcessorFactoryMock.Object);
        }

        [Benchmark]
        public object DeserializeSimpleObject() => _jsonApiDeSerializer.Deserialize<SimpleType>(Content);

        private class SimpleType : Identifiable {
            [Attr("name")]
            public string Name { get; set; }
        }
    }
}
