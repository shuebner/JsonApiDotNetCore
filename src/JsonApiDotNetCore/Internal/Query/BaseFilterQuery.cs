using System;

namespace JsonApiDotNetCore.Internal.Query
{
    public class BaseFilterQuery
    {
        protected FilterOperations GetFilterOperation(string prefix)
        {
            if (prefix.Length == 0) return FilterOperations.eq;

            if (Enum.TryParse(prefix, out FilterOperations opertion) == false)
                throw new JsonApiException(400, $"Invalid filter prefix '{prefix}'");

            return opertion;
        }
    }
}
