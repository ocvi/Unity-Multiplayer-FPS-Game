﻿using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	
	public MatchSettings matchSetting;

	[SerializeField]
	private GameObject sceneCamera;

	void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one GameManager in scene!");
		}else
		{
			instance = this;
		}

	}

	public void SetSceneCameraActive(bool isActive)
	{
		if (sceneCamera == null) 
			return;

		sceneCamera.SetActive(isActive);
	}

	#region Player tracking
	private const string PLAYER_ID_PREFIX = "Player";

	private static Dictionary<string, PlayerManager> players =new Dictionary<string,PlayerManager>();

	public static void RegisterPlayer(string _netID, PlayerManager _player)
	{
		string _playerID = PLAYER_ID_PREFIX + _netID;
		players.Add(_playerID, _player);
		_player.transform.name = _playerID;
	}

	public static void UnRegisterPlayer(string _playerID)
	{
		players.Remove(_playerID);
	}

	public static PlayerManager GetPlayer (string _playerID)
	{
		return players[_playerID];
	}

	#endregion
}
