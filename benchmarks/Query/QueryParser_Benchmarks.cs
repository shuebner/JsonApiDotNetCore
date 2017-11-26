using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Internal;
using JsonApiDotNetCore.Models;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Benchmarks.Query {
    [MarkdownExporter, SimpleJob(launchCount : 3, warmupCount : 10, targetCount : 20)]
    public class QueryParser_Benchmarks {
        private readonly BenchmarkFacade _queryParser;

        private const string ATTRIBUTE = "Attribute";
        private const string ASCENDING_SORT = ATTRIBUTE;
        private const string DESCENDING_SORT = "-" + ATTRIBUTE;

        public QueryParser_Benchmarks() {
            var controllerContextMock = new Mock<IControllerContext>();
            controllerContextMock.Setup(m => m.RequestEntity).Returns(new ContextEntity {
                Attributes = new List<AttrAttribute> {
                    new AttrAttribute(ATTRIBUTE) {
                        InternalAttributeName = ATTRIBUTE
                    }
                }
            });
            var options = new JsonApiOptions();
            _queryParser = new BenchmarkFacade(controllerContextMock.Object, options);
        }

        [Benchmark]
        public void AscendingSort() => _queryParser._ParseSortParameters(ASCENDING_SORT);

        [Benchmark]
        public void DescendingSort() => _queryParser._ParseSortParameters(DESCENDING_SORT);

        [Benchmark]
        public void ComplexQuery() => Run(100, () => _queryParser.Parse(
            new QueryCollection(
                new Dictionary<string, StringValues> { 
                    { $"filter[{ATTRIBUTE}]", new StringValues(new [] { "abc", "eq:abc" }) },
                    { $"sort", $"-{ATTRIBUTE}" },
                    { $"include", "relationship" },
                    { $"page[size]", "1" },
                    { $"fields[resource]", ATTRIBUTE },
                }
            )
        ));

        private void Run(int iterations, Action action) { 
            for (int i = 0; i < iterations; i++)
                action();
        }

        // this facade allows us to expose and micro-benchmark protected methods
        private class BenchmarkFacade : QueryParser {
            public BenchmarkFacade(
                IControllerContext controllerContext,
                JsonApiOptions options) : base(controllerContext, options) { }

            public void _ParseSortParameters(string value) => base.ParseSortParameters(value);
        }
    }
}
