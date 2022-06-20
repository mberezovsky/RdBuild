using System;
using System.Collections.Generic;

namespace RdBuild.Shared
{
    /// <summary>
    /// Ресурс, от которого зависит задача. Когда все ресурсы задачи переходят в состояние
    /// Ready, Задача сама переходит в состояние Ready.
    /// </summary>
    public abstract class JobResource
    {
        private readonly Guid m_id;

        public enum JobStates{
            Initializing,
            Awaiting,
            Processing,
            Done
        }

        // TODO Implement the dependencies check in tests.
        // TODO Implement the dependent resources processing.
        public List<JobResource> AwaitingResources { get; } = new List<JobResource>();
        public List<JobResource> ReadyResources { get; } = new List<JobResource>();

        public event EventHandler<JobStates> StateChanged;

        protected JobResource(Guid id)
        {
            m_id = id;
        }

        protected JobResource()
        {
            m_id = Guid.NewGuid();
        }

        public Guid Id => m_id;
    }

}