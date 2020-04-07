using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Stella3d.ExternJobGenerator
{
    public struct SphereCullBatchFilterJob : IJob
    {
        [ReadOnly] public readonly NativeArray<Plane> Frustum;

        [ReadOnly] public readonly NativeArray<Sphere> Spheres;

        [WriteOnly] public readonly NativeArray<int> UnculledIndices;

        public readonly int Length;

        [WriteOnly] public NativeArray<int> Returned;
        public int ReturnValue => Returned[0];

        public unsafe void Execute()
        {
            Returned[0] = TestMethods.SphereCullBatchFilter(Frustum.ReadPtr(), Spheres.ReadPtr(), UnculledIndices.Ptr(), Length);
        }
    }
}
