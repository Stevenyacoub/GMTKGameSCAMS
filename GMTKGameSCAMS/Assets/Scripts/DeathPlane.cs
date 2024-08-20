using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            other.transform.position = spawnPosition.position;
        }
    }
}
