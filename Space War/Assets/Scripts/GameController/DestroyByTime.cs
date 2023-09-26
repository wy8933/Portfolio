using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    [Tooltip("The time to destroy the object"),SerializeField] 
    private float destroyTime;

    void Update()
    {
        Destroy(gameObject, destroyTime);
    }
}
