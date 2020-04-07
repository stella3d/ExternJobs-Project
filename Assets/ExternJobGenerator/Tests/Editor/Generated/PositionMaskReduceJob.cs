using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Stella3d.ExternJobGenerator
{
    public struct PositionMaskReduceJob : IJob
    {
        [ReadOnly] public readonly NativeArray<Vector3> Positions;

        [ReadOnly] public readonly NativeArray<int> Mask;

        [WriteOnly] public readonly NativeArray<Vector3> Reduced;

        public readonly int Length;

        [WriteOnly] public NativeArray<int> ReturnValue;

        public unsafe void Execute()
        {
            ReturnValue[0] = TestMethods.PositionMaskFilter(Positions.ReadPtr(), Mask.ReadPtr(), Reduced.Ptr(), Length);
        }
    }
}