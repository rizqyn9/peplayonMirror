using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RizqyNetworking
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] GameObject camera1;

        public void CameraPlayer(bool _active)
        {
            camera1.SetActive(_active);
        }

    }
}

