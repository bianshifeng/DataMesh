using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMP.Unity
{
    public static class ComponentExtensions
    {

        /// <summary>
        ///  Ensure that a component of type <typeparamref name="T"/> exists on the game object.
        ///  if it doesn't exist, creates it
        /// </summary>
        /// 
        public static T EnsureComponent<T>(this GameObject gameObject) where T:Component
        {
            T foundComponent = gameObject.GetComponent<T>();
            if(foundComponent == null)
            {
                return gameObject.AddComponent<T>();
            }

            return foundComponent;
        }


    }


}
