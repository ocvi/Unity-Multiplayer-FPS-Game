using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayerLayer";

	[SerializeField]
	string dontDrawLayerName = "DontDraw";

	[SerializeField]
	GameObject playerGraphics;

	[SerializeField]
	GameObject playerUIPrefab;

	[HideInInspector]
	public GameObject playerUIInstance;


	void Start()
	{	
		//check if we are controlling a player if not disable components 
		if(!isLocalPlayer)
		{
			DisableComponents();
			AssignRemotePlayer();
		}
		else
		{
			//disable player graphics for local player, removed graphics for FPS collision
			SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

			//create player UI
			playerUIInstance = Instantiate(playerUIPrefab);
			playerUIInstance.name = playerUIPrefab.name;

			//configure player UI
			PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
			if (ui == null) 
			{
				Debug.LogError("No PlayerUI component on PlayerUI prefab.");
			}
			ui.SetPlayerController(GetComponent<PlayerController>());
			GetComponent<PlayerManager>().SetupPlayer();
		}



	}

	//recursive methods call them selves
	void SetLayerRecursively(GameObject obj, int newLayer)
	{
		obj.layer = newLayer;
		foreach (Transform child in obj.transform) 
		{
			SetLayerRecursively(child.gameObject,newLayer);
		}
	}

	public override void OnStartClient ()
	{
		base.OnStartClient ();

		string _netID = GetComponent<NetworkIdentity>().netId.ToString();
		PlayerManager _player = GetComponent<PlayerManager>();

		GameManager.RegisterPlayer(_netID, _player);
	}

	void AssignRemotePlayer()
	{
		gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
	}

	void DisableComponents()
	{
		for (int i = 0; i < componentsToDisable.Length; i++)
			{
				componentsToDisable[i].enabled = false;
			}
	}
	//when we are dead/destroyed
	void onDisable()
	{
		Destroy(playerUIInstance);

		if (isLocalPlayer) 
			GameManager.instance.SetSceneCameraActive(true);

		GameManager.UnRegisterPlayer(transform.name);
	}
}
