using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private float cameraRotationLimit = 85f;

	private Vector3 velocity = Vector3.zero;
	private Vector3 rotation = Vector3.zero;
	private float cameraRotationX = 0f;
	private float currentCameraRotationX = 0f;
	private Vector3 thrusterForce = Vector3.zero;



	private Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>(); 
	}

	//Gets a movement vector
	public void Move (Vector3 _velocity)
	{
		velocity = _velocity;
	}

	//Gets a rotation vector
	public void Rotate (Vector3 _rotation)
	{
		rotation = _rotation;
	}

	//Gets a Camera rotation vector
	public void RotateCamera (float _cameraRotationX)
	{
		cameraRotationX = _cameraRotationX;
	}

	//geta force vector for our thruster
	public void ApplyThruster (Vector3 _thrusterForce)
	{
		thrusterForce = _thrusterForce;
	}



	//Run every physics iteration
	void FixedUpdate()
	{
		PerformMovement();
		PerformRotation();
	
	}

	//Perform movement based on velocity variable
	void PerformMovement()
	{
		if(velocity != Vector3.zero)
		{
			rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
		}

		if(thrusterForce != Vector3.zero)
		{
			rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
		}


	}

	//Perform movement based on velocity variable
	void PerformRotation()
	{
			rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
			if(cam != null)
			{
				//set our rotation and clamp it
				currentCameraRotationX -= cameraRotationX;
				currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

				//Apply our rotation to thetransform of our cam
				cam.transform.localEulerAngles= new Vector3(currentCameraRotationX, 0f, 0f);


			}

	}

}
