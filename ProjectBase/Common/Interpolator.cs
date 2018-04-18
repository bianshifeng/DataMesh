using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMP.Unity
{
    public class Interpolator : MonoBehaviour
    {
        [Tooltip(" When interpolating, use unscaled time, this is useful for games that have a pause mechanism or otherwise adjust the game timescale")]
        public bool UseUnscaledTime = true;

        private const float smallNumber = 0.0000001f;

        // The movement speed in meters per second
        public float PositionPerSecond = 30.0f;

        // The rotation speed, in degrees pre second
        public float RotationDegressPerSecond = 720.0f;

        // Adjusts rotation speed based on angular distance.
        public float RotationSpeedScaler = 0.0f;

        // The amount to scale per second
        public float ScalePerSecond = 5.0f;

        [HideInInspector]
        public bool SmoothLerpToTarget = false;
        [HideInInspector]
        public float SmoothPositionLerpRatio = 0.5f;
        [HideInInspector]
        public float SmoothRotationLerpRatio = 0.5f;
        [HideInInspector]
        public float SmoothScaleLerpRatio = 0.5f;


        // Positon data
        private Vector3 targetPosition;
        public Vector3 TargetPosition
        {
            get
            {
                if (AnimatingPosition)
                {
                    return targetPosition;
                }
                return transform.position;
            }
        }
        public bool AnimatingPosition { get; private set; }

        public void SetTargetPosition(Vector3 target)
        {
            bool wasRunning = this.Running;
            targetPosition = target;
            float magsq = (targetPosition - transform.position).sqrMagnitude;

            if (magsq > smallNumber)
            {
                AnimatingPosition = true;
                enabled = true;

                if (InterpolationStated != null && !wasRunning)
                {
                    InterpolationStated();
                }
            }
            else
            {
                transform.position = target;
                AnimatingPosition = false;
            }

        }

        /// <summary>
        /// The event fired when an Interpolation is stated
        /// </summary>
        public event System.Action InterpolationStated;

        /// <summary>
        /// The event fired when an interpolation is completed.
        /// </summary>
        public event System.Action InterpolationDone;

        /// <summary>
        /// The velocity of a transform whose position is being interplated.
        /// </summary>
        public Vector3 PositionVelocity { get; private set; }

        private Vector3 oldPosition = Vector3.zero;


        /// <summary>
        /// True if position, rotation or scale are animating; false otherwise.
        /// </summary>
        public bool Running
        {
            get
            {
                return (AnimatingPosition);
            }
        }


        public void Awake()
        {
            this.targetPosition = transform.position;

            enabled = false;
        }




        public static Vector3 NonLinearInterpolateTo(Vector3 start, Vector3 target, float deltaTime, float speed)
        {

            // if no interpolation speed, jump to target value.
            if (speed <= 0.0f)
            {
                return target;
            }
            Vector3 distance = (target - start);


            // when close enough, jump to the target
            if (distance.sqrMagnitude <= Mathf.Epsilon)
            {
                return target;
            }

            // Apply the delta, then clamp so we don't overshoot the target
            Vector3 deltaMove = distance * Mathf.Clamp(deltaTime * speed, 0.0f, 1.0f);

            return start + deltaMove;
        }


        public void Update()
        {
            float deltaTime = UseUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            bool interpOccuredThisFrame = false;

            if (AnimatingPosition)
            {
                Vector3 lerpTargetPosition = targetPosition;
                if (SmoothLerpToTarget)
                {
                    lerpTargetPosition = Vector3.Lerp(transform.position, lerpTargetPosition, SmoothPositionLerpRatio);
                }

                Vector3 newPosition = NonLinearInterpolateTo(transform.position, lerpTargetPosition, deltaTime, PositionPerSecond);

                if ((targetPosition - newPosition).sqrMagnitude <= smallNumber)
                {
                    newPosition = targetPosition;
                    AnimatingPosition = false;
                }
                else
                {
                    interpOccuredThisFrame = true;
                }

                transform.position = newPosition;

                // calculate interpolatedVelocity and store position for next frame
                PositionVelocity = oldPosition - newPosition;
                oldPosition = newPosition;

            }


            if (!interpOccuredThisFrame)
            {
                if (InterpolationDone != null)
                {
                    InterpolationDone();
                }
                enabled = false;
            }
        }


        public void SnapToTarget()
        {
            if (enabled)
            {
                transform.position = TargetPosition;
                AnimatingPosition = false;

                enabled = false;

                if (InterpolationDone != null)
                {
                    InterpolationDone();
                }
            }
        }

        public void StopInterpolating()
        {
            if (enabled)
            {
                Reset();

                if (InterpolationDone != null)
                {
                    InterpolationDone();
                }

            }
        }


        public void Reset()
        {
            targetPosition = transform.position;
            AnimatingPosition = false;
            enabled = false;
        }






    }
}


