using UnityEngine;
using System.Runtime.CompilerServices;

namespace ScriptableObjectArchitecture
{
    /// <summary>
    /// Internal extension methods for <see cref="Quaternion"/>.
    /// </summary>
    public static class QuaternionExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
        /// <summary>
        /// Returns a <see cref="Vector4"/> instance where the component values are equal to this
        /// <see cref="Quaternion"/>'s components.
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        [MethodImpl( INLINE )] public static Vector4 ToVector4(this Quaternion quaternion)
        {
            return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }
}