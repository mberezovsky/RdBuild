using System;
using System.IO;
using NUnit.Framework;
using RdBuild.Shared.Protocol;

namespace RdBuild.Shared.Tests
{
    public class ObjectSectionTest
    {

        class TestClass
        {
            int i = 0;
            string s = null;

            public int I
            {
                get => i; set => i = value; 
            }

            public string S
            {
                get => s;  set => s = value;
            }
        }

        [Test]
        public void NullObjectTest()
        {
            var objectSection = new ObjectSection<TestClass>();
            var memStream = new MemoryStream();
            var writer = new BinaryWriter(memStream);
            objectSection.SerializeBodyToStream(writer);

            memStream.Position = 0;
            var inObjSection = new ObjectSection<TestClass>();
            using (var reader = new BinaryReader(memStream))
                inObjSection.DeserializeBodyFromStream(reader);

            Assert.That(inObjSection.Get(), Is.Null);
        }

        [Test]
        public void FillObjectTest()
        {
            var objectSection = new ObjectSection<TestClass>(new TestClass(){I = 10, S = "Hello"});
            var memStream = new MemoryStream();

            var writer = new BinaryWriter(memStream);
            objectSection.SerializeBodyToStream(writer);

            memStream.Position = 0;
            var inObjSection = new ObjectSection<TestClass>();
            using (var reader = new BinaryReader(memStream))
                inObjSection.DeserializeBodyFromStream(reader);

            Assert.That(inObjSection.Get(), Is.Not.Null);
            Assert.That(inObjSection.Get().I, Is.EqualTo(objectSection.Get().I));
            Assert.That(inObjSection.Get().S, Is.EqualTo(objectSection.Get().S));
        }

        [Test]
        public void StringObjectTest()
        {
            var objectSection = new ObjectSection<string>("Hello");
            var memStream = new MemoryStream();

            var writer = new BinaryWriter(memStream); 
            objectSection.SerializeBodyToStream(writer);

            memStream.Position = 0;
            var inObjSection = new ObjectSection<string>();
            using (var reader = new BinaryReader(memStream))
                inObjSection.DeserializeBodyFromStream(reader);

            Assert.That(objectSection.Get(), Is.EqualTo(inObjSection.Get()));
        }

        [Test]
        public void WrongDataTypeTest()
        {
            var objectSection = new ObjectSection<string>("Hello");
            var memStream = new MemoryStream();

            using (var writer = new BinaryWriter(memStream))
                objectSection.SerializeBodyToStream(writer);

            memStream = new MemoryStream();
            var inObjStream = new ObjectSection<TestClass>();
            Assert.Catch(() =>
            {
                using (var reader = new BinaryReader(memStream))
                    inObjStream.DeserializeBodyFromStream(reader);
            });

        }
    }
}