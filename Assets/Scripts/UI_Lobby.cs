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

        public void HostPublic()
		{
			lobbySelectables.ForEach(x => x.interactable = false);

			Player.localPlayer.HostGame(true);
		}

		public void HostPrivate()
		{
			lobbySelectables.ForEach(x => x.interactable = false);

			Player.localPlayer.HostGame(false);
		}


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

		public void Join() {
			joinMatchInput.interactable = false;
			lobbySelectables.ForEach(x => x.interactable = false);


			Player.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
		}
     
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

		public GameObject SpawnPlayerUIPrefab(Player player)
        {
			GameObject newUIPlayer = Instantiate(UI_PlayerPrefab, UI_PlayerParent);
			newUIPlayer.GetComponent<UI_Player>().SetPlayer(player);
			newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
			return newUIPlayer;
        }

		public void BeginGame()
        {
			Player.localPlayer.BeginGame();
        }

		public void SearchGame ()
        {
			Debug.Log($"Searching");
			searchCanvas.enabled = true;
			StartCoroutine(SearchingForGame());
        }

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

		public void SearchCancel ()
        {
			Debug.Log("Cancel");
			searching = false;
        }

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