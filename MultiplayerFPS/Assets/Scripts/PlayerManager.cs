using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class PlayerManager : NetworkBehaviour {

	[SyncVar]
	private bool _isDead = false;
	public bool isDead
	{
		get{ return _isDead; }
		protected set { _isDead = value; }
	}

	[SerializeField]
	private int maxHealth = 100;

	[SyncVar]
	private int currentHealth;

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	[SerializeField]
	private GameObject deathEffect;

	[SerializeField]
	private GameObject spawnEffect;

	[SerializeField]
	private GameObject[] disableGameObjectOnDeath;

	private bool firstSetup = true;

	public void SetupPlayer()
	{	
		if (isLocalPlayer) 
		{
			//switch cameras
			GameManager.instance.SetSceneCameraActive(false);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
	
		}

		CmdNewPlayerSetup();
	}

	[Command]
	private void CmdNewPlayerSetup()
	{
		RpcSetupPlayersOnAllClients();
	}

	[ClientRpc]
	private void RpcSetupPlayersOnAllClients()
	{
		if (firstSetup) 
		{
			wasEnabled = new bool[disableOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++) 
			{
				wasEnabled[i] = disableOnDeath[i].enabled;
			}

			firstSetup = false;
		}


		SetDefaults();
	}

	void Update()
	{
		if (!isLocalPlayer) 
			return;

		if (Input.GetKeyDown(KeyCode.K)) 
		{
			RpcTakeDamage(999999);
		}
	}

	[ClientRpc]
	public void RpcTakeDamage(int _amount)
	{
		if (isDead) 
			return;
		
		currentHealth -= _amount;
		Debug.Log(transform.name + " now has " + currentHealth + " health");

		if (currentHealth <= 0) 
		{
			Die();
		}
	}

	private void Die()
	{
		isDead = true;

		//disable components onDeath
		for (int i = 0; i < disableOnDeath.Length; i++) 
		{
			disableOnDeath[i].enabled =false;
		}

		//disable GameObject onDeath
		for (int i = 0; i < disableGameObjectOnDeath.Length; i++) 
		{
			disableGameObjectOnDeath[i].SetActive(false);
		}

		//disable collider
		Collider _col = GetComponent<Collider>();
		if (_col != null) 
		{
			_col.enabled = true;
		}
		//spawn death effect and destroying it
		GameObject explosion = (GameObject) Instantiate(deathEffect,transform.position,Quaternion.identity);
		Destroy(explosion,2f);

		// switch camera if Local player dies
		if (isLocalPlayer) 
		{
			GameManager.instance.SetSceneCameraActive(true);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
		}


		Debug.Log(transform.name + " is DEAD!");

		StartCoroutine(Respawn());

	}

	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds(GameManager.instance.matchSetting.respawnTime);


		Transform _spawnPoints = NetworkManager.singleton.GetStartPosition();
		transform.position = _spawnPoints.position;
		transform.rotation = _spawnPoints.rotation;

		yield return new WaitForSeconds(0.1f);

		SetupPlayer();
		Debug.Log(transform.name + " respawned!");

	}

	public void SetDefaults()
	{
		isDead = false;
		currentHealth = maxHealth;

		//Set components active
		for (int i = 0; i < disableOnDeath.Length; i++) 
		{
			disableOnDeath[i].enabled = wasEnabled[i];
		}

		//Enable gameObjects again
		for (int i = 0; i < disableGameObjectOnDeath.Length; i++) 
		{
			disableGameObjectOnDeath[i].SetActive(true);
		}

		//enable the collider
		Collider _col = GetComponent<Collider>();
		if (_col != null) 
		{
			_col.enabled = true;
		}

		//create SpawnEffect
		GameObject respawn = (GameObject) Instantiate(spawnEffect,transform.position,Quaternion.identity);
		Destroy(respawn,2f);

	}
}
