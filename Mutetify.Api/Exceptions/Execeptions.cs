using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutetify.Api.Exceptions
{

    [Serializable]
    public class WebServiceNotFoundException : Exception
    {
        public WebServiceNotFoundException() { }
        public WebServiceNotFoundException(string message) : base(message) { }
        public WebServiceNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected WebServiceNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class ApiException : Exception
    {
        public ApiException() { }
        public ApiException(string message) : base(message) { }
        public ApiException(string message, Exception inner) : base(message, inner) { }
        protected ApiException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
