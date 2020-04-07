using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Stella3d.ExternJobGenerator
{
    public struct MaskThresholdJob : IJob
    {
        [ReadOnly] public readonly NativeArray<float> Levels;

        [WriteOnly] public readonly NativeArray<int> Mask;

        public readonly float Threshold;

        public readonly int Length;

        public unsafe void Execute()
        {
            TestMethods.MaskThreshold(Levels.ReadPtr(), Mask.Ptr(), Threshold, Length);
        }
    }
}
