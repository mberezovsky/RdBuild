using System;
using RdBuild.Shared.SectionData;

namespace RdBuild.Shared.Protocol;

public abstract class SectionHeader
{
    public abstract EHeaderSectionType SectionType { get; }
    public abstract int SectionSize { get; set; }
    public abstract void DoRegisterHandler();
}

// Описывает передаваемый объект
class ObjectSectionHeader : SectionHeader
{
    public override EHeaderSectionType SectionType => EHeaderSectionType.ObjectSection;

    public Type ObjectType { get; set; }


    public override int SectionSize { get; set; }
    public override void DoRegisterHandler()
    {
        throw new System.NotImplementedException();
    }
}

// Before serialization or after the sectionHeader is received from stream, the serialized data
// are stored in this section. It gives information about size of the section.
class SerializedSectionData
{
    private byte[] data;
}