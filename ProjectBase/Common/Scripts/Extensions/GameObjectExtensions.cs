using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMP.Unity
{
    /// <summary>
    ///  Extension methods for Unity's GameObject class
    /// </summary>
    /// 

    public static class GameObjectExtensions
    {
        /// <summary>
        ///  Set the layer to the given object and the full hiearchy below it.
        /// </summary>
        /// 

        public  static void SetLayerRecursively(this GameObject root, int layer)
        {
            if(root == null)
            {
                throw new ArgumentNullException("root", "Root transform can't be null");
            }

            foreach( var child in root.transform.EnumerateHierarchy())
            {
                child.gameObject.layer = layer;
            }
        }


        public static void SetLayerRecursively(this GameObject root, int layer, out Dictionary<GameObject, int> cache)
        {
            if(root == null)
            {
                throw new ArgumentNullException("root");
            }

            cache = new Dictionary<GameObject, int>();

            foreach (var child in root.transform.EnumerateHierarchy())
            {
                cache[child.gameObject] = child.gameObject.layer;
                child.gameObject.layer = layer;
            }
        }


        public static void ApplyLayerCacheRecursively(this GameObject root, Dictionary<GameObject, int> cache)
        {
            if(root == null)
            {
                throw new ArgumentNullException("root");
            }
            if(cache == null)
            {
                throw new ArgumentNullException("cache");
            }

            foreach( var child in root.transform.EnumerateHierarchy())
            {
                int layer;

                if (!cache.TryGetValue(child.gameObject, out layer)) { continue; }

                child.gameObject.layer = layer;
                cache.Remove(child.gameObject);
            }
        }


        public static GameObject GetParentRoot(this GameObject child)
        {
            if (child.transform.parent == null)
            {
                return child;
            }
            else
            {
                return GetParentRoot(child.transform.parent.gameObject);
            }
                
        }
    }

}