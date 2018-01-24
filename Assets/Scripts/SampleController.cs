using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tap.Tilt
{
    /**
     * An audio sample controller which reacts to the user inputs
     */
    [RequireComponent(typeof(AudioSource))]
    public class SampleController : MonoBehaviour
    {
        // The sample which this controller controls
        public SampleData sampleData;

        // The minimum Volume
        public float minVolume = 0f;

        // The maximum Volume
        public float maxVolume = 2f;

        // The current tempo
        public float tempo = 1f;

        public float minPitch = -1f;

        public float maxPitch = 5f;
        
        // The current pitch adjustment
        public float pitch = 0f;

        // The game object to show selection
        public GameObject selectionObject;

        // The Label which shows the name of this sample
        public Text titleText;

        // The slider showing the volume
        public Slider volumeSlider;

        // The slider showing the pitch
        public Slider pitchSlider;

        // The audio Source
        private AudioSource audioSource;

        // update each time touch data is processed
        [HideInInspector]
        public TouchData[] lastTouches;

        // the last time Touch Started
        [HideInInspector]
        public float lastTouchStart;

        public void TriggerEnter(GameObject other)
        {
            Indicate();
        }
        public void TriggerLeave()
        {
            Dimmer();
        }
        // Use this for initialization
        void OnEnable()
        {
            audioSource = GetComponent<AudioSource>();
            if (selectionObject != null)
                selectionObject.SetActive(false);

            // stop the audio source
            audioSource.Stop();

            if (sampleData != null)
                SetData(sampleData);
        }

        // Turn on the indicator for this object
        public void Indicate()
        {
            selectionObject.SetActive(true);
            if (!audioSource.isPlaying)
                audioSource.Play();
        }

        // Turn off indicator for this object
        public void Dimmer()
        {
            selectionObject.SetActive(false);
            if (audioSource.isPlaying)
                audioSource.Stop();
        }

        // Set the Sample configuration
        public void SetData(SampleData newData)
        {
            Debug.Log("SetData: " + JsonUtility.ToJson(newData));
            sampleData = newData;
            if (sampleData != null)
            {
                // Set the title
                if (titleText != null)
                    titleText.text = sampleData.title;
                // Set the clip
                if (sampleData.clip != null)
                    audioSource.clip = sampleData.clip;

                // Color the things
                ColorThings ct = GetComponent<ColorThings>();
                if (ct != null)
                    ct.DoColor(sampleData.color);
            }
        }

        public void TouchInput(ControlData controlData)
        {
            string touchType = controlData.type;
            TouchData[] touches = controlData.touches;
            ScreenData screen = controlData.screen;

            // touchType will be touchstart, touchend, touchcancel, or touchmove
            switch (touchType)
            {
                case "touchstart":
                    // double tap drops the instrument in place
                    TouchStart(touches);
                    break;
                case "touchmove":
                    // User slide on this sample controls tempo, pitch
                    TouchMove(touches, screen);
                    break;
                case "touchend":
                    break;
                case "touchcancel":
                    break;
            }
            // update the lastTouches
            lastTouches = touches;

            // @todo: Tilt controls position of the sample
            // @todo: add a volume adjuster to react to volume change on the phone

        }

        // Get the corresponding previous touch location for this touch location
        private TouchData PreviousTouch(TouchData touch)
        {
            foreach (TouchData thisTouch in lastTouches)
                if (thisTouch.Equals(touch))
                    return thisTouch;

            return null;
        }

        void TouchStart(TouchData[] touches)
        {
            Debug.Log("Touch Start Sample");
            if (!audioSource.isPlaying)
                audioSource.Play();

            // @todo If the previous touchstart was within the doubleTapLimit
            // And it isn't the same touch
            // Drop the object

            // update the lastTouchStart
            lastTouchStart = Time.time;
        }

        private Vector3 TouchToVector2(TouchData touch, ScreenData screen)
        {
            return new Vector2(
                (touch.pageX - (screen.width / 2)) / screen.width,
                (touch.pageY - screen.height) / screen.height);
        }
        void TouchMove(TouchData[] touches, ScreenData screen)
        {
            if (audioSource == null)
                return;
            foreach (TouchData touch in touches)
            {
                // Get the difference between the previous touch and the current touch
                TouchData previousTouch = PreviousTouch(touch);
                if (previousTouch == null)
                    break;

                float diffX = (touch.pageX - previousTouch.pageX);
                float diffY = (touch.pageY - previousTouch.pageY);

                // 0 Touch moves the position of the sample
                if (touch.identifier == "0")
                {
                    // Apply the Vertical touch as depth to the object
                    // transform.Translate(new Vector3(0, 0, diffY * -.02f * Time.deltaTime));

                }
                // any touch past the first to select is a music control touch
                else
                {

                    Debug.Log("touchadjust x= " + diffX + " y=" + diffY);
                    diffX = diffX / 100;
                    diffY = diffY / 500;
                    if (audioSource.pitch + diffX < maxPitch && audioSource.pitch + diffX > minPitch)
                        // horizontal slide controls pitch
                        audioSource.pitch += diffX;

                    // vertical slide controls volume
                    // if (Mathf.Abs(diffY) > tolerance)
                    if (audioSource.volume - diffY < maxVolume && audioSource.volume - diffY > minVolume)
                        audioSource.volume -= diffY;

                    if (volumeSlider != null)
                        volumeSlider.value = audioSource.volume;

                    if (pitchSlider != null)
                        pitchSlider.value = audioSource.pitch;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
