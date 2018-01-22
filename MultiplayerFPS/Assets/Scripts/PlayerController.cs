using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed = 5f;

	[SerializeField]
	private float lookSens = 2f;

	[SerializeField]
	private float thrusterForce = 1750f;

	[SerializeField]
	private float thrusterFuelBurnSpeed = 1f;
	[SerializeField]
	private float thrusterFuelRegenSpeed = 0.3f;
	private float thrusterFuelAmount = 1f;
	[SerializeField]
	private LayerMask environmentMask;

	public float GetThrusterFuelAmount()
	{
		return thrusterFuelAmount;
	}

	[Header("Joint options:")]
	[SerializeField]
	private JointDriveMode mode = JointDriveMode.Position;
	[SerializeField]
	private float jointSpring = 20f;
	[SerializeField]
	private float jointMaxForce = 40f;

	//Component caching
	private Animator animator;
	private PlayerMotor motor;
	private ConfigurableJoint joint;

	void Start()
	{
		motor = GetComponent<PlayerMotor>();
		joint= GetComponent<ConfigurableJoint>();
		animator = GetComponent<Animator>();

		SetJointSettings(jointSpring);
	}

	void Update()
	{
		// Handling player collision with cubes
		//after we jump on them
		// we are checking height every frame using rayCast
		// and setting its position over objects
		RaycastHit _hit;
		if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, environmentMask))
		{
			joint.targetPosition = new Vector3 (0f, -_hit.point.y, 0f);
		}else
		{
			joint.targetPosition = new Vector3 (0f, 0f, 0f);
		}


		//Calculate movement velocity as a 3D vector 
		float xMove = Input.GetAxis ("Horizontal");
		float zMove = Input.GetAxis ("Vertical");

		Vector3 moveHorizontal = transform.right * xMove;
		Vector3 moveVertical = transform.forward * zMove;

		//final movement Vector
		Vector3 _velocity = (moveHorizontal + moveVertical) * speed;

		//Animate movement
		animator.SetFloat("ForwardVelocity", zMove);

		//apply Movement
		motor.Move(_velocity);

		//calculate rotation as a 3D Vector (turning around)
		float yRot = Input.GetAxis("Mouse X");

		Vector3 _rotation = new Vector3 (0f, yRot, 0f) * lookSens;

		//apply rotation
		motor.Rotate(_rotation);


		//calculate rotation as a 3D Vector (turning around)
		float xRot = Input.GetAxisRaw("Mouse Y");

		float _cameraRotationX = xRot * lookSens;

		//apply camera rotation
		motor.RotateCamera(_cameraRotationX);

		Vector3 _thrusterForce = Vector3.zero;

		//Calculate the thrusterForce based on player input
		if(Input.GetButton("Jump") && thrusterFuelAmount > 0)
		{	
			thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

			if (thrusterFuelAmount >= 0.01f) 
			{
				_thrusterForce = Vector3.up * thrusterForce;
				SetJointSettings(0f);
			}
		
		}else
		{
			thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
			SetJointSettings(jointSpring);
		}

		thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

		//Apply the thruster force
		motor.ApplyThruster(_thrusterForce);


	}

	private void SetJointSettings(float _jointSpring)
	{
		joint.yDrive = new JointDrive
		{
			maximumForce = jointMaxForce,
			positionSpring = jointSpring
			
		};
	}

}
 