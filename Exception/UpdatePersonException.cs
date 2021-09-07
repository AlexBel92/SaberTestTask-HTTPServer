using System;

namespace HTTPServer
{
    class UpdatePersonException : Exception
    {
        public UpdatePersonException()
        {
        }

        public UpdatePersonException(string message)
            : base(message)
        {
        }

        public UpdatePersonException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
