using DataMesh.AR.Interactive;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace IMP.Unity
{
    public enum ObjectGestureType
    {
        None,
        Fitting,
        Move,
        Rotate,
        Scale,
    }

    public enum StabilizationPlaneType
    {
        FollowCamera,
        Normal,
        Customize,
    }
    public class TapToDrag : MonoBehaviour
    {

        protected MultiInputManager inputManager;

       
        public ObjectGestureType gestureType = ObjectGestureType.None;
        private Action cbTapEmpty;  //用于反馈没有点击到操作对象要做的记录和处理
        public GameObject currentActiveObject;

        private bool isTapActive = false;


        private bool isDragActive = false;
        private float m_dragSpeed = 10f;
        private Vector3 m_dragStartPos;

        private bool isRotateActive = false;
        private float m_rotateSpeed = 60f;

        private bool isScaleActive = false;
        private float m_scaleSpeed = 1.0f;

        [SerializeField]
        public UnityEvent eventUpdatePosition;
        [SerializeField]
        public UnityEvent eventObjectActive;
        [SerializeField]
        public UnityEvent eventObjectUnActive;



        private void OnClickNone()
        {
            ChangeGestureType(ObjectGestureType.None);
        }
        private void OnClickMove()
        {
            ChangeGestureType(ObjectGestureType.Move);
        }
        private void OnClickRotate()
        {
            ChangeGestureType(ObjectGestureType.Rotate);
        }
        private void OnClickScale()
        {
            ChangeGestureType(ObjectGestureType.Scale);
        }
        private void OnClickGaze()
        {
            ChangeGestureType(ObjectGestureType.Fitting);
        }



        private void Start()
        {
            isTapActive = false;
            isScaleActive = false;
            isRotateActive= false;
            isDragActive = false;

            inputManager = MultiInputManager.Instance;
            inputManager.cbTap += OnTap;
            ActiveGestureManager(true);
        }
        private void OnDestroy()
        {
            isTapActive = false;
            isScaleActive = false;
            isRotateActive = false;
            isDragActive = false;

            if (inputManager)
            {
                inputManager.cbTap -= OnTap;
                ActiveGestureManager(false);
            }
        }

        // Use this for initialization
        #region gesturemanager
        private void ActiveGestureManager(bool isActive)
        {
            if (isActive)
            {
                //点击手势
                //导航手势
                inputManager.cbNavigationStart += OnNavigationStart;
                inputManager.cbNavigationUpdate += OnNavigationUpdate;
                inputManager.cbNavigationEnd += OnNavigationEnd;

                // 操控手势
                inputManager.cbManipulationStart += OnManipulationStart;
                inputManager.cbManipulationUpdate += OnManipulationUpdate;
                inputManager.cbManipulationEnd += OnManipulationEnd;
            }
            else
            {
                //导航手势
                inputManager.cbNavigationStart -= OnNavigationStart;
                inputManager.cbNavigationUpdate -= OnNavigationUpdate;
                inputManager.cbNavigationEnd -= OnNavigationEnd;

                // 操控手势
                inputManager.cbManipulationStart -= OnManipulationStart;
                inputManager.cbManipulationUpdate -= OnManipulationUpdate;
                inputManager.cbManipulationEnd -= OnManipulationEnd;
            }


        }


        public void ChangeGestureType(ObjectGestureType type)
        {
            gestureType = type;

            if (type == ObjectGestureType.Move)
            {
                inputManager.ChangeToManipulationRecognizer();
            }
            else if (type == ObjectGestureType.Rotate)
            {
                inputManager.ChangeToNavigationRecognizer();
            }
            else if (type == ObjectGestureType.Scale)
            {
                inputManager.ChangeToNavigationRecognizer();
            }
            else if (type == ObjectGestureType.Fitting)
            {
                inputManager.ChangeToManipulationRecognizer();
                //currentAnchorInfo.mark.SetAdjustType(AnchorAdjestType.Free);
                // 直接开始fitting 
                //Debug.Log("start fitting!");
                StartFit();
            }
            else
            {
                inputManager.ChangeToManipulationRecognizer();
                //currentAnchorInfo.mark.SetAdjustType(AnchorAdjestType.None);
                //anchorUI.SelectAnAnchor();
            }
        }

        private void StartFit()
        {
            //if (currentAnchorInfo == null)
            //    return;

            //spatialMappingManager.DrawVisualMeshes = true;
            //SetLayerMask(LayerMaskType.Spacial);


            //currentAnchorInfo.mark.HideTips();

            //StartCoroutine(StartFitLate());
        }



        private void OnTap(int obj)
        {
            //判断是否点击到了要操作的对象
            if (inputManager.FocusedObject == this.gameObject)
            {
                currentActiveObject = this.gameObject;
                if (isTapActive)
                {
                    isTapActive = false;
                    eventObjectUnActive.Invoke();
                }
                else
                {
                    isTapActive = true;
                    eventObjectActive.Invoke();
                    //OnClickScale();
                }
            }
        }

 

        private void OnNavigationStart(Vector3 obj)
        {
            if (currentActiveObject == null)
                return;

            if (gestureType == ObjectGestureType.Rotate)
            {
                isRotateActive = true;
                // currentAnchorInfo.mark.HideTips();
            }

            if (gestureType == ObjectGestureType.Scale)
            {
                isScaleActive = true;
                // currentAnchorInfo.mark.HideTips();
            }

        }
        private void OnNavigationUpdate(Vector3 delta)
        {
            if (gestureType == ObjectGestureType.Rotate)
            {
                if (isRotateActive)
                {
                    Vector3 deltaRot = new Vector3(0, -delta.x, 0);
                    Debug.LogFormat("x:{0} y:{1} z:{2}", deltaRot.x, deltaRot.y, deltaRot.z);
                    currentActiveObject.transform.Rotate(deltaRot * m_rotateSpeed * Time.deltaTime, Space.Self);
                    eventUpdatePosition.Invoke();
                }
            }

            if (gestureType == ObjectGestureType.Scale)
            {
                if (isScaleActive)
                {
                    Vector3 deltaScale = new Vector3(delta.z, -delta.x, -delta.y);
                    Debug.LogFormat("x:{0} y:{1} z:{2}", deltaScale.x, deltaScale.y, deltaScale.z);
                    //float m_scaleSpeed = 1.0f;

                    Vector3 t_scale = currentActiveObject.transform.localScale;



                    float t_step = 1 + m_scaleSpeed * Time.deltaTime * deltaScale.x;
                    currentActiveObject.transform.localScale = t_scale * t_step;

                    eventUpdatePosition.Invoke();
                }
            }
        }
        private void OnNavigationEnd(Vector3 obj)
        {
            isScaleActive = false;
            isRotateActive = false;

            if (currentActiveObject == null)
                return;
            // currentAnchorInfo.mark.ShowTips();
        }




        private void OnManipulationStart(Vector3 obj)
        {
            if (currentActiveObject == null)
                return;
            if (gestureType == ObjectGestureType.Move)
            {
                isDragActive = true;
                //m_dragStartPos = currentAnchorInfo.mark.transform.position;
                m_dragStartPos = currentActiveObject.transform.position;
                //currentAnchorInfo.mark.HideTips();
            }

        }
        private void OnManipulationUpdate(Vector3 delta)
        {
            if (currentActiveObject == null)
                return;

            if (gestureType == ObjectGestureType.Move)
            {
                if (isDragActive)
                {
                    Vector3 d = new Vector3(delta.x * m_dragSpeed, delta.y * m_dragSpeed, delta.z * m_dragSpeed);
                    //Debug.Log("M--->" + d);
                    Vector3 newPosition = (m_dragStartPos + d);
                    currentActiveObject.transform.position = newPosition;
                    eventUpdatePosition.Invoke();
                    //currentAnchorInfo.FollowMark();
                }
            }
        }
        private void OnManipulationEnd(Vector3 obj)
        {

        }



        //private void OnTap(int obj)
        //{
        //    if (pm.FocusedObject == this.gameObject)
        //    {

        //Transform t_myCom = inputManager.FocusedObject.GetComponentInParent<Transform>();
        //if (t_myCom != null)
        //{
        //    //进行初始操作开始
        //}
        //if (t_myCom == null)
        //{
        //    if (cbTapEmpty != null)
        //    {
        //        cbTapEmpty();
        //    }
        //    return;
        //}

        //        if (!isTapActiveDrag)
        //        {
        //            isTapActiveDrag = true;
        //            BeginStartDrag();
        //        }
        //        else
        //        {

        //            BeginEndDrag();
        //            isTapActiveDrag = false;
        //        }
        //    }
        //}


        #endregion
    }
}
