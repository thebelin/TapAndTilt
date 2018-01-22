using UnityEngine;
using System;

namespace Tap.Tilt
{

    [Serializable]
    public class SampleData : MonoBehaviour
    {
        // The name to show for the sample representation
        public string title;

        // The color to paint the identifiers and cursors
        public Color color;

        // The audio clip to use for this controller
        public AudioClip clip;
    }
}