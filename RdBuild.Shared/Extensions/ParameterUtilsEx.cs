using System;
using System.IO;
using System.Text;

namespace RdBuild.Shared.Extensions;

static class ParameterUtilsEx
{
    internal static Stream WriteToStream(this Stream stream, int val)
    {
        stream.Write(BitConverter.GetBytes(val));
        return stream;
    }

    internal static Stream WriteToStream(this Stream stream, string str)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(str);
        stream.WriteToStream(buffer.Length);
        stream.Write(buffer);
        return stream;
    }

    internal static Stream ReadFromStream(this Stream stream, out int res)
    {
        byte[] buffer = new byte[sizeof(int)];
        if (stream.Read(buffer) != sizeof(int))
            throw new StreamReadingException();
        res = BitConverter.ToInt32(buffer);
        return stream;
    }

    internal static Stream ReadFromStream(this Stream stream, out string res)
    {

        stream.ReadFromStream(out int bufferLength);
        var buffer = new byte[bufferLength];
        if (stream.Read(buffer) != bufferLength)
            throw new StreamReadingException();
        res = Encoding.UTF8.GetString(buffer);
        return stream;
    }

    internal static int GetStreamLength(string str)
    {
        return sizeof(int) + str.Length;
    }

    public class StreamReadingException : Exception
    {

    }
}