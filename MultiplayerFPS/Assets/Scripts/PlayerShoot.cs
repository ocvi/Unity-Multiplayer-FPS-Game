using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerManager))]
public class PlayerShoot : NetworkBehaviour {

	private const string PLAYER_TAG = "Player";

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	private WeaponManager weaponManager;
	private PlayerWeapon currentWeapon;

	void Start()
	{
		if (cam == null)	
		{
			Debug.LogError("PlayerShoot : No camera referenced!");
			this.enabled = false;
		}

		weaponManager = GetComponent<WeaponManager>();
	}

	void Update()
	{
		currentWeapon = weaponManager.GetCurrentWeapon();

		if (currentWeapon.fireRate <= 0f) {
			if(Input.GetButtonDown("Fire1"))
			{
				Shoot();
			}
		}else
		{
			if (Input.GetButtonDown("Fire1")) 
			{
				InvokeRepeating("Shoot", 0f, 0.1f/currentWeapon.fireRate); 
			}else if (Input.GetButtonUp("Fire1")) 
			{
				CancelInvoke("Shoot");
			}
		}


	}

	//Is called on theserver when a player shoots
	[Command]
	void CmdOnShoot()
	{
		RpcDoShootEffect();
	}

	//Is called on the server when wehit something
	// takes in the hit point and the normal of the surface
	[Command]
	void CmdOnHit (Vector3 _pos, Vector3 _normal)
	{
		RpcDoHitEffect(_pos, _normal);
	}

	//Is called on all clients when we need to do a shoot effect
	[ClientRpc]
	void RpcDoShootEffect()
	{
		weaponManager.GetCurrentGraphics().muzzleFlash.Play();
	}

	//Is called on all clients, we spawn in the cool effects
	[ClientRpc]
	void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
	{
		GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
		Destroy(_hitEffect,2f);
	}

	[Client]
	void Shoot()
	{

		Debug.Log("SHOOT!");
		if (!isLocalPlayer) 
		{
			return;
		}

		//We are shooting, call the OnShoot method on the server
		CmdOnShoot();
		
		RaycastHit _hit;
		if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
		{
			if(_hit.collider.tag == PLAYER_TAG)
			{
				CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
			}

			//we hit sth, call the OnHit method on server
			CmdOnHit(_hit.point, _hit.normal);
		}
	}

	[Command]
	void CmdPlayerShot(string _playerID, int _damage)
	{
		Debug.Log(_playerID + " has been shot.");

		PlayerManager player = GameManager.GetPlayer(_playerID);
		player.RpcTakeDamage(_damage);
	}
}
