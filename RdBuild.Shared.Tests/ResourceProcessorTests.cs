using System;
using System.Threading;
using NUnit.Framework;
using RdBuild.Shared.Tests.Utils;

namespace RdBuild.Shared.Tests
{
    public class ResourceProcessorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleOneResourceOneType()
        {
            var token = CancellationToken.None;
            ResourceProcessor processor = new ResourceProcessor();

            var res1 = new JobResource1();

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            processor.RegisterResource(res1, token);
            JobResource resource = null;
            try
            {
                resource = processor.GetResource<JobResource1>(cancellationTokenSource.Token);
                Assert.Fail("Must be cancelled");
            }
            catch (OperationCanceledException e)
            {
            }

            Assert.That(resource, Is.Null);

            processor.ResourceInitialized(res1, token);

            processor.Activate(res1);

            resource = processor.GetResource<JobResource1>(token);

            Assert.That(resource.GetType(), Is.EqualTo(typeof(JobResource1)));
            Assert.That(((JobResource1)resource).Id, Is.EqualTo(res1.Id));

            Assert.Pass();
        }

        [Test]
        public void SimpleTwoResourcesOneType()
        {
            var token = CancellationToken.None;

            ResourceProcessor processor = new ResourceProcessor();
            var processorDebug = processor.GetDebug();

            var res1 = new JobResource1();
            var res2 = new JobResource1();

            processor.RegisterResource(res1, token);
            processor.RegisterResource(res2, token);

            Assert.That(processorDebug.GetInitializingCount(res1.GetType()), Is.EqualTo(2));
            Assert.That(processorDebug.GetPreparingCount(res1.GetType()), Is.EqualTo(0));

            try
            {
                processor.Activate(res1);
                Assert.Fail("Can't activate resource which is not prepared");
            }
            catch (Exception)
            {
                // ignored
            }

            processor.ResourceInitialized(res1, token);
            Assert.That(processorDebug.GetInitializingCount(res1.GetType()), Is.EqualTo(1));
            Assert.That(processorDebug.GetPreparingCount(res1.GetType()), Is.EqualTo(1));
            processor.Activate(res1);

            Assert.That(processorDebug.GetPreparingCount(res1.GetType()), Is.EqualTo(0));
            Assert.That(processorDebug.GetActiveCount(res1.GetType()), Is.EqualTo(1));

            var res = processor.GetResource<JobResource1>(token);

            Assert.IsNotNull(res);

            Assert.That(processorDebug.GetActiveCount(res1.GetType()), Is.EqualTo(0));
        }
    }
}