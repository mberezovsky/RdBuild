using System.IO;
using RdBuild.Shared.SectionData;

namespace RdBuild.Shared.Protocol;

public class ParametersSectionHeader : SectionHeader
{
    private Section m_section;

    public ParametersSectionHeader(Section section)
    {
        m_section = section;
    }

    public ParametersSectionHeader(BinaryReader reader)
    {

    }

    public override EHeaderSectionType SectionType => EHeaderSectionType.MainParametersSection;
    public override int SectionSize { get; set; }

    public override void DoRegisterHandler()
    {
    }
}