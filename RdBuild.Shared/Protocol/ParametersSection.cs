using System;
using System.Collections.Generic;
using System.IO;
using RdBuild.Client;
using RdBuild.Shared.Exceptions;
using RdBuild.Shared.Extensions;

namespace RdBuild.Shared.Protocol;

public class ParametersSection<TEnum> : Section where TEnum : System.Enum
{
    Dictionary<TEnum, string> m_parameters = new();
 
    public ParametersSection() : base(EHeaderSectionType.MainParametersSection)
    {
    }

    public override void SerializeBodyToStream(Stream stream)
    {
        stream.WriteToStream(typeof(TEnum).FullName);
        stream.WriteToStream(m_parameters.Count);
        foreach (var (name, par) in m_parameters)
        {
            stream.WriteToStream(name.ToString()).WriteToStream(par);
        }
    }

    public override void DeserializeBodyFromStream(Stream stream)
    {
        stream.ReadFromStream(out string enumName);
        if (enumName != typeof(TEnum).FullName)
            throw new EnumIncompatibleException();

        stream.ReadFromStream(out int dataLen);
        m_parameters.Clear();

        for (int i = 0; i < dataLen; i++)
        {
            stream.ReadFromStream(out string skey)
                .ReadFromStream(out string val);

            var key = (TEnum)Enum.Parse(typeof(TEnum), skey);

            if (m_parameters.ContainsKey(key))
                throw new ParameterDeserializationException("Key is already exist");
            m_parameters[key] = val;
        }
    }

    public override int GetBodyLength()
    {
        int bodyLen = sizeof(int)
                      + typeof(TEnum).FullName.Length
                      + sizeof(int) // parametersCount
            ;

        foreach (var (key, val) in m_parameters)
        {
            bodyLen += sizeof(int) + sizeof(int) + val.Length;
        }
        return bodyLen;
    }

    public int Count => m_parameters.Count;

    public string this[TEnum val] => m_parameters[val];

    public void Add(TEnum key, string val)
    {
        if (m_parameters.ContainsKey(key))
            throw new InvalidDataException();
        m_parameters[key] = val;
    }
}