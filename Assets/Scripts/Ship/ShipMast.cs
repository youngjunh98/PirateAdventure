using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMast : MonoBehaviour
{
    [SerializeField]
    private GameObject sailMast;
    [SerializeField]
    private GameObject stopMast;

    public bool IsUp
    {
        set
        {
            if (sailMast != null)
            {
                sailMast.SetActive (!value);
            }
            if (stopMast != null)
            {
                stopMast.SetActive (value);
            }
        }
    }
}
