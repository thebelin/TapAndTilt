using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tap.Tilt
{
    public class SampleManager : MonoBehaviour
    {
        // The samples to manage
        public SampleData[] samples;

        // The prefab to use for the instrument
        public GameObject instrument;

        // The distance to lay them out from the Manager
        public float distance = 1f;

        // The absolute max rotation to arrange on
        public float maxAngle = 1f;

        private SampleController sc;

        // Load samples on startup
        private void OnEnable()
        {
            // LoadSamples();
            sc = GetComponent<SampleController>();
        }

        // Use this to load the samples which are set
        public void LoadSamples()
        {
            // Destroy any containers which are already in this Manager
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            // If there's no samples, get the local ones
            if (samples.Length == 0)
                samples = GetComponents<SampleData>();

            float degreesBetweenSamples = maxAngle / (samples.Length / 2);
            int i = 0;
            foreach(SampleData sample in samples)
            {
                // Determine the current angle to lay out objects
                float currentDegree = -maxAngle + (i * degreesBetweenSamples);

                // Create a GameObject to contain the instrument
                GameObject container = new GameObject();
                container.transform.position = transform.position;
                container.transform.rotation = transform.rotation;
                container.name = sample.title;
                container.transform.SetParent(transform);

                // Create the instrument as a child of the container
                GameObject inst = Instantiate(instrument, container.transform) as GameObject;

                // Apply the sample data to the instrument
                SampleController sc = inst.GetComponent<SampleController>();
                if (sc != null)
                    sc.SetData(sample);

                // Turn the instrument to the currentDegree
                container.transform.rotation = new Quaternion(transform.rotation.x, currentDegree, transform.rotation.z, transform.rotation.w);

                // Move it back the specified distance
                inst.transform.localPosition = new Vector3(0, 0, distance);

                // Iterate
                i++;
            }
        }
    }

}
