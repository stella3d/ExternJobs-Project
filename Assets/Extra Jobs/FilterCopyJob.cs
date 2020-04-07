
using System;
using Unity.Collections;
using Unity.Jobs;

namespace Stella3d
{
    public struct FilterCopyJob<T> : IJobFor, IJobParallelFor 
        where T: unmanaged
    {
        [NativeDisableParallelForRestriction]
        [ReadOnly] public NativeArray<T> Source;
        
        [ReadOnly] public NativeArray<int> SourceIndices;
        
        [WriteOnly] public NativeArray<T> Filtered;
        
        public void Execute(int i)
        {
            Filtered[i] = Source[SourceIndices[i]];
        }
    }
    
    public struct FilterCopyJob<T1, T2> : IJobFor, IJobParallelFor 
        where T1: unmanaged
        where T2: unmanaged
    {
        [NativeDisableParallelForRestriction]
        [ReadOnly] public NativeArray<T1> Source1;
        [ReadOnly] public NativeArray<T2> Source2;
        
        [ReadOnly] public NativeArray<int> SourceIndices;
        
        [WriteOnly] public NativeArray<T1> Filtered1;
        [WriteOnly] public NativeArray<T2> Filtered2;

        public void Execute(int i)
        {
            var sourceIndex = SourceIndices[i];
            Filtered1[i] = Source1[sourceIndex];
            Filtered2[i] = Source2[sourceIndex];
        }
    }
}