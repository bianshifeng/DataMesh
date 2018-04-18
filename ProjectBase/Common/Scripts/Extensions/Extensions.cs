using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IMP.Unity
{
    public static class Extensions
    {
        public static float Duration(this AnimationCurve  curve)
        {
            if (curve == null || curve.length <= 1)
            {
                return 0.0f;
            }

            return Mathf.Abs(curve[curve.length - 1].time - curve[0].time);
        }

        /// <summary>
        /// Determines whether or not a ray is valid.
        /// </summary>
        /// <param name="ray">The ray being tested.</param>
        /// <returns>True if the ray is valid, false otherwise.</returns>
        public static bool isValid ( this Ray ray)
        {
            return (ray.direction != Vector3.zero);
        }


        #region UnityEngine.Object

        public static void DontDestroyOnLoad( this Object target)
        {
#if UNIT_EDITOR
            if(UnityEditor.EditorApplication.isPlaying)
#endif
            Object.DontDestroyOnLoad(target);
        }

#endregion


    }

}


