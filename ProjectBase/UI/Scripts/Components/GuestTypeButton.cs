using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IMP.Unity;


namespace IMP.Unity.UI
{
    public class GuestTypeButton : MonoBehaviour
    {

        private GuestButtonManager m_manager;

        public ObjectGestureType gestureType;
        // Use this for initialization
        private Image image;
        private Color active = new Color(1f, 0.7f, 0.7f, 1f);
        private Color disactive = new Color(1f, 1f, 1f, 1f);

        private bool isHover = false;
        private bool isPress = false;
        private bool isSelected = false;

        // Use this for initialization

        public void Init(GuestButtonManager manager)
        {
            image = GetComponent<Image>();
            m_manager = manager;
        }

        void Start()
        {
            

        }


        // Update is called once per frame
        public void SetActive(bool b)
        {
            if (b)
            {
                image.color = active;
            }
            else
            {
                image.color = disactive;
            }
        }


        private void OnTapOnObject()
        {
            isPress = true;
            m_manager.ChangeGuestType(gestureType);
            TimerManager.Instance.RegisterTimer(ClickOver, 0.1f, 1);
        }
        private void ClickOver(Hashtable hashtable)
        {
            isPress = false;
        }

        private void OnGazeEnterObject()
        {
            isHover = true;
        }

        private void OnGazeExitObject()
        {
            isHover = false;
        }



    }

}

