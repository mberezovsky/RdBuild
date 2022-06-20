using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace RdBuild.Shared
{
    class ResourceProcessor : IResourceProcessor, IResourceProcessorDebug
    {
        class ResourceProcessorSlot
        {
            public readonly ConcurrentDictionary<Guid, JobResource> InitializingResources = new ConcurrentDictionary<Guid, JobResource>();
            public readonly ConcurrentDictionary<Guid, JobResource> PreparingResources = new ConcurrentDictionary<Guid, JobResource>();
            public readonly BlockingCollection<JobResource> ReadyToExecuteResources = new BlockingCollection<JobResource>();
        }

        private readonly ReaderWriterLockSlim m_readerWriterLockSlim = new ReaderWriterLockSlim();
        private readonly Dictionary<Type, ResourceProcessorSlot> m_resourceSlotByType = new Dictionary<Type, ResourceProcessorSlot>();

        public void RegisterResource(JobResource resource, CancellationToken token)
        {
            ResourceProcessorSlot res = null;
            m_readerWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                if (!m_resourceSlotByType.TryGetValue(resource.GetType(), out res))
                {
                    m_readerWriterLockSlim.EnterWriteLock();
                    try
                    {
                        res = new ResourceProcessorSlot();
                        m_resourceSlotByType.Add(resource.GetType(), res);
                    }
                    finally
                    {
                        m_readerWriterLockSlim.ExitWriteLock();
                    }
                }

                if (!res.InitializingResources.TryAdd(resource.Id, resource))
                    throw new ResourceProcessorException($"Resource '{resource.Id} is already initializing");
            }
            finally
            {
                m_readerWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        public JobResource GetResource(Type resourceType, CancellationToken token)
        {
            var slot = GetProcessorSlot(resourceType);
            return slot.ReadyToExecuteResources.Take(token);
        }

        public JobResource GetResource<T>(CancellationToken token)
        {
            return GetResource(typeof(T), token);
        }

        public void ResourceInitialized(JobResource res, CancellationToken token)
        {
            var slot = GetProcessorSlot(res.GetType());
            if (slot.InitializingResources.TryRemove(res.Id, out var val))
                if (!slot.PreparingResources.TryAdd(res.Id, val))
                    throw new ResourceProcessorException($"Can't make resource {res.Id} as Preparing");
        }

        private ResourceProcessorSlot GetProcessorSlot(Type t)
        {
            m_readerWriterLockSlim.EnterReadLock();
            try
            {
                if (m_resourceSlotByType.TryGetValue(t, out var res))
                    return res;

                throw new ResourceProcessorException($"No registered slots for type {t.FullName}");
            }
            finally
            {
                m_readerWriterLockSlim.ExitReadLock();
            }
        }

        public class ResourceProcessorException : Exception
        {
            public ResourceProcessorException(string str) : base(str) {}
        }

        public void Activate(JobResource res)
        {
            var slot = GetProcessorSlot(res.GetType());
            if (slot.PreparingResources.TryRemove(res.Id, out var val))
            {
                if (!slot.ReadyToExecuteResources.TryAdd(res))
                    throw new ResourceProcessorException($"Can't activate resource {res.Id}");
            }
            else
                throw new ResourceProcessorException($"No preparing resource {res.Id}");
        }

        public int GetInitializingCount(Type t) => GetProcessorSlot(t).InitializingResources.Count;

        public int GetPreparingCount(Type t) => GetProcessorSlot(t).PreparingResources.Count;

        public int GetActiveCount(Type t) => GetProcessorSlot(t).ReadyToExecuteResources.Count;

        internal IResourceProcessorDebug GetDebug() => this;
    }

}