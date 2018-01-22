using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tap.Tilt
{
    // Use to control a player object
    public class GameController : MonoBehaviour
    {
        // The objects to use as the representative player
        public GameObject player;

        // The players in the game
        private List<PlayerController> players;

        // For startup
        private void OnEnable()
        {
            players = new List<PlayerController>();
        }

        // Call to join a user to the scene
        private PlayerController UserJoin(string socketId)
        {
            // Create a new user object for the new user
            GameObject thisPlayer = Instantiate(player, Vector3.zero, Quaternion.identity) as GameObject;

            // Assign the PlayerController in the player to the player list
            PlayerController pc = thisPlayer.GetComponent<PlayerController>();
            if (pc)
            {
                // Identify this user
                pc.socketId = socketId;
                players.Add(pc);
            }
            return pc;
        }

        // Call to exit a user from the scene
        public void UserExit(string socketId)
        {
            // find the user in the List
            PlayerController thisPlayer = players.Find(rec => {
                Debug.Log("rec" + rec.ToString() + rec.socketId + " / " + socketId);
                return rec.Equals(socketId);
                });

            if (thisPlayer == null)
            {
                Debug.Log("exit thisPlayer is null " +  socketId);
                return;
               
            }
            // destroy the user's avatar object
            thisPlayer.gameObject.SetActive(false);

            // remove the user from the list
            players.Remove(thisPlayer);
        }

        // Call to control a user in the scene
        public void UserControl(ControlData controlData)
        {
            // Get the player in the list with that controller id
            PlayerController thisPlayer = players.Find(rec => rec.socketId == controlData.i);

            // If there is no player in the list with that id, then add one
            if (thisPlayer == null)
            {
                thisPlayer = UserJoin(controlData.i);
            }

            // Apply the controls to that player
            if (thisPlayer != null)
                if (controlData.type == "tilt")
                    thisPlayer.Tilt(controlData.tilt);
                else
                    thisPlayer.Touch(controlData);
        }
    }
}
