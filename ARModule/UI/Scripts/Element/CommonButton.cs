using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataMesh.AR.Interactive;

namespace DataMesh.AR.UI
{

    public class CommonButton : MonoBehaviour
    {
        public enum TransitionType
        {
            None,
            Color,
            Image
        }

        //public TransitionType transitionType = TransitionType.Color;

        public Image targetImage;
        public Text buttonNameText;

        public Color normalColor;
        public Color hoverColor;
        public Color clickColor;
        public Color disabledColor;
        public Color selectedColor;

        public Image hoverImage;
        public Image clickImage;
        public Image disabledImage;
        public Image selectedImage;


        public System.Action<CommonButton> callbackClick;

        /// <summary>
        /// 作为备注的内容
        /// </summary>
        [HideInInspector]
        public object param;

        protected MultiInputManager inputManager;

        private bool isDisabled = false;
        private bool isHover = false;
        private bool isPress = false;
        private bool isSelected = false;

        private string buttonName;


        public string ButtonName
        {
            get { return buttonName; }
            set
            {
                buttonName = value;
                if (buttonNameText != null)
                {
                    buttonNameText.text = buttonName;
                }
            }
        }

        public void SetSize(int w, int h)
        {
            RectTransform trans = transform as RectTransform;
            trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);

            BoxCollider buttonCollider = GetComponent<BoxCollider>();
            buttonCollider.size = new Vector3(w, h, 0);

        }

        public void SetButtonDisabled(bool dis)
        {
            isDisabled = dis;
            RefreshButton();
        }

        public void SetButtonSelected(bool sel)
        {
            isSelected = sel;
            RefreshButton();
        }

        private void Awake()
        {
            if (buttonNameText == null)
            {
                Transform t = transform.Find("Text");
                if (t != null)
                {
                    buttonNameText = t.GetComponent<Text>();
                }
            }
            if (buttonNameText != null && buttonName != null)
                buttonNameText.text = buttonName;
        }

        private void Start()
        {
            RefreshButton();
        }

        private void OnTapOnObject()
        {
            isPress = true;
            RefreshButton();

            DealClick();

            if (callbackClick != null)
                callbackClick(this);

            TimerManager.Instance.RegisterTimer(ClickOver, 0.1f, 1);
        }

        private void ClickOver(Hashtable hashtable)
        {
            isPress = false;
            RefreshButton();
        }


        private void OnGazeEnterObject()
        {
            isHover = true;
            RefreshButton();

            DealEnter();
        }

        private void OnGazeExitObject()
        {
            isHover = false;
            RefreshButton();

            DealExit();
        }

        private void RefreshButton()
        {
            if (targetImage != null)
            {
                if (isDisabled)
                {
                    targetImage.color = disabledColor;
                }
                else if (isSelected)
                {
                    targetImage.color = selectedColor;
                }
                else if (isPress)
                {
                    targetImage.color = clickColor;
                }
                else if (isHover)
                {
                    targetImage.color = hoverColor;
                }
                else
                {
                    targetImage.color = normalColor;
                }
            }

            if (hoverImage != null)
                hoverImage.gameObject.SetActive(false);
            if (clickImage != null)
                clickImage.gameObject.SetActive(false);
            if (disabledImage != null)
                disabledImage.gameObject.SetActive(false);
            if (selectedImage != null)
                selectedImage.gameObject.SetActive(false);

            if (isDisabled)
            {
                if (disabledImage != null)
                    disabledImage.gameObject.SetActive(true);
            }
            else if (isSelected)
            {
                if (selectedImage != null)
                    selectedImage.gameObject.SetActive(true);
            }
            else if (isPress)
            {
                if (clickImage != null)
                    clickImage.gameObject.SetActive(true);
            }
            else if (isHover)
            {
                if (hoverImage != null)
                    hoverImage.gameObject.SetActive(true);
            }
            else
            {
            }
        }

        /// <summary>
        /// 子类可实现自己的点击处理方法 
        /// </summary>
        protected virtual void DealClick()
        {

        }

        /// <summary>
        /// 游标移入时，子类可实现自己的处理方法
        /// </summary>
        protected virtual void DealEnter()
        {

        }

        /// <summary>
        /// 游标移出时，子类可实现自己的处理方法
        /// </summary>
        protected virtual void DealExit()
        {

        }

    }
}