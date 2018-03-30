using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataMesh.AR.UI
{
    public class KeyboardManager : MonoBehaviour
    {
        public GameObject keyboardPrefab;

        private FloatKeyboard keyboard = null;
        
        private bool hasTurnOn = false;

        public FloatKeyboard Keyboard { get { return keyboard; } }

        public void Init()
        {
        }

        public void TurnOn()
        {
            if (keyboard == null)
            {
                GameObject obj = Instantiate<GameObject>(keyboardPrefab) as GameObject;
                keyboard = obj.GetComponent<FloatKeyboard>();

                obj.transform.SetParent(this.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
            }
            hasTurnOn = true;
        }

        public void TurnOff()
        {
            hasTurnOn = false;
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