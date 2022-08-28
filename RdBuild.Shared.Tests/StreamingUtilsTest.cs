using System;
using System.IO;
using NUnit.Framework;
using RdBuild.Shared.Extensions;
using RdBuild.Shared.Tests.Utils;

namespace RdBuild.Shared.Tests
{
    public class StreamingUtilsTest
    {
        [Test]
        public void SerializationTestOneWrite()
        {
            var stream = new MemoryStream();

            stream.WriteToStream(10);
            stream.Flush();
            stream.Position = 0;
            stream.ReadFromStream(out int data);

            Assert.That(data, Is.EqualTo(10));
        }

        [Test]
        public void SerializationTestIntsSomeWrites()
        {
            var stream = new MemoryStream();

            stream.WriteToStream(1);
            stream.WriteToStream(2);
            stream.WriteToStream(Int32.MaxValue);

            stream.Flush();
            stream.Position = 0;
            stream.ReadFromStream(out int i1);
            Assert.That(i1, Is.EqualTo(1));
            stream.ReadFromStream(out int i2);
            Assert.That(i2, Is.EqualTo(2));
            stream.ReadFromStream(out int i3);
            Assert.That(i3, Is.EqualTo(Int32.MaxValue));
        }

        private const string oneString = "Hello, world";

        [Test]
        public void SerializationStringOnce()
        {
            var stream = new MemoryStream();

            stream.WriteToStream(oneString);
            stream.Flush();
            stream.Position = 0;

            stream.ReadFromStream(out string var);
            Assert.That(var, Is.EqualTo(oneString));
        }

        private const string TwoString = "One more test";


            [Test] 
        public void SerializationMultipleStrings()
        {
            var stream = new MemoryStream();

            string val1 = FakeGenerator.Instance.GenerateString();
            string val2 = FakeGenerator.Instance.GenerateString();
            string val3 = FakeGenerator.Instance.GenerateString();
            string val4 = string.Empty;

            stream.WriteToStream(val1)
                .WriteToStream(val2)
                .WriteToStream(val3)
                .WriteToStream(val4)
                .Flush();

            stream.Position = 0;

            stream.ReadFromStream(out string sv1)
                .ReadFromStream(out string sv2)
                .ReadFromStream(out string sv3)
                .ReadFromStream(out string sv4);

            Assert.That(sv1, Is.EqualTo(val1));
            Assert.That(sv2, Is.EqualTo(val2));
            Assert.That(sv3, Is.EqualTo(val3));
            Assert.That(sv4, Is.EqualTo(val4));
        }

        [Test]
        public void SerializaionMixedData()
        {
            string v1 = FakeGenerator.Instance.GenerateString();
            int v2 = FakeGenerator.Instance.GenerateInt();
            string v3 = FakeGenerator.Instance.GenerateString();
            int v4 = FakeGenerator.Instance.GenerateInt();

            var stream = new MemoryStream();

            stream.WriteToStream(v1)
                .WriteToStream(v2)
                .WriteToStream(v3)
                .WriteToStream(v4)
                .Flush();

            stream.Position = 0;

            stream.ReadFromStream(out string ov1)
                .ReadFromStream(out int ov2)
                .ReadFromStream(out string ov3)
                .ReadFromStream(out int ov4)
                ;

            Assert.That(ov1, Is.EqualTo(v1));
            Assert.That(ov2, Is.EqualTo(v2));
            Assert.That(ov3, Is.EqualTo(v3));
            Assert.That(ov4, Is.EqualTo(v4));
        }
    }
}