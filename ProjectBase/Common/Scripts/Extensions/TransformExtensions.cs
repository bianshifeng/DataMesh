using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace IMP.Unity
{
    public  static class TransformExtensions 
    {

        private static void GetFullPath_P(StringBuilder stringBuilder, Transform transform, string delimiter, string prefix)
        {
            if(transform.parent == null)
            {
                stringBuilder.Append(prefix);
            }
            else
            {
                GetFullPath_P(stringBuilder, transform.parent, delimiter, prefix);
                stringBuilder.Append(delimiter);
            }

        }


        public static string GetFullPath( this Transform transform, string delimiter = ".", string prefix = "/")
        {

            StringBuilder m_stringBuilder = new StringBuilder();
            GetFullPath_P(m_stringBuilder, transform, delimiter, prefix);
            return m_stringBuilder.ToString();
        }


        private static IEnumerable<Transform> EnumerateHierarchyCore(this Transform root, ICollection<Transform> ignore)
        {
            var transformQueue = new Queue<Transform>();
            transformQueue.Enqueue(root);

            while (transformQueue.Count > 0)
            {
                var parentTransform = transformQueue.Dequeue();
                if (!parentTransform || ignore.Contains(parentTransform))
                {
                    continue;
                }

                for (var i = 0; i < parentTransform.childCount; i++)
                {
                    transformQueue.Enqueue(parentTransform.GetChild(i));
                }

                yield return parentTransform;
            }
        }


        public static IEnumerable<Transform> EnumerateHierarchy(this Transform root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            return root.EnumerateHierarchyCore(new List<Transform>(0));
        }

        public static IEnumerable<Transform> EnumerateHierarchy(this Transform root, ICollection<Transform> ignore)
        {
            if(root == null)
            {
                throw new ArgumentNullException("root");
            }

            if(ignore == null)
            {
                throw new ArgumentNullException("ignore", "Ignore collection can't be null, use EnumerateHierarchy(root) instead.");

            }

            return root.EnumerateHierarchyCore(ignore);
        }

    }


}
