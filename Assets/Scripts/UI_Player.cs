using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RizqyNetworking
{
    public class UI_Player : MonoBehaviour
    {
        [SerializeField] Text text;
        Player player;

        public void SetPlayer(Player player)
        {
            this.player = player;
            text.text = "Player" + player.playerIndex.ToString();
        }

    }

}

