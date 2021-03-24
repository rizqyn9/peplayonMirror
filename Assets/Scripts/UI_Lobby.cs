using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RizqyNetworking {
	public class UI_Lobby : MonoBehaviour
	{   
		public static UI_Lobby instance;

		[Header("Host Join")]
		[SerializeField] List<Selectable> lobbySelectables = new List<Selectable>();
		[SerializeField] InputField	joinMatchInput;
		[SerializeField] Canvas		lobbyCanvas;
		[SerializeField] Canvas		searchCanvas;
		[SerializeField] Canvas		connectCanvas;


		[Header("Lobby")]
		[SerializeField] Transform	UI_PlayerParent;
		[SerializeField] GameObject UI_PlayerPrefab;
		[SerializeField] Text		matchIDText;
		[SerializeField] GameObject	beginGameButton;
		GameObject playerLobbyUI;


		bool searching = false;

		void Start() {
			instance = this ;
		}


        void Update()
        {
			// Key for show/hidden billboard players room
            if (Input.GetKeyDown(KeyCode.Escape) && Player.localPlayer.matchID != null )
            {
                if (lobbyCanvas.enabled)
                {
					lobbyCanvas.enabled = false;

                } else
                {
					lobbyCanvas.enabled = true;

                }
            }  
        }

		// Button Host Public
        public void HostPublic()
		{
			lobbySelectables.ForEach(x => x.interactable = false);

			Player.localPlayer.HostGame(true);
		}

		//Button Host Private
		public void HostPrivate()
		{
			lobbySelectables.ForEach(x => x.interactable = false);

			Player.localPlayer.HostGame(false);
		}

		// Host Checking Created code
		public void HostSuccess (bool success, string _matchID) {
			if(success) {
				lobbyCanvas.enabled = true;

				playerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
				matchIDText.text = _matchID;
				beginGameButton.SetActive(true);

				connectCanvas.enabled = false;
				Player.localPlayer.WaitingScene();

			}
			else {
				joinMatchInput.interactable = true;
				lobbySelectables.ForEach(x => x.interactable = true);
			}
		}

		// Button Join
		public void Join() {
			joinMatchInput.interactable = false;
			lobbySelectables.ForEach(x => x.interactable = false);


			Player.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
		}

		// Checkking client success Join ?
        public void JoinSuccess (bool success,string _matchID) {
			if(success) {
				lobbyCanvas.enabled = true;
				beginGameButton.SetActive(false);

				playerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
				matchIDText.text = _matchID;

				connectCanvas.enabled = false;
				Player.localPlayer.WaitingScene();

			} else {
				joinMatchInput.interactable = true;
				lobbySelectables.ForEach(x => x.interactable = true);

			}
		}

		// Spawn Player UI to billboard
		public GameObject SpawnPlayerUIPrefab(Player player)
        {
			GameObject newUIPlayer = Instantiate(UI_PlayerPrefab, UI_PlayerParent);
			newUIPlayer.GetComponent<UI_Player>().SetPlayer(player);
			newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
			return newUIPlayer;
        }

		// Button Start game in Hosted Player
		public void BeginGame()
        {
			Player.localPlayer.BeginGame();
        }

		// Button Search Public Room
		public void SearchGame ()
        {
			Debug.Log($"Searching");
			searchCanvas.enabled = true;
			StartCoroutine(SearchingForGame());
        }

		// Time Intreval
		IEnumerator SearchingForGame ()
        {
			searchCanvas.enabled = true;
			searching = true;

			float searchInterval = 1;
			float currentTime = 1;

			while (searching)
			{
				if (currentTime > 0)
				{
					currentTime -= Time.deltaTime;
				}
				else
				{
					currentTime = searchInterval;
					Player.localPlayer.SearchGame();
				}
				yield return null;
			}
			searchCanvas.enabled = false;

		}

		// Checking Success searching ?
		public void SearchSuccess(bool success, string matchID)
		{
            if (success)
            {
				Debug.Log("Match Found");
				searchCanvas.enabled = false;
				JoinSuccess(success, matchID);
				searching = false;
            }
		}

		// Buttn canceliing search
		public void SearchCancel ()
        {
			Debug.Log("Cancel");
			searching = false;
        }

// Out from Room
        public void DisconnectedLobby ()
        {
			if (playerLobbyUI != null) Destroy(playerLobbyUI);
			Player.localPlayer.DisconnectGame();

			connectCanvas.enabled = true;
			lobbyCanvas.enabled = false;
			lobbySelectables.ForEach(x => x.interactable = true);
			beginGameButton.SetActive(false);
		}
	}
}