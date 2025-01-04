using System;

namespace EcsR3.Exceptions
{
    public class InvalidEntityException : Exception
    {
        public InvalidEntityException(string message) : base(message)
        {
        }
    }
}