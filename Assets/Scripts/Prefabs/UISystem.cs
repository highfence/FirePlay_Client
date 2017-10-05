using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    public Camera _uiCam;
    public Canvas _canvas;

    public void AttachUI(GameObject uiObject)
    {
        uiObject.transform.SetParent(_canvas.transform);
    }
}
