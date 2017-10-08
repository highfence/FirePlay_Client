using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class KeyboardHandlerForDebug : MonoBehaviour
{
    public void Update()
    {
        var direction = Vector3.zero;

        if (Input.GetKey(KeyCode.DownArrow))
            direction += Vector3.down;
        if (Input.GetKey(KeyCode.RightArrow))
            direction += Vector3.right;
        if (Input.GetKey(KeyCode.LeftArrow))
            direction += Vector3.left;
        if (Input.GetKey(KeyCode.UpArrow))
            direction += Vector3.up;

        if (Input.GetKey(KeyCode.LeftControl))
            direction += Vector3.forward;
        if (Input.GetKey(KeyCode.LeftAlt))
            direction += Vector3.back;

        direction *= 0.3f;

        this.transform.position += direction;
    }
}
