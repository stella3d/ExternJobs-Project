using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Collections;






namespace Stella3d.ExternJobGenerator
{
    public static unsafe class TestMethods 
    {
        // Methods we want to generate jobs for are 
        // written in a native language, such as ISPC
        
        [DllImport("ExampleNativeMethods")]
        public static extern void MaskThreshold(
            [ReadOnly] float* levels, 
            [WriteOnly] int* mask, 
            float threshold, 
            int length);
        
        [DllImport("ExampleNativeMethods")]
        public static extern void SphereCullBatch(
            [ReadOnly] Plane* frustumPlanes, 
            [ReadOnly] Vector3* positions, 
            [ReadOnly] float* radii, 
            [WriteOnly] int* cullingMask, 
            int length);

        /// <summary>
        /// Compact all elements of a masked vector array into a continuous array
        /// </summary>
        /// <param name="mask">Inclusion mask - non-zero means include element</param>
        /// <param name="filtered">The array to filter into</param>
        /// <param name="length">The element count in the source array</param>
        /// <returns>The element count in the filtered array</returns>
        [DllImport("ExampleNativeMethods")]
        public static extern int PositionMaskFilter(
            [ReadOnly] Vector3* positions, 
            [ReadOnly] int* mask,
            [WriteOnly] Vector3* filtered,
            int length);

        public static void ScalePositions(Vector3* positions, float scaleFactor) { }
    }
}

