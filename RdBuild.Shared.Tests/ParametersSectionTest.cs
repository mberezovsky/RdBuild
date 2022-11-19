using System;
using System.IO;
using NUnit.Framework;
using RdBuild.Shared.Exceptions;
using RdBuild.Shared.Extensions;
using RdBuild.Shared.Protocol;
using RdBuild.Shared.Tests.Utils;

namespace RdBuild.Shared.Tests
{
    public class ParametersSectionTest
    {
        public enum MyTestEnum
        {
            Val1,
            Val2,
            Val3,
            Val4
        }

        [Test]
        public void EmptyParametersSectionSerializationTest()
        {
            var stream = new MemoryStream();
            var section = new ParametersSection<MyTestEnum>();

            Assert.That(section.Count, Is.EqualTo(0));

            var writer = new BinaryWriter(stream);
            section.SerializeBodyToStream(writer);

            stream.Position = 0;
            var newSection = new ParametersSection<MyTestEnum>();
            using (var reader = new BinaryReader(stream))
                newSection.DeserializeBodyFromStream(reader);

            Assert.That(0, Is.EqualTo(newSection.Count));
        }

        enum MyWrongEnum
        {

        }

        [Test]
        public void WrongEnumTypeDeserialization()
        {
            var stream = new MemoryStream();
            var outSection = new ParametersSection<MyTestEnum>();
            var writer = new BinaryWriter(stream);
            outSection.SerializeBodyToStream(writer);

            stream.Position = 0;
            var inSection = new ParametersSection<MyWrongEnum>();
            try
            {
                using (var reader = new BinaryReader(stream))
                    inSection.DeserializeBodyFromStream(reader);
            }
            catch (EnumIncompatibleException)
            {
                Assert.Pass();
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
            Assert.Fail();
        }

        [Test]
        public void FilledParametersSection()
        {
            var stream = new MemoryStream();
            var oval1 = FakeGenerator.Instance.GenerateString();
            var oval2 = FakeGenerator.Instance.GenerateString();
            var oval3 = "";

            var mySection = new ParametersSection<MyTestEnum>();
            mySection.Add(MyTestEnum.Val1, oval1);
            mySection.Add(MyTestEnum.Val2, oval2);
            mySection.Add(MyTestEnum.Val3, oval3);

            try
            {
                mySection.Add(MyTestEnum.Val1, "Wrong");
                Assert.Fail();
            }
            catch (InvalidDataException) { }
            catch (Exception) { Assert.Fail(); }

            var writer = new BinaryWriter(stream);
            mySection.SerializeBodyToStream(writer);

            stream.Position = 0;
            var inSection = new ParametersSection<MyTestEnum>();
            using (var reader = new BinaryReader(stream))
                inSection.DeserializeBodyFromStream(reader);

            Assert.That(inSection.Count, Is.EqualTo(mySection.Count));
            Assert.That(inSection[MyTestEnum.Val1], Is.EqualTo(mySection[MyTestEnum.Val1]));
            Assert.That(inSection[MyTestEnum.Val2], Is.EqualTo(mySection[MyTestEnum.Val2]));
            Assert.That(inSection[MyTestEnum.Val3], Is.EqualTo(mySection[MyTestEnum.Val3]));
        }
    }
}

