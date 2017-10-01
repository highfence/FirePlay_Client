using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAutoDestory : MonoBehaviour
{
    public float delay = 0f;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, this.gameObject.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }
}
