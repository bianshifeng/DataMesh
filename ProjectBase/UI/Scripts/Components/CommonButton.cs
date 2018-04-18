using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMP.Unity.UI
{
    public class CommonButton : MonoBehaviour
    {

        public enum RenderType
        {
            None,
            Color,
            Image
        }

        public Image targetImage;
        public Text nameText;   //button 里面的文本控件

        public Color normalColor;
        public Color hoverColor;
        public Color selectedColor;
        public Color disabledColor;
        public Color clickColor;


        public Image normalImage;
        public Image hoverImage;
        public Image selectedImage;
        public Image disabledImage;
        public Image clickImage;

        public System.Action<CommonButton> callbackClick;


        [HideInInspector]
        public object param;

        private bool isDiabled = false;
        private bool isHover = false;
        private bool isPress = false;
        private bool isSelected = false;


        private string buttonName;
        public string ButtonName
        {
            get
            {
                return buttonName;
            }

            set
            {
                buttonName = value;
                if(nameText != null)
                {
                    nameText.text = buttonName;
                }
            }
        }



        public void SetSize(int w, int h)
        {
            RectTransform t_trans = transform as RectTransform;
            t_trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            t_trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);

            BoxCollider t_butCollider = GetComponent<BoxCollider>();
            t_butCollider.size = new Vector3(w, h, 0);
        }

        private void RefreshButton()
        {
            if(targetImage != null)
            {
                if(isDiabled)
                {
                    targetImage.color = disabledColor;
                }
            }
        }

        private void Awake()
        {
            if(nameText == null)
            {
                Transform t = transform.Find("Text");  //自动寻找自己子目录下的一个组件，通过组建名。
                if(t!=null)
                {
                    nameText = t.GetComponent<Text>();
                }
            }

            if(nameText != null && buttonName != null)
            {
                nameText.text = buttonName;
            }
        }

       



        // Use this for initialization
        void Start()
        {

           

        }

        // Update is called once per frame
        void Update()
        {

        }
    }


}
