using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Stella3d.ExternJobGenerator
{
    public static unsafe class NativeArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Ptr<T>(this NativeArray<T> array) where T : unmanaged
        {
            return (T*) array.GetUnsafePtr();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* ReadPtr<T>(this NativeArray<T> array) where T : unmanaged
        {
            return (T*) array.GetUnsafeReadOnlyPtr();
        }
    }
}