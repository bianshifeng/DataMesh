using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataMesh.AR;
using DataMesh.AR.Interactive;
using System;
using UnityEngine.Events;
using IMP.Unity.InputModule;


namespace IMP.Unity
{
    public class DragScript : MonoBehaviour, IFocusable, IInputHandler
    {


        #region  1 control state
        // base state machine params

        private bool isGazed;  // show gaze event
        public bool IsDraggingEnabled = true;  // contorl drag active status
        private bool isDragging;   // show dragging status
        #endregion

        #region 10 emit trigger
        /// <summary>
        /// Event triggered when dragging starts.
        /// </summary>
        public event Action StartedDragging;

        /// <summary>
        /// Event triggered when dragging.
        /// </summary>
        public event Action ActionDragging;

        /// <summary>
        /// Event triggered when dragging stops.
        /// </summary>
        public event Action StoppedDragging;
        #endregion

        // Use this for initialization

        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        public Transform HostTransform;

        [Tooltip("Scale by which hand movement in z is multiplied to move the dragged object.")]
        public float DistanceScale = 2f;

        public enum RotationModeEnum
        {
            Default,
            LockObjectRotation,
            OrientTowardUser,
            OrientTowardUserAndKeepUpright
        }

        public RotationModeEnum RotationMode = RotationModeEnum.Default;

        [Tooltip("Controls the speed at which the object will interpolate toward the desired position")]
        [Range(0.01f, 1.0f)]
        public float PositionLerpSpeed = 0.2f;

        [Tooltip("Controls the speed at which the object will interpolate toward the desired rotation")]
        [Range(0.01f, 1.0f)]
        public float RotationLerpSpeed = 0.2f;


        private Vector3 objRefForward;
        private Vector3 objRefUp;
        private float objRefDistance;
        private Quaternion gazeAngularOffset;
        private float handRefDistance;   //手到头的距离
        private Vector3 objRefGrabPoint;

        private Vector3 draggingPosition;
        private Quaternion draggingRotation;


        private Rigidbody hostRigidbody;
        private bool hostRigidbodyWasKinematic;






        void Start()
        {
            if (HostTransform == null)
            {
                HostTransform = this.transform;
            }

            this.hostRigidbody = HostTransform.GetComponent<Rigidbody>();

            StartCoroutine(WaitForInit());
        }

        private void OnDestroy()
        {
            if (isDragging)
            {
                StopDragging();
            }

            if (isGazed)
            {
                OnFocusExit();
            }
        }


        private void Update()
        {
            //if (IsDraggingEnabled && isDragging)
            //{
            //    UpdateDragging(Vector3.zero);
            //}
        }

        /// <summary>
        ///  You can set the object from drag or disable dragging use this interface
        /// </summary>
        /// <param name="isEnabled"> Indicates whether dragging should be enabled or disabled</param>
        public void SetDragging(bool isEnabled)
        {
            if (IsDraggingEnabled == isEnabled)
            {
                return;
            }

            IsDraggingEnabled = isEnabled;

            if (isDragging)
            {
                StopDragging();
            }

        }





