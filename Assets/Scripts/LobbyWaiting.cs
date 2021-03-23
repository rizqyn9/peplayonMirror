using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RizqyNetworking
{
    public class LobbyWaiting : MonoBehaviour
    {
        // Start is called before the first frame update

        void Start()
        {
            Debug.Log("Welcome to Lobby");
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("escape");
            }
        }
    }

}
