using System;

namespace SystemsR3.Infrastructure.Exceptions
{
    public class BindingException : Exception
    {
        public BindingException(string message) : base(message)
        {
        }
    }
}