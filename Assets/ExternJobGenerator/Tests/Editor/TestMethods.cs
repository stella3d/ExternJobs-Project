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
        public static extern void SphereCullBatch(
            [ReadOnly] Plane* frustum, 
            [ReadOnly] Sphere* spheres, 
            [WriteOnly] int* cullingMask, 
            int length);
        
        /// <summary>
        /// Compact all elements of a masked vector array into a continuous array
        /// </summary>
        /// <param name="frustum">Camera frustum planes</param>
        /// <param name="unculledIndices">The array to write active indices to</param>
        /// <param name="length">The count of the source array</param>
        /// <returns>The count of the unculled indices list</returns>
        [DllImport("ExampleNativeMethods")]
        public static extern int SphereCullBatchFilter(
            [ReadOnly] Plane* frustum, 
            [ReadOnly] Sphere* spheres, 
            [WriteOnly] int* unculledIndices,        
            int length);

        public static void ScalePositions(Vector3* positions, int length, float scaleFactor)
        {
            var ptr = positions;
            for (int i = 0; i < length; i++)
            {
                *ptr = *ptr * scaleFactor;
                ptr++;
            }
        }
    }
}

