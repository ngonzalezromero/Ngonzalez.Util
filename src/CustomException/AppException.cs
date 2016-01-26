using System;
using System.Runtime.Serialization;

namespace Ngonzalez.Util
{
    [Serializable]
    public class AppException : Exception
    {
        public AppException()
        {
        }

        public AppException(string message)
            : base(message)
        {

        }

        public AppException(string message, Exception inner)
            : base(message, inner)
        {

        }


        protected AppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }

}

