using System;
using System.Linq;

namespace JsonApiDotNetCore.Internal
{
    public static class JsonApiExceptionFactory
    {
        private const string JsonApiException = nameof(JsonApiException);
        private const string InvalidCastException = nameof(InvalidCastException);

        public static JsonApiException GetException(Exception exception)
        {
            var exceptionType = exception.GetType().ToString().Split('.').Last();
            switch(exceptionType)
            {
                case JsonApiException:
                    return (JsonApiException)exception;
                case InvalidCastException:
                    return new JsonApiException(409, exception.Message);
                default:
                    return new JsonApiException(500, exception.Message, GetExceptionDetail(exception.InnerException));
            }
        }

        private static string GetExceptionDetail(Exception exception)
        {
            string detail = null;
            while(exception != null)
            {
                detail = $"{detail}{exception.Message}; ";
                exception = exception.InnerException;
            }
            return detail;
        }
    }
}
