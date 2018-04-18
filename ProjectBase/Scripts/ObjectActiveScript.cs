using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActiveScript : MonoBehaviour {

    private Color normalColor = new Color(0.25f, 0.6896552f, 1f);
    private Color selectedColor = new Color(1f, 0.7310345f, 0.25f);
    Renderer render;

    public GameObject activeBorder;
    public Collider activeCollider;

    private bool isSetFocused = false;

    public void SetActiveFocused()
    {
        Material mat = render.material;
        mat.SetColor("_EmissionColor", selectedColor);
        activeBorder.GetComponent<AlphaBlink>().StartBlink();
       // Collider col = activeBorder.GetComponent<Collider>();
    }

    public void SetUnActiveFocused()
    {
        Material mat = render.material;
        mat.SetColor("_EmissionColor", normalColor);
        activeBorder.GetComponent<AlphaBlink>().StopBlink();
        // Collider col = activeBorder.GetComponent<Collider>();
    }
    // Use this for initialization

}
