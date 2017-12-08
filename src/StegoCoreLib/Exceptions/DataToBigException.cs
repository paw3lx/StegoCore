using System;

namespace StegoCore.Exceptions
{
    public class DataToBigException : Exception
    {
        private DataToBigException()
        {
        }

        public DataToBigException(string message)
            : base(message)
        {
        }

        public DataToBigException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}