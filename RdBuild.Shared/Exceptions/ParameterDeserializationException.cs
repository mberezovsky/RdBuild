using System;

namespace RdBuild.Shared.Exceptions;

public class ParameterDeserializationException : Exception
{
    public ParameterDeserializationException(string message) : base(message)
    {
    }
}