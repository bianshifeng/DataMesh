using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DataMesh.AR;
using DataMesh.AR.Interactive;

public class XBOXControllerSample : MonoBehaviour
{
    public Text DebugText;

    private MultiInputManager inputManager;

    void Start ()
    {
        StartCoroutine(WaitForInit());
    }

    private IEnumerator WaitForInit()
    {
        MEHoloEntrance entrance = MEHoloEntrance.Instance;

        while (!entrance.HasInit)
        {
            yield return null;
        }

        inputManager = MultiInputManager.Instance;
    }

    private void Update()
    {
        if (inputManager == null)
            return;

        float ltaxis = inputManager.controllerInput.GetAxisLeftTrigger();
        float rtaxis = inputManager.controllerInput.GetAxisRightTrigger();

        float hAxis = inputManager.controllerInput.GetAxisLeftThumbstickX();
        float vAxis = inputManager.controllerInput.GetAxisLeftThumbstickY();

        float htAxis = inputManager.controllerInput.GetAxisRightThumbstickX();
        float vtAxis = inputManager.controllerInput.GetAxisRightThumbstickY();

        bool a = inputManager.controllerInput.GetButton(XBoxControllerButton.A);
        bool b = inputManager.controllerInput.GetButton(XBoxControllerButton.B);
        bool x = inputManager.controllerInput.GetButton(XBoxControllerButton.X);
        bool y = inputManager.controllerInput.GetButton(XBoxControllerButton.Y);
        bool lb = inputManager.controllerInput.GetButton(XBoxControllerButton.LeftShoulder);
        bool rb = inputManager.controllerInput.GetButton(XBoxControllerButton.RightShoulder);
        bool ls = inputManager.controllerInput.GetButton(XBoxControllerButton.LeftThumbstick);
        bool rs = inputManager.controllerInput.GetButton(XBoxControllerButton.RightThumbstick);
        bool view = inputManager.controllerInput.GetButton(XBoxControllerButton.View);
        bool menu = inputManager.controllerInput.GetButton(XBoxControllerButton.Menu);

        DebugText.text =
            string.Format(
                "LeftStickX: {12:0.000} LeftStickY: {13:0.000}\n" +
                "RightStickX: {14:0.000} RightStickY: {15:0.000}\n" +
                "LTrigger: {0:0.000} RTrigger: {1:0.000}\n" +
                "A: {2} B: {3} X: {4} Y:{5}\n" +
                "LB: {6} RB: {7} LStick: {8} RStick:{9}\n" +
                "View: {10} Menu: {11}\n"
                ,
                ltaxis, rtaxis,
                a, b, x, y,
                lb, rb, ls, rs,
                view, menu,
                hAxis, vAxis,
                htAxis, vtAxis
                );
    }
    
}
