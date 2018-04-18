using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IMP.Unity.UI
{
    public class GuestButtonManager : MonoBehaviour
    {

        public GuestTypeButton MoveButton;
        public GuestTypeButton RotateButton;
        public GuestTypeButton ScaleButton;

        public TapToDrag guestBehavior;

        // Use this for initialization

        private void Awake()
        {
            init();
        }

        public void init()
        {
            MoveButton.Init(this);
            RotateButton.Init(this);
            ScaleButton.Init(this);

            MoveButton.SetActive(false);
            RotateButton.SetActive(false);
            ScaleButton.SetActive(false);

        }

        public void ChangeGuestType(ObjectGestureType type)
        {
            
            switch (type)
            {
                case ObjectGestureType.Move:
                    MoveButton.SetActive(true);
                    RotateButton.SetActive(false);
                    ScaleButton.SetActive(false);
                   
                    break;
                case ObjectGestureType.Rotate:
                    MoveButton.SetActive(false);
                    RotateButton.SetActive(true);
                    ScaleButton.SetActive(false);
                    break;
                case ObjectGestureType.Scale:
                    MoveButton.SetActive(false);
                    RotateButton.SetActive(false);
                    ScaleButton.SetActive(true);
                    break;
                default:
                    MoveButton.SetActive(false);
                    RotateButton.SetActive(false);
                    ScaleButton.SetActive(false);
                    break;
            }

            guestBehavior.ChangeGestureType(type);
        }

        // Update is called once per frame
        void Update()
        {

        }


        public void ActivePanel(bool isActive)
        {
            this.gameObject.SetActive(isActive);
            ChangeGuestType(ObjectGestureType.None);
        }


    }


}
