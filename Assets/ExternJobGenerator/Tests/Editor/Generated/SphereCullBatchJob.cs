using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Stella3d.ExternJobGenerator
{
    public struct SphereCullBatchJob : IJob
    {
        [ReadOnly] public readonly NativeArray<Plane> Frustum;

        [ReadOnly] public readonly NativeArray<Sphere> Spheres;

        [WriteOnly] public readonly NativeArray<int> CullingMask;

        public readonly int Length;

        public unsafe void Execute()
        {
            TestMethods.SphereCullBatch(Frustum.ReadPtr(), Spheres.ReadPtr(), CullingMask.Ptr(), Length);
        }
    }
}
