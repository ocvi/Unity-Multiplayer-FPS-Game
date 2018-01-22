using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

	[SerializeField]
	RectTransform thrusterFuelAmount;

	private PlayerController controller;

	public void SetPlayerController(PlayerController _controller)
	{
		controller = _controller;
	}

	void Update()
	{
		SetFuelAmount(controller.GetThrusterFuelAmount());
	}

	void SetFuelAmount (float _amount)
	{
		thrusterFuelAmount.localScale = new Vector3(1f, _amount, 1f);
	}
}
