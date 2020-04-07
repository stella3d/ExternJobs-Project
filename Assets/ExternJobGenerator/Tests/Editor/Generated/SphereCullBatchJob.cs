using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Stella3d.ExternJobGenerator
{
    public struct SphereCullBatchJob : IJob
    {
        [ReadOnly] public readonly NativeArray<Plane> FrustumPlanes;

        [ReadOnly] public readonly NativeArray<Vector3> Positions;

        [ReadOnly] public readonly NativeArray<float> Radii;

        [WriteOnly] public readonly NativeArray<int> CullingMask;

        public readonly int Length;

        public unsafe void Execute()
        {
            TestMethods.SphereCullBatch(FrustumPlanes.ReadPtr(), Positions.ReadPtr(), Radii.ReadPtr(), CullingMask.Ptr(), Length);
        }
    }
}
