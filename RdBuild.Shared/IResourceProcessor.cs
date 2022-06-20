using System;
using System.Threading;

namespace RdBuild.Shared
{
    interface IResourceProcessor
    {
        void RegisterResource(JobResource resource, CancellationToken token);
        JobResource GetResource(Type resourceType, CancellationToken token);
    }

    internal interface IResourceProcessorDebug
    {
        int GetInitializingCount(Type t);
        int GetPreparingCount(Type t);
        int GetActiveCount(Type t);
    }
}