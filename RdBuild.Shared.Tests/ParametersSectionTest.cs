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

            section.SerializeBodyToStream(stream);
            stream.Position = 0;

            var newSection = new ParametersSection<MyTestEnum>();
            newSection.DeserializeBodyFromStream(stream);

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
            outSection.SerializeBodyToStream(stream);
            stream.Position = 0;

            var inSection = new ParametersSection<MyWrongEnum>();
            try
            {
                inSection.DeserializeBodyFromStream(stream);
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

            mySection.SerializeBodyToStream(stream);
            stream.Position = 0;

            var inSection = new ParametersSection<MyTestEnum>();
            inSection.DeserializeBodyFromStream(stream);

            Assert.That(inSection.Count, Is.EqualTo(mySection.Count));
            Assert.That(inSection[MyTestEnum.Val1], Is.EqualTo(mySection[MyTestEnum.Val1]));
            Assert.That(inSection[MyTestEnum.Val2], Is.EqualTo(mySection[MyTestEnum.Val2]));
            Assert.That(inSection[MyTestEnum.Val3], Is.EqualTo(mySection[MyTestEnum.Val3]));
        }
    }
}

