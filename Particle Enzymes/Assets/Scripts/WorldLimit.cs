using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLimit : MonoBehaviour
{
    // detect when object exits this trigger
    void OnTriggerExit(Collider other)
    {
        // if the object is a particle
        if (other.gameObject.CompareTag("Particle"))
        {
            other.transform.position = Vector3.zero;
        }
    }
}
