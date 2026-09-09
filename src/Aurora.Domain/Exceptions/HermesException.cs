using System;

namespace Aurora.Domain.Exceptions;

public class HermesException : Exception
{
    public int StatusCode { get; }

    public HermesException(string message, int statusCode = 0) : base(message)
    {
        StatusCode = statusCode;
    }
}
