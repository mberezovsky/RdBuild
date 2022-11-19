using System;
using System.IO;
using NUnit.Framework;
using RdBuild.Shared.SectionData;

namespace RdBuild.Shared.Tests
{
    public class FileSectionTests
    {
        [Test]
        public void WrongFileTest()
        {
            
            var fileSection = new FileSection("WrongFileName@sdfsdf&.hello");
            var memStream = new MemoryStream();
            using var writer = new BinaryWriter(memStream);
            Assert.Catch(() => fileSection.SerializeBodyToStream(writer));
        }

        [Test]
        public void CorrectFileTransfer()
        {
            string fileName = Path.GetTempFileName();
            string inFileName = Path.GetTempFileName();
            try
            {
                int fileSize = 10000;
                using (var ofstream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    for (int i = 0; i < fileSize; i++)
                    {
                        ofstream.Write(BitConverter.GetBytes(i));
                    }
                }

                var fileSection = new FileSection(fileName);
                var memStream = new MemoryStream();
                var writer = new BinaryWriter(memStream);
                fileSection.SerializeBodyToStream(writer);

                memStream.Position = 0;
                var inFileSection = new FileSection(inFileName);
                using (var reader = new BinaryReader(memStream))
                    inFileSection.DeserializeBodyFromStream(reader);

                Assert.That(AreFilesEqual(fileName, inFileName), Is.True);
            }
            finally 
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
                if (File.Exists(inFileName))
                    File.Delete(inFileName);
            }

        }

        private bool AreFilesEqual(string fileName, string inFileName)
        {
            try
            {
                var fi1 = new FileInfo(fileName);
                var fi2 = new FileInfo(inFileName);

                Assert.That(fi1.Exists, Is.True);
                Assert.That(fi2.Exists, Is.True);

                Assert.That(fi1.Length, Is.EqualTo(fi2.Length));

                using (var sf1 = File.Open(fileName, FileMode.Open, FileAccess.Read))
                using (var sf2 = File.Open(inFileName, FileMode.Open, FileAccess.Read))
                {
                    for (int i = 0; i < fi1.Length; i++)
                    {
                        var b1 = sf1.ReadByte();
                        var b2 = sf2.ReadByte();
                        if (b1 != b2)
                            return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}