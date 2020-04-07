using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Stella3d.ExternJobGenerator
{
    public struct ScalePositionsJob : IJob
    {
        public readonly NativeArray<Vector3> Positions;

        public readonly int Length;

        public readonly float ScaleFactor;

        public unsafe void Execute()
        {
            TestMethods.ScalePositions(Positions.Ptr(), Length, ScaleFactor);
        }
    }
}
