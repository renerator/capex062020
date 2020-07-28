using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capex.Web.Content.exception
{
    [Serializable()]
    public class InvalidParameterExcelException : System.Exception
    {
        public InvalidParameterExcelException() : base() { }
        public InvalidParameterExcelException(string message) : base(message) { }
        public InvalidParameterExcelException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected InvalidParameterExcelException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}