        public void OnFocusEnter()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }
            if (isGazed)
            {
                return;
            }
            isGazed = true;
        }

        public void OnFocusExit()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (!isGazed)
            {
                return;
            }

            isGazed = false;
        }



        public void OnInputDown(InputEventData eventData)
        {
            StartDragging();
        }

        /// <summary>
        ///  Starts dragging the object
        /// </summary>
        public void StartDragging()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (isDragging)
            {
                return;
            }

            // TODO: robertes: Fix push/pop and single-hanlder model so that multiple handdraggable components
            // can be active at once.

            // Add self as a modal input handler, to get all inputs during the manipulation

            isDragging = true;
            if (hostRigidbody != null)
            {
                hostRigidbodyWasKinematic = hostRigidbody.isKinematic;
                hostRigidbody.isKinematic = true;
            }


            Vector3 initialDraggingPosition = HostTransform.position;

            Transform cameraTransform = CameraCache.Main.transform;

            Vector3 inputPosition = Vector3.zero; // TODO: 如何获取手势输入的空间点


            Vector3 pivotPosition = GetHandPivotPosition(cameraTransform);
            handRefDistance = Vector3.Magnitude(inputPosition - pivotPosition);
            objRefDistance = Vector3.Magnitude(initialDraggingPosition - pivotPosition);

            Vector3 objForward = HostTransform.forward;
            Vector3 objUp = HostTransform.up;

            // Store where the object was grabbed from
            objRefGrabPoint = cameraTransform.transform.InverseTransformDirection(HostTransform.position - initialDraggingPosition);

            Vector3 objDirection = Vector3.Normalize(initialDraggingPosition - pivotPosition);
            Vector3 handDirection = Vector3.Normalize(inputPosition - pivotPosition);

            objForward = cameraTransform.InverseTransformDirection(objForward); // the transform in camera space
            objUp = cameraTransform.InverseTransformDirection(objUp); // the transform up in camera space
            objDirection = cameraTransform.InverseTransformDirection(objDirection); // the transform direction in camera space
            handDirection = cameraTransform.InverseTransformDirection(handDirection); // the hand transform direction in camera space;


            objRefForward = objForward;
            objRefUp = objUp;


            // Store the initial offset between the hand and the object, so that we can consider it when dragging
            gazeAngularOffset = Quaternion.FromToRotation(handDirection, objDirection);
            draggingPosition = initialDraggingPosition;

            StartedDragging.RaiseEvent();
        }


        /// <summary>
        /// Update the position of the object being dragged.
        /// </summary>
        /// <param name="delta"> current changed  inputpoint</param>
        private void UpdateDragging(Vector3 delta)
        {
            Transform cameraTransform = CameraCache.Main.transform;



            Vector3 inputPosition = delta; // TODO: 这里需要知道实时的手势输入位置



            Vector3 pivotPosition = GetHandPivotPosition(cameraTransform);
            Vector3 newHandDirection = Vector3.Normalize(inputPosition - pivotPosition);

            newHandDirection = cameraTransform.InverseTransformDirection(newHandDirection); // transform in camera space
            Vector3 targetDirection = Vector3.Normalize(gazeAngularOffset * newHandDirection);
            targetDirection = cameraTransform.TransformDirection(targetDirection); // transform back to world space

            float currentHandDistance = Vector3.Magnitude(inputPosition - pivotPosition);

            float distanceRatio = currentHandDistance / handRefDistance;
            float distanceOffset = distanceRatio > 0 ? (distanceRatio - 1f) * DistanceScale : 0;  //current move offset
            float targetDistance = objRefDistance + distanceOffset;

            draggingPosition = pivotPosition + (targetDirection * targetDistance);




            // update rotation
            if (RotationMode == RotationModeEnum.OrientTowardUser || RotationMode == RotationModeEnum.OrientTowardUserAndKeepUpright)
            {
                draggingRotation = Quaternion.LookRotation(HostTransform.position - pivotPosition);
            }
            else if (RotationMode == RotationModeEnum.LockObjectRotation)
            {
                draggingRotation = HostTransform.rotation;
            }
            else // RotationModeEnum.Default
            {
                Vector3 objForward = cameraTransform.TransformDirection(objRefForward); // in world space
                Vector3 objUp = cameraTransform.TransformDirection(objRefUp); // in world space
                draggingRotation = Quaternion.LookRotation(objForward, objUp);
            }

            // Apply Final Position
            Vector3 newPosition = Vector3.Lerp(HostTransform.position, draggingPosition + cameraTransform.TransformDirection(objRefGrabPoint), PositionLerpSpeed);
            if (hostRigidbody == null)
            {
                HostTransform.position = newPosition;
            }
            else
            {
                hostRigidbody.MovePosition(newPosition);
            }

            // ApplyFinal Rotation

            Quaternion newRotation = Quaternion.Lerp(HostTransform.rotation, draggingRotation, RotationLerpSpeed);
            if (hostRigidbody == null)
            {
                HostTransform.rotation = newRotation;
            }
            else
            {
                hostRigidbody.MoveRotation(newRotation);
            }

            if (RotationMode == RotationModeEnum.OrientTowardUserAndKeepUpright)   //保持向上
            {
                Quaternion upRotation = Quaternion.FromToRotation(HostTransform.up, Vector3.up);
                HostTransform.rotation = upRotation * HostTransform.rotation;
            }

            //在这里进行通信刷新位置
            ActionDragging.RaiseEvent();
        }




        public void OnInputUp(InputEventData eventData)
        {
            StopDragging();
        }


        /// <summary>
        /// Stop dragging
        /// </summary>
        public void StopDragging()
        {
            if (!isDragging)
            {
                return;
            }
            isDragging = false;
            if (hostRigidbody != null)
            {
                hostRigidbody.isKinematic = hostRigidbodyWasKinematic;
            }

            StoppedDragging.RaiseEvent();

        }


        /// <summary>
        /// Gets the pivot position for the hand, which is approximated to the base of the neck;
        /// </summary>
        /// <param name="cameraTransform"></param>
        /// <returns> Pivot position for the hand</returns>
        private Vector3 GetHandPivotPosition(Transform cameraTransform)
        {

            return cameraTransform.position + new Vector3(0, -0.2f, 0) - cameraTransform.forward * 0.2f;

        }















        private MultiInputManager pm;
        private float moveSpeed = 1f;
        private bool isTapActiveDrag = false;
        private Vector3 manipulationStartPos = Vector3.zero;

        [SerializeField]
        public UnityEvent localObjectSelect;

        [SerializeField]
        public UnityEvent localObjectDraging;
        [SerializeField]
        public UnityEvent localObjectDragStop;

        [SerializeField]
        public UnityEvent localObjectUnselect;

        private IEnumerator WaitForInit()
        {
            MEHoloEntrance entrance = MEHoloEntrance.Instance;
            while (!entrance.HasInit)
            {
                yield return null;
            }

            pm = MultiInputManager.Instance;
            pm.cbTap += OnTap;


        }


        public void OnGazeExitObject()
        {
            OnFocusExit();
        }

        public void OnGazeEnterObject()
        {
            OnFocusEnter();
        }
        private void OnTap(int obj)
        {
            if (pm.FocusedObject == this.gameObject)
            {
                if (!isTapActiveDrag)
                {
                    isTapActiveDrag = true;
                    BeginStartDrag();
                }
                else
                {

                    BeginEndDrag();
                    isTapActiveDrag = false;
                }
            }
        }

        private void OnManipulationStart(Vector3 delat)
        {
            manipulationStartPos = this.transform.position;
            StartDragging();
        }
        private void OnManipulationEnd(Vector3 delta)
        {
            StopDragging();
        }
        private void OnManipulationUpdate(Vector3 delta)
        {
            if (IsDraggingEnabled && isDragging)
            {

                Vector3 d = new Vector3(delta.x * moveSpeed, delta.y * moveSpeed, delta.z * moveSpeed);
                Vector3 newPosition = (manipulationStartPos + d);

                if (hostRigidbody == null)
                {
                    HostTransform.position = newPosition;
                }
                else
                {
                    hostRigidbody.MovePosition(newPosition);
                }

                localObjectDraging.Invoke();
            }


        }


        private void BeginEndDrag()
        {
            pm.cbManipulationStart -= OnManipulationStart;
            pm.cbManipulationUpdate -= OnManipulationUpdate;
            pm.cbManipulationEnd -= OnManipulationEnd;

            localObjectUnselect.Invoke();
        }

        private void BeginStartDrag()
        {
            pm.ChangeToManipulationRecognizer();
            pm.cbManipulationStart += OnManipulationStart;
            pm.cbManipulationUpdate += OnManipulationUpdate;
            pm.cbManipulationEnd += OnManipulationEnd;

            localObjectSelect.Invoke();
        }


    }


}
