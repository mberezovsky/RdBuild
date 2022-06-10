using System;
using System.Collections.Generic;

namespace RdBuild.Shared
{
    /// <summary>
    /// Ресурс, от которого зависит задача. Когда все ресурсы задачи переходят в состояние
    /// Ready, Задача сама переходит в состояние Ready.
    /// </summary>
    public class JobResource
    {
        public enum JobStates{
            Initializing,
            Awaiting,
            Processing,
            Done
        }

        public List<JobResource> AwaitingResources { get; } = new List<JobResource>();
        public List<JobResource> ReadyResources { get; } = new List<JobResource>();
    }
}