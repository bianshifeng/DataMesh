using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataMesh.AR.Interactive;
using System;

public class CreateObject : MonoBehaviour {



    private MultiInputManager inputManager;
	// Use this for initialization
	void Start () {
        StartCoroutine(WaitForInit());
	}

    private IEnumerator WaitForInit()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update () {
		
	}

    void AddObject()
    {
        Vector3 hitPoint = inputManager.hitPoint;
    }
}
