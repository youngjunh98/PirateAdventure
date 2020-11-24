using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBehavior : MonoBehaviour
{
    public PlayerController PlayerController { get; private set; }

    protected virtual void Awake ()
    {
        PlayerController = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
    }
}
