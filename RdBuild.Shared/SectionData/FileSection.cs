using System;
using System.IO;
using RdBuild.Shared.Protocol;

namespace RdBuild.Shared.SectionData;

// Possibly it should be StreamSection.
public class FileSection : Section
{
    private readonly string m_fileName;

    public FileSection(string fileName) : this()
    {
        m_fileName = fileName;
    }

    public FileSection() : base(EHeaderSectionType.FileSection)
    {
        m_fileName = null;
    }


    public override void SerializeBodyToStream(BinaryWriter writer)
    {
        if (string.IsNullOrEmpty(m_fileName))
            throw new ArgumentException("FileName");

        if (!File.Exists(m_fileName))
            throw new FileNotFoundException(m_fileName);

        using var fs = File.Open(m_fileName, FileMode.Open, FileAccess.Read);
        
        fs.CopyTo(writer.BaseStream);
    }

    public override void DeserializeBodyFromStream(BinaryReader reader)
    {
        string fileName = m_fileName;
        if (string.IsNullOrEmpty(fileName))
            fileName = Path.GetTempFileName();

        using var fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write);
        reader.BaseStream.CopyTo(fs);
    }

    public override int GetBodyLength()
    {
        if (string.IsNullOrEmpty(m_fileName))
            throw new ArgumentException("FileName");

        if (!File.Exists(m_fileName))
            throw new FileNotFoundException(m_fileName);

        var fi = new FileInfo(m_fileName);

        return (int)fi.Length;

    }
}