using System;

namespace Ngonzalez.Util.CustomException
{

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

    }

}
