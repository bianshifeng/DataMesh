using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMP.Unity
{
    public class ObjectFaceToCamera : MonoBehaviour
    {

        public bool useOffset = false;
        public Vector3 rotateOffset;

        private Transform m_CameraTrans;
        private Transform m_selfTrans;

        private void Awake()
        {
            m_CameraTrans = CameraCache.Main.transform;
            m_selfTrans = this.transform;
        }

        private void LateUpdate()
        {
            Vector3 t_dir = m_selfTrans.position - m_CameraTrans.position;
            t_dir.y = 0;

            m_selfTrans.forward = t_dir;

            if(useOffset)
            {
                this.transform.localEulerAngles += rotateOffset;
            }
        }


    }


}

