using System.IO;
using RdBuild.Client;
using RdBuild.Shared.Extensions;
using RdBuild.Shared.SectionData;

namespace RdBuild.Shared.Protocol;

public abstract class Section
{
    protected Section(EHeaderSectionType sectionType)
    {
        HeaderSectionType = sectionType;
    }

    public EHeaderSectionType HeaderSectionType { get; set; }

    SectionHeader Header { get; set; }

    public void SerializeHeader(Stream stream)
    {
        stream.WriteToStream((int)HeaderSectionType);
        stream.WriteToStream(GetBodyLength());
    }

    public void DeserializeHeader(Stream stream)
    {
        stream.ReadFromStream(out int iHeaderType);
        HeaderSectionType = (EHeaderSectionType)iHeaderType;
        stream.ReadFromStream(out int bodyLen);
    }

    public abstract void SerializeBodyToStream(BinaryWriter binaryWriter);
    public abstract void DeserializeBodyFromStream(BinaryReader stream);

    public abstract int GetBodyLength();
}