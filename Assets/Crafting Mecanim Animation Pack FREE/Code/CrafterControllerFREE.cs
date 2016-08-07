using UnityEngine;
using System.Collections;

public class CrafterControllerFREE : MonoBehaviour {

	private Animator animator;
	
	private GameObject box;

	float rotationSpeed = 5;
	Vector3 inputVec;
	bool isMoving;
	bool isPaused;

	public enum CharacterState {Idle, Box};
	
	public CharacterState charState;

	void Awake()
	{
		animator = this.GetComponent<Animator>();
		box = GameObject.Find("Carry");
	}

	void Start()
	{
		StartCoroutine (COShowItem("none", 0f));
		charState = CharacterState.Idle;
	}

	void Update()
	{
		//Get input from controls
		float z = Input.GetAxisRaw("Horizontal");
		float x = -(Input.GetAxisRaw("Vertical"));
		inputVec = new Vector3(x, 0, z);
		animator.SetFloat("VelocityX", -x);
		animator.SetFloat("VelocityY", z);

		if (x != 0 || z != 0 )  //if there is some input
		{
			//set that character is moving
			animator.SetBool("Moving", true);
			isMoving = true;

			//if we are running, set the animator
			if (Input.GetButton("Jump"))
				animator.SetBool("Running", true);
			else
				animator.SetBool("Running", false);
		}
		else
		{
			//character is not moving
			animator.SetBool("Moving", false);
			isMoving = false;
		}

		UpdateMovement();  //update character position and facing

		if(Input.GetKey(KeyCode.R))
			this.gameObject.transform.position = new Vector3(0,0,0);

		animator.SetFloat("Velocity", UpdateMovement());  //sent velocity to animator
	}

	void RotateTowardsMovementDir()  //face character along input direction
	{
		if (!isPaused)
		{
			if (inputVec != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotationSpeed);
			}
		}
	}

	float UpdateMovement()  //movement of character
	{
		Vector3 motion = inputVec;  //get movement input from controls

		//reduce input for diagonal movement
		motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1)?.7f:1;
		
		if(!isPaused)
			RotateTowardsMovementDir();  //if not paused, face character along input direction

		return inputVec.magnitude;
	}

	void OnGUI () 
	{
		if (charState == CharacterState.Idle && !isMoving)
		{
			isPaused = false;

			if (GUI.Button (new Rect (25, 25, 150, 30), "Pickup Box")) 
			{
				animator.SetTrigger("CarryPickupTrigger");
				StartCoroutine (COMovePause(1.2f));
				StartCoroutine (COShowItem("box", .5f));
				charState = CharacterState.Box;
			}

			if (GUI.Button (new Rect (25, 65, 150, 30), "Recieve Box")) 
			{
				animator.SetTrigger("CarryRecieveTrigger");
				StartCoroutine (COMovePause(1.2f));
				StartCoroutine (COShowItem("box", .5f));
				charState = CharacterState.Box;
			}
		}

		if (charState == CharacterState.Box && !isMoving)
		{
			if (GUI.Button (new Rect (25, 25, 150, 30), "Put Down Box")) 
			{
				animator.SetTrigger("CarryPutdownTrigger");
				StartCoroutine (COMovePause(1.2f));
				StartCoroutine (COShowItem("none", .7f));
				charState = CharacterState.Idle;
			}
			
			if (GUI.Button (new Rect (25, 65, 150, 30), "Give Box")) 
			{
				animator.SetTrigger("CarryHandoffTrigger");
				StartCoroutine (COMovePause(1.2f));
				StartCoroutine (COShowItem("none", .6f));
				charState = CharacterState.Idle;
			}
		}
	}

	public IEnumerator COMovePause(float pauseTime)
	{
		isPaused = true;
		yield return new WaitForSeconds(pauseTime);
		isPaused = false;
	}

	public IEnumerator COChangeCharacterState(float waitTime, CharacterState state)
	{
		yield return new WaitForSeconds(waitTime);
		charState = state;
	}
	
	public IEnumerator COShowItem(string item, float waittime)
	{
		yield return new WaitForSeconds (waittime);
		
		if(item == "none")
		{
			box.SetActive(false);
		}

		else if(item == "box")
		{
			box.SetActive(true);
		}

		yield return null;
	}
}