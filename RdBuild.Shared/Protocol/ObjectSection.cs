using System;
using System.IO;
using System.Text.Json;
using RdBuild.Client;
using RdBuild.Shared.Extensions;
using RdBuild.Shared.SectionData;

namespace RdBuild.Shared.Protocol;

public class ObjectSection<TObject> : Section where TObject : class
{
    private string m_str;
    private TObject m_obj;

    public ObjectSection(TObject obj) : base(EHeaderSectionType.ObjectSection)
    {
        m_str = JsonSerializer.Serialize(obj);
        m_obj = obj;
    }

    public ObjectSection() : this(default(TObject)) {}

    public override void SerializeBodyToStream(BinaryWriter writer)
    {
        writer.Write(typeof(TObject).FullName);
        writer.Write(m_str);
    }

    public override void DeserializeBodyFromStream(BinaryReader reader)
    {
        var typeName = reader.ReadString();
        if (typeName != typeof(TObject).FullName)
            throw new InvalidCastException($"Can't cast {typeName} to {typeof(TObject).FullName}");

        m_str = reader.ReadString();
        m_obj = JsonSerializer.Deserialize<TObject>(m_str);
    }

    public override int GetBodyLength()
    {
        return ParameterUtilsEx.GetStreamLength(typeof(TObject).FullName) +
               ParameterUtilsEx.GetStreamLength(m_str);
    }

    public TObject Get()
    {
        return m_obj;
    }
}