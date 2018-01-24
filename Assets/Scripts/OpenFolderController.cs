using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Runtime.InteropServices;
namespace Tap.Tilt
{ 
    public class OpenFolderController :MonoBehaviour
        {
        [DllImport("user32.dll")]
        private static extern void FolderBrowserDialog();

        // The currently selected data folder which holds the samples to use
        public string selectedPath;

        // The server url for the Slab - Fondler server
        public string serverUrl;

        // The text area for the url
        public InputField serverText;

        // The canvas group to manage the visibility of the dialog
        private CanvasGroup canvasGroup;

        // Display a dialog asking for a folder to select
        public void ShowDialog()
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Debug.Log("dialog results: " + fbd.SelectedPath.ToString());
                // Assign the path they selected to the app
                selectedPath = fbd.SelectedPath.ToString();
                LoadMain();
            }
        }

        public void OnEnable()
        {
            // Don't destroy this loader object
            DontDestroyOnLoad(gameObject);

            if (serverText != null)
                serverText.text = PlayerPrefs.GetString("serverUrl", "https://slab-fondler.herokuapp.com");

            // Default the selected Path to the application path
            selectedPath = Application.dataPath;

            canvasGroup = GetComponent<CanvasGroup>();
        }

        // Save the selected server url
        public void SaveServer()
        {
            if (serverText.text != "")
            {
                serverUrl = serverText.text;
                // Save the setting
                PlayerPrefs.SetString("serverUrl", serverUrl);
            }
        }

        // Load the main scene and configure the components there according to the selected options
        public void LoadMain()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            // Find the loader and set the path
            SampleLoader loader = FindObjectOfType<SampleLoader>();
            if (loader != null)
            {
                loader.loadPath = selectedPath;
                StartCoroutine(loader.DoLoad());
            }


            // Find the SocketIONetwork and set the url
            SocketIONetwork socket = FindObjectOfType<SocketIONetwork>();
            if (socket != null)
            {
                socket.uri = serverText.text + "/socket.io/display";
                socket.SocketConnect();
            }
        }
    }
}
