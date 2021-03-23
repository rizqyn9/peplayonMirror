using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RizqyNetworking {
	public class Player : NetworkBehaviour {

		[Header("playerCharacter")]
		[SerializeField] GameObject Character;
		

		[Header("SyncVar")]
		public static Player localPlayer;
		[SyncVar] public string matchID;
		[SyncVar] public int playerIndex;

		NetworkMatchChecker networkMatchChecker;

		[SyncVar] public Match currentMatch;
		[SerializeField] GameObject playerLobbyUI;



		void Awake() {
			networkMatchChecker = GetComponent<NetworkMatchChecker>();

		}

		public override void OnStartClient()
		{
			if (isLocalPlayer) {
				localPlayer = this;
			} else
            {
				Debug.Log($"Spawming other player");
				playerLobbyUI = UI_Lobby.instance.SpawnPlayerUIPrefab(this);
            }
			
		}

		public override void OnStopClient()
		{
			Debug.Log($"Player Disconnected");
			ClientDisconnect();	
		}

		public override void OnStopServer()
		{
			Debug.Log($"Client Stopped from server ");
			ServerDisconnect(); 
		}


		// ! HOSTED MATCH
		#region HOSTED MATCH

		public void HostGame(bool publicMatch) {
			string matchID = MatchMaker.GetRandomMatchID();
			CmdHostGame(matchID, publicMatch);
		}

		[Command]
		void CmdHostGame(string _matchID, bool publicMatch) {
			matchID = _matchID;
			if (MatchMaker.instance.HostGame(_matchID, gameObject, out playerIndex, publicMatch)) {
				Debug.Log($"<color=green>Game hosted successfuly</color>");
				networkMatchChecker.matchId = _matchID.ToGuid();
				TargetHostGame(true, _matchID, playerIndex);
			} else {
				Debug.Log($"<color=red>Game hosted failed</color>");
				TargetHostGame(false, _matchID, playerIndex);
			}
		}

		[TargetRpc]
		void TargetHostGame(bool success, string _matchID, int _playerIndex) {
			playerIndex = _playerIndex;
			Debug.Log($"Match ID : {matchID} == {_matchID}");
			UI_Lobby.instance.HostSuccess(success,_matchID);
		}

#endregion

        // ! JOIN MATCH
#region JOIN MATCH
        public void JoinGame(string _inputID) {
            CmdJoinGame(_inputID);
		}

		[Command]
		void CmdJoinGame(string _matchID) {
			matchID = _matchID;
			if (MatchMaker.instance.JoinGame(_matchID, gameObject, out playerIndex)) {
				Debug.Log($"<color=green>Game Joined successfully</color>");
				networkMatchChecker.matchId = _matchID.ToGuid();
				TargetJoinGame(true, _matchID, playerIndex);
			} else {
				Debug.Log($"<color=red>Game Joined failed</color>");
				TargetJoinGame(false, _matchID,playerIndex);
			}
		}

		[TargetRpc]
		void TargetJoinGame(bool success, string _matchID, int _playerIndex) {
			playerIndex = _playerIndex;
			matchID = _matchID;
			Debug.Log($"MatchID: {matchID} == {_matchID}");
			UI_Lobby.instance.JoinSuccess(success,_matchID);
		}

        #endregion

        #region SEARCH MATCH

		public void SearchGame ()
        {
			CmdSearchGame();
        }

		[Command]
		public void CmdSearchGame()
		{
			if (MatchMaker.instance.SearchGame(gameObject, out playerIndex, out matchID))
			{
				Debug.Log($"<color=green>Game Found</color>");
				networkMatchChecker.matchId = matchID.ToGuid();
				TargetSearchGame(true, matchID, playerIndex);
			}
			else
			{
				Debug.Log($"<color=red>Game Not Found!!</color>");
				TargetSearchGame(false, matchID, playerIndex);
			}
		}

		[TargetRpc]
		public void TargetSearchGame(bool success, string _matchID, int _playerIndex)
		{
			playerIndex = _playerIndex;
			matchID = _matchID;
			Debug.Log($"MatchID: {matchID} == {_matchID}");
			UI_Lobby.instance.SearchSuccess(success, _matchID);
		}

        #endregion

        #region Lobby Waiting
		public void LobbyWaiting()
        {
			SpawnCharacter();
			TargetLobbyWaiting();
        }

	
		void TargetLobbyWaiting()
        {
			SceneManager.LoadScene(2, LoadSceneMode.Additive);
        }

        #endregion

        // ! BEGIN MATCH
        #region BEGIN MATCH

        public void BeginGame()
		{
			CmdBeginGame();
		}

		[Command]
		void CmdBeginGame()
		{
			MatchMaker.instance.BeginGame(matchID);
			Debug.Log($"<color=green>Game Beginning</color>");

		}

		public void StartGame()
        {
			TargetBeginGame();

		}

		[TargetRpc]
		void TargetBeginGame()
		{
			Debug.Log($"Match ID : {matchID} | Begining ");
			//! Load Game scene here
			SceneManager.LoadScene(3, LoadSceneMode.Single);
		}

        #endregion

        #region  MATCH DISCONNECTED

		public void DisconnectGame()
        {
			CmdDisconnectGame();
        }


		[Command]
		void CmdDisconnectGame ()
        {
			ServerDisconnect();
        }

		void ServerDisconnect()
        {
			MatchMaker.instance.PlayerDisconnected(this, matchID);
			RpcDisconnectGame();
			networkMatchChecker.matchId = string.Empty.ToGuid();

        }

		[ClientRpc]
		void RpcDisconnectGame ()
        {
			ClientDisconnect();
        }

		void ClientDisconnect()
        {
			if(playerLobbyUI != null)
			{ 
				Destroy(playerLobbyUI);
            }

        }

        #endregion

        #region
		public void SpawnCharacter ()
        {
			Camera.main.enabled = false;
			GameObject.Instantiate(Character,transform);
        }

		public void CameraPlayer()
        {
			Character.gameObject.SetActive(false);
        }

        #endregion
    }
}
