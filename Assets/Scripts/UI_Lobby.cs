using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RizqyNetworking {
	public class UI_Lobby : MonoBehaviour
	{
		public static UI_Lobby instance;

		[Header("Host Join")]
		[SerializeField] List<Selectable> lobbySelectables = new List<Selectable>();
		[SerializeField] InputField joinMatchInput;
		[SerializeField] Canvas lobbyCanvas;
		[SerializeField] Canvas searchCanvas;
		[SerializeField] GameObject connectUI;

		[Header("Lobby")]
		[SerializeField] Transform UI_PlayerParent;
		[SerializeField] GameObject UI_PlayerPrefab;
		[SerializeField] Text matchIDText;
		[SerializeField] GameObject beginGameButton;

		[Header("LobbyCharacter")]
		[SerializeField] GameObject CharPrefabs;
		[SerializeField] GameObject HideShowLobbyButton;

		GameObject playerLobbyUI;

		bool searching = false;

		void Start() {
			instance = this;
		}

        private void Update()
        {
// Show Players Lobby
            if (Input.GetKeyDown(KeyCode.Escape))
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

        #region HOST
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


		public void HostSuccess(bool success, string _matchID) {
			if (success) {
				lobbyCanvas.enabled = true;
				connectUI.SetActive(false);
				playerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
				matchIDText.text = _matchID;
				beginGameButton.SetActive(true);
				//HideShowLobbyButton.SetActive(true);

                Player.localPlayer.LobbyWaiting();


			}
			else {
				joinMatchInput.interactable = true;
				lobbySelectables.ForEach(x => x.interactable = true);
			}
		}

        #endregion

        #region JOIN
        public void Join() {
			joinMatchInput.interactable = false;
			lobbySelectables.ForEach(x => x.interactable = false);


			Player.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
		}

		public void JoinSuccess(bool success, string _matchID) {
			if (success) {
				lobbyCanvas.enabled = true;
				beginGameButton.SetActive(false);
				connectUI.SetActive(false);
				playerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
				matchIDText.text = _matchID;
                //HideShowLobbyButton.SetActive(true);

                Player.localPlayer.LobbyWaiting();

			}
			else {
				joinMatchInput.interactable = true;
				lobbySelectables.ForEach(x => x.interactable = true);

			}
		}

        #endregion

        #region SHOW_HIDE_Lobby

		public void ShowHideLobby ()
        {
			if(lobbyCanvas.enabled)
            {
				lobbyCanvas.enabled = false;
				beginGameButton.GetComponent<Text>().text = "Show Lobby";
            } else
            {
				lobbyCanvas.enabled = false;
				beginGameButton.GetComponent<Text>().text = "Hide Lobby";


			}
		}

        #endregion

        public GameObject SpawnPlayerUIPrefab(Player player)
		{
			GameObject newUIPlayer = Instantiate(UI_PlayerPrefab, UI_PlayerParent, CharPrefabs);
			newUIPlayer.GetComponent<UI_Player>().SetPlayer(player);
			newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
			return newUIPlayer;
		}

		public void BeginGame()
		{
			Debug.Log("begin");
			Player.localPlayer.BeginGame();
		}

        #region SEARCH_GAME

        public void SearchGame()
		{
			Debug.Log($"Searching");
			searchCanvas.enabled = true;
			StartCoroutine(SearchingForGame());
		}

		IEnumerator SearchingForGame()
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

		public void SearchCancel()
		{
			Debug.Log("Cancel");
			searching = false;
		}

        #endregion


        public void DisconnectedLobby()
		{
			if (playerLobbyUI != null) Destroy(playerLobbyUI);
			Player.localPlayer.DisconnectGame();

			lobbyCanvas.enabled = false;
			lobbySelectables.ForEach(x => x.interactable = true);
			beginGameButton.SetActive(false);
		}

	}
}