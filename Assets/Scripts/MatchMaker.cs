using System;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using UnityEngine;

namespace RizqyNetworking{
		
	[System.Serializable]
	public class Match {
		public string matchID;

		public bool publicMatch;

		public bool inMatch;

		public bool matchFull;

		public SyncListGameObject players = new SyncListGameObject();

		public Match(string matchID, GameObject player, bool publicMatch) {
			matchFull = false;
			inMatch = false;
			this.matchID = matchID;
			this.publicMatch = publicMatch;
			players.Add(player);
		}
		public Match() { }
	}


	[System.Serializable]
	public class SyncListGameObject : SyncList<GameObject> { }

	[System.Serializable]
	public class SyncListMatch : SyncList<Match> { }

	public class MatchMaker : NetworkBehaviour {

		public static MatchMaker instance;

		public SyncListMatch matches = new SyncListMatch();
		public SyncListString matchIDs = new SyncListString ();

		[SerializeField] GameObject turnManagerPrefab;
		[SerializeField] int MaxPlayerInRoom = 40;


		void Start() {
			instance = this;	
		}


// ! HOSTED MATCH
		public bool HostGame (string _matchID, GameObject _player, out int playerIndex, bool publicMatch) {
			playerIndex = -1;

			if (!matchIDs.Contains (_matchID)) {
				matchIDs.Add (_matchID);
				Match match = new Match(_matchID, _player, publicMatch);
				match.publicMatch = publicMatch;
				matches.Add(match);
				Debug.Log ($"Match generated");
				_player.GetComponent<Player>().currentMatch = match;
				playerIndex = 1; 
				return true;
			} else {
				Debug.Log ($"Match ID already exists");
				return false;
			}
		}

// ! JOINED MATCH
		public bool JoinGame (string _matchID, GameObject _player, out int playerIndex) {
			playerIndex = -1;
			if (matchIDs.Contains (_matchID)) {
				for (int i = 0; i < matches.Count ; i++) {
					if(matches[i].matchID == _matchID){
						matches[i].players.Add(_player);
						_player.GetComponent<Player>().currentMatch = matches[i];
						playerIndex = matches[i].players.Count;

// Checking Room is Full?
                        if (matches[i].players.Count == MaxPlayerInRoom)
						{
							matches[i].matchFull = true;
							Debug.Log($"Room is Full");
						}

						break;
					}
				}
				Debug.Log ($"Match Joined");
				return true;
			} else {
				Debug.Log ($"Match ID does't exists");
				return false;
			}
		}

// Button Search Game
		public bool SearchGame(GameObject _player, out int playerIndex, out string matchID )
        {
			playerIndex = -1;
			matchID = "";

			for(int i = 0; i <  matches.Count; i++)
            {
				if(matches[i].publicMatch && !matches[i].inMatch)
                {
					matchID = matches[i].matchID;
					if(JoinGame(matchID, _player, out playerIndex))
                    {
						return true;
                    }
                }
            }

			return false;
        }

// Button Begin Game
		public void BeginGame (string _matchID)
        {
			GameObject newTurnManager = Instantiate(turnManagerPrefab);
			NetworkServer.Spawn(newTurnManager);
			newTurnManager.GetComponent<NetworkMatchChecker>().matchId = _matchID.ToGuid();
			TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();

			for (int i = 0; i < matches.Count; i++)
            {
				if(matches[i].matchID == _matchID)
                {
					matches[i].inMatch = true;
					foreach(var player in matches[i].players)
                    {
						Player _player = player.GetComponent<Player>();
						_player.StartGame();
                    }
					break;
                }
            }
        }

//Generated room code
		public static string GetRandomMatchID() {
			string _id = string.Empty;
			for(int i = 0 ; i < 5 ; i++) {
				int random = UnityEngine.Random.Range(0,36);
				if(random < 26) {
					_id += (char)(random + 65);
				} else {
					_id += (random - 26).ToString();
				}
			}
			Debug.Log($"Random Match ID: {_id}");
			return _id;
		}

// Clear and terminate room if (player.count == 0)
        public void PlayerDisconnected(Player player, string _matchID)
        {
			for(int i = 0; i < matches.Count; i++)
            {
				if(matches[i].matchID == _matchID)
                {
					int playerIndex = matches[i].players.IndexOf(player.gameObject);
					matches[i].players.RemoveAt(playerIndex);
					Debug.Log($"Player disconnected from match {_matchID} | {matches[i].players.Count} players remaining");

					if(matches[i].players.Count == 0)
                    {
						Debug.Log($"No Players, Terminating {_matchID}");
						matches.RemoveAt(i);
						matchIDs.Remove(_matchID);
                    }

					break;
                }
            }

        }
	}

// Hash room code
	public static class MatchExtensions {
		public static Guid ToGuid (this string id) {
			MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
			byte[] inputBytes = Encoding.Default.GetBytes(id);
			byte[] hashBytes = provider.ComputeHash (inputBytes);

			return new Guid (hashBytes);
		}
	}

}
