using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMP.Unity
{
    public static class CameraCache
    {
        private static Camera cachedCamera;

        public  static Camera Main
        {
            get
            {
                if(cachedCamera == null)
                {
                    return Refresh(Camera.main);
                }

                return cachedCamera;

            }
        }

        public static Camera Refresh(Camera newMain)
        {
            return cachedCamera = newMain;
        }


    }


}

