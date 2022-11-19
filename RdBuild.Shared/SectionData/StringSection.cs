using System.IO;
using System.Text;
using RdBuild.Shared.Protocol;

namespace RdBuild.Shared.SectionData;

public class StringSection : Section
{
    private string m_val;

    public StringSection() : base(EHeaderSectionType.StringSection)
    {
        
    }

    public StringSection(string val) : base(EHeaderSectionType.StringSection)
    {
        m_val = val;
    }

    public override void SerializeBodyToStream(BinaryWriter writer)
    {
        writer.Write(m_val);
    }

    public override void DeserializeBodyFromStream(BinaryReader reader)
    {
        m_val = reader.ReadString();
    }

    public override int GetBodyLength()
    {
        if (m_val == null)
            return 0;

        return m_val.Length;
    }
}