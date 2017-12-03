using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonApiDotNetCore.Models
{
    public class DocumentBase
    {
        [JsonProperty("links")]
        public RootLinks Links { get; set; }

        [JsonProperty("included")]
        public List<DocumentData> Included { get; set; }

        [JsonProperty("meta")]
        public Dictionary<string, object> Meta { get; set; }

        // http://www.newtonsoft.com/json/help/html/ConditionalProperties.htm
        public bool ShouldSerializeIncluded() => (Included != null);
        public bool ShouldSerializeMeta() => (Meta != null);
        public bool ShouldSerializeLinks() => (Links != null);
    }
}
