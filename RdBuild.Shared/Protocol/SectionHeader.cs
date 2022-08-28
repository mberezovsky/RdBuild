using RdBuild.Client;

namespace RdBuild.Shared.Protocol;

public class SectionHeader
{
    public EHeaderSectionType SectionType { get; set; }
    public int SectionSize { get; set; }
}