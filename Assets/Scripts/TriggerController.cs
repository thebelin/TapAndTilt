using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tap.Tilt
{
    public class TriggerController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("TriggerEnter " + other.name);
            // Check if the other object has an attached SampleController
            SampleController sc = other.GetComponent<SampleController> ();
            // If Controller exists, call TriggerEnter on it
            if (sc != null)
                sc.TriggerEnter(gameObject);
        }
    }
}
