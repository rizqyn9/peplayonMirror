using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace RizqyNetworking
{
    public class TurnManager : NetworkBehaviour
    {
        List<Player> players = new List<Player>();

        public void AddPlayer (Player _player)
        {
            players.Add(_player);
        }
    }
}

