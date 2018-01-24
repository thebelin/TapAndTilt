using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Tap.Tilt
{
    // Go to the loader folder and load every song into a SampleData item
    // Send all the SampleData to the SampleManager
    public class SampleLoader : MonoBehaviour
    {
        // Set this to look somewhere besides the root path of the app for music
        public string loadPath;

        // The file types to look for when loading sample controllers
        private string[] fileTypes = { "*.wav", "*.ogg", "*.m4a" };

        // The colors to use for samples when they're loaded
        public Color[] colors = { Color.red, Color.magenta, Color.blue, Color.cyan, Color.green, Color.yellow };

        private AudioClip audioClip;

        private List<SampleData> samples;

        public void OnEnable()
        {
            StartCoroutine(DoLoad());
        }

        // Use this for initialization
        IEnumerator DoLoad()
        {
            samples = new List<SampleData>();
            // Get files
            loadPath = loadPath == "" ? Application.dataPath : loadPath;
            Debug.Log("Get Files " + loadPath);
            foreach (string fileType in fileTypes)
                yield return StartCoroutine(LoadFiles(fileType));

            // Send the list of samples to the manager
            SampleManager sm = GetComponent<SampleManager>();
            sm.samples = samples.ToArray();
            sm.LoadSamples();
        }

        IEnumerator LoadFiles(string fileType)
        {
            int i = 0;
            string[] files = Directory.GetFiles(loadPath, fileType);
            foreach (string file in files)
            {
                Debug.Log("File to load: " + file);
                SampleData sample = gameObject.AddComponent(typeof(SampleData)) as SampleData;
                sample.title = Path.GetFileName(file);
                sample.title = sample.title.Substring(0, sample.title.Length - 4);
                sample.color = colors[i];
                audioClip = new AudioClip();
                yield return StartCoroutine(SetupAudio(file));
                sample.clip = audioClip;
                samples.Add(sample);
                i++;
                if (i == colors.Length)
                    i = 0;
            }
        }

        /// <summary>
        /// Setups the audio.
        /// </summary>
        /// <returns>The audio.</returns>
        IEnumerator SetupAudio(string filename)
        {
            // Get the media content from the url specified by newMedia
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filename, AudioType.UNKNOWN);

            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Got audio: " + filename);
                audioClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
