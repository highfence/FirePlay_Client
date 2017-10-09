using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCamera : MonoBehaviour
{
    Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main; 
    }

    void Update ()
    {
        var mainCameraPos = _mainCam.transform.position;
        mainCameraPos.z = -27;
        this.transform.position = mainCameraPos;
	}
}
