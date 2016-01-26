using System;
using System.Runtime.Serialization;

namespace Ngonzalez.Util
{

    [Serializable]
    public class ValidationApiException : Exception
    {
        public ValidationApiException()
        {
        }

        public ValidationApiException(string message)
            : base(message)
        {

        }

        public ValidationApiException(string message, Exception inner)
            : base(message, inner)
        {

        }

        protected ValidationApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }

}
