using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataMesh.AR.Interactive;
using DataMesh.AR.UI;
using DataMesh.AR.Utility;


namespace DataMesh.AR.Anchor
{

    public class AdjustAnchorUI : MonoBehaviour
    {

        public Text nameText;
        public Text contentText;

        public CommonButton moveButton;
        public CommonButton rotateButton;
        public CommonButton gazeButton;
        public CommonButton markerButton;

        public CommonButton confirmButton;
        public CommonButton quitButton;

        public System.Action cbMoveClick;
        public System.Action cbRotateClick;
        public System.Action cbGazeClick;
        public System.Action cbMarkerClick;
        public System.Action cbConfirmClick;
        public System.Action cbQuitClick;

        public void Init()
        {
            moveButton.callbackClick = OnClickMove;
            rotateButton.callbackClick = OnClickRotate;
            gazeButton.callbackClick = OnClickGaze;
            markerButton.callbackClick = OnClickMarker;

            quitButton.callbackClick = OnClickQuit;
            confirmButton.callbackClick = OnClickConfirm;
        }

        public void TurnOn()
        {
            gameObject.SetActive(true);

            WaitSelectAnchor();
        }

        public void TurnOff()
        {
            gameObject.SetActive(false);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnClickMove(CommonButton b)
        {
            cbMoveClick?.Invoke();
        }
        private void OnClickRotate(CommonButton b)
        {
            cbRotateClick?.Invoke();
        }
        private void OnClickGaze(CommonButton b)
        {
            cbGazeClick?.Invoke();
        }
        private void OnClickMarker(CommonButton b)
        {
        }
        private void OnClickConfirm(CommonButton b)
        {
            cbConfirmClick?.Invoke();
        }
        private void OnClickQuit(CommonButton b)
        {
            cbQuitClick?.Invoke();
        }

        public void WaitSelectAnchor()
        {
            nameText.text = "Please select an Anchor";
            contentText.text = "Use AirTap to select an anchor object.";

            moveButton.SetButtonDisabled(true);
            rotateButton.SetButtonDisabled(true);
            gazeButton.SetButtonDisabled(true);
            markerButton.SetButtonDisabled(true);

            moveButton.SetButtonSelected(false);
            rotateButton.SetButtonSelected(false);
            gazeButton.SetButtonSelected(false);
            markerButton.SetButtonSelected(false);

            confirmButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(true);
        }

        public void SelectAnAnchor()
        {
            nameText.text = "Selet what you want from buttons below";
            contentText.text = "<b><color=#ffff00>Move:</color></b> move the anchor\n<b><color=#ffff00>Rotate:</color></b> rotate the anchor\n<b><color=#ffff00>Gaze:</color></b> set anchor by gaze\n<b><color=#ffff00>Marker:</color></b> use marker to set anchor";
            moveButton.SetButtonDisabled(false);
            rotateButton.SetButtonDisabled(false);
            gazeButton.SetButtonDisabled(false);
            markerButton.SetButtonDisabled(true);

            moveButton.SetButtonSelected(false);
            rotateButton.SetButtonSelected(false);
            gazeButton.SetButtonSelected(false);
            markerButton.SetButtonSelected(true);

            confirmButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(false);
        }

        public void BeginMove()
        {
            nameText.text = "Move the anchor";
            contentText.text = "Tap and hold anywhere, and then move your hand";
            moveButton.SetButtonSelected(true);
            rotateButton.SetButtonSelected(false);
            gazeButton.SetButtonSelected(false);
            markerButton.SetButtonSelected(false);
        }

        public void BeginRotate()
        {
            nameText.text = "Rotate the anchor";
            contentText.text = "Tap and hold anywhere, and then move your hand";
            moveButton.SetButtonSelected(false);
            rotateButton.SetButtonSelected(true);
            gazeButton.SetButtonSelected(false);
            markerButton.SetButtonSelected(false);
        }

        public void BeginGaze()
        {
            nameText.text = "Move the anchor";
            contentText.text = "The anchor will follow your gaze. See where you want to put and then AirTap";
            moveButton.SetButtonSelected(false);
            rotateButton.SetButtonSelected(false);
            gazeButton.SetButtonSelected(true);
            markerButton.SetButtonSelected(false);
        }

    }
}