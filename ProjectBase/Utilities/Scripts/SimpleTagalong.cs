using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IMP.Unity
{
    [RequireComponent(typeof(BoxCollider),typeof(Interpolator))]
    public class SimpleTagalong : MonoBehaviour
    {


        public float TagalongDistance = 2.0f;
        public bool EnforceDistance = true;
        public float PositionUpdateSpeed = 9.8f;
        public bool SmoothMotion = true;
        public float SmoothingFactor = 0.75f;


        protected BoxCollider tagalongCollider;
        protected Interpolator interpolator;

        protected Plane[] frustumPlanes;
        protected const int frustumLeft = 0;
        protected const int frustumRight = 1;
        protected const int frustumBottom = 2;
        protected const int frustumTop = 3;



        // Use this for initialization

        protected virtual void Start()
        {
            tagalongCollider = GetComponent<BoxCollider>();
            interpolator = GetComponent<Interpolator>();
        }
        // Update is called once per frame
        protected virtual void Update()
        {
            Camera mainCamera = Camera.main; 


        }

    }


}

