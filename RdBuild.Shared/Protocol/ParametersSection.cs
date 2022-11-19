using System;
using System.Collections.Generic;
using System.IO;
using RdBuild.Shared.Exceptions;
using RdBuild.Shared.SectionData;

namespace RdBuild.Shared.Protocol;

public class ParametersSection<TEnum> : Section where TEnum : System.Enum
{
    Dictionary<TEnum, string> m_parameters = new();
 
    public ParametersSection() : base(EHeaderSectionType.MainParametersSection)
    {
    }

    public override void SerializeBodyToStream(BinaryWriter writer)
    {
        var fullName = typeof(TEnum).FullName;
        if (fullName == null)
            throw new ArgumentException();
        writer.Write(fullName);
        writer.Write(m_parameters.Count);
        foreach (var (name, par) in m_parameters)
        {
            writer.Write(name.ToString());
            writer.Write(par);
        }
    }

    public override void DeserializeBodyFromStream(BinaryReader reader)
    {
        string enumName = reader.ReadString();
        if (enumName != typeof(TEnum).FullName)
            throw new EnumIncompatibleException();

        int dataLen = reader.ReadInt32();
        m_parameters.Clear();

        for (int i = 0; i < dataLen; i++)
        {
            string keyName = reader.ReadString();
            string val = reader.ReadString();

            var key = (TEnum)Enum.Parse(typeof(TEnum), keyName);

            if (m_parameters.ContainsKey(key))
                throw new ParameterDeserializationException("Key is already exist");
            m_parameters[key] = val;
        }
    }

    public override int GetBodyLength()
    {
        var fullName = typeof(TEnum).FullName;
        if (fullName == null)
            throw new ArgumentException();

        int bodyLen = sizeof(int)
                      + fullName.Length
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