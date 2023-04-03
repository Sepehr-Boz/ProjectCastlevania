using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
	[Header("Components")]
	[Space(2)]
	public Rigidbody2D rigidBody;
	public CircleCollider2D circleCollider;

	//private PlayerInput playerInput;
	private PlayerInputActions playerInputActions;

	[Header("Movement Variables")]
	public float moveForce;
	private Vector2 velocity;

	public int health = 20;
	public int maxHealth = 20;

	//public Area area;


	public void Start()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		circleCollider = GetComponent<CircleCollider2D>();

		//add inputs
		playerInputActions = new PlayerInputActions();

		playerInputActions.Enable();
		playerInputActions.Player.Move.performed += Move;
		playerInputActions.Player.Move.canceled += MoveStop;
		playerInputActions.Player.Die.performed += DecreaseHealth;


		//load the scene the player is in


		//PlayerManager.Instance.SetCamera(transform);
	}

	private void OnEnable()
	{
		//set health back to max health
		health = maxHealth;


	}

	//private void OnDisable()
	//{
	//	//playerInputActions.Disable();
	//	playerInputActions.Player.Move.performed -= Move;
	//	playerInputActions.Player.Move.canceled -= MoveStop;
	//	playerInputActions.Player.Die.performed -= DecreaseHealth;
	//}

	//private void OnEnable()
	//{
	//	//set health to max health
	//	health = maxHealth;

	//	//add checking to see if a player is in a new area
	//	//find the closest 10s to the player and get the spawn point
	//	Vector2 centre = GameManager.Instance.GetClosestCentre(transform.position);
	//	Collider2D[] colliders = Physics2D.OverlapCircleAll(centre, 1f);
	//	GameObject room = null;
	//	foreach (Collider2D collider in colliders)
	//	{
	//		if (collider.name == "FOCUS")
	//		{
	//			room = collider.transform.parent.gameObject;
	//		}
	//	}

	//	if (room == null)
	//	{
	//		GameManager.Instance.DestroyArea(PlayerManager.Instance.roomsIndex);
	//		GameManager.Instance.GenerateArea();
	//		return;
	//	}

	//	Area currentArea = room.GetComponent<AddRoom>().area;
	//	if (currentArea != area && (currentArea == Area.RED || currentArea == Area.BLUE || currentArea == Area.GREEN || currentArea == Area.YELLOW))
	//	{
	//		return;
	//	}

	//	GameManager.Instance.DestroyArea(PlayerManager.Instance.roomsIndex);
	//	GameManager.Instance.GenerateArea();

	//	//Collider2D[] collider = Physics2D.OverlapPoint(centre);
	//	//if (collider == null)
	//	//{
	//	//	GameManager.Instance.DestroyArea(PlayerManager.Instance.roomsIndex);
	//	//	GameManager.Instance.GenerateArea();
	//	//	return;
	//	//}
	//	//Collider2D collider = Physics2D.OverlapCircle(transform.position, 11f);
	//	//GameObject room = null;
	//	//if (collider.gameObject.GetComponent<Destroyer>() != null)
	//	//{
	//	//	room = collider.transform.parent.parent.gameObject;
	//	//}
	//	//foreach (Collider collider in colliders)
	//	//{
	//	//	if (collider.gameObject.GetComponent<Destroyer>() != null)
	//	//	{
	//	//		spawnPoint = collider.gameObject;
	//	//	}
	//	//}

	//	//get the room from the spawn point if the spawn point has a destroyer ( to check that its the centre of a room )
	//	//if (room == null)
	//	//{
	//	//	GameManager.Instance.DestroyArea(PlayerManager.Instance.roomsIndex);
	//	//	GameManager.Instance.GenerateArea();
	//	//	return;
	//	//}

	//	////compare the room colour with the player colour
	//	//if (room.GetComponent<AddRoom>().area != area)
	//	//{
	//	//	return;
	//	//}
	//	//if the colours are the same then destroy and generate a new set of rooms
	//	//otherwise dont

	//	//whenever a new player is active get rid of their rooms and generate a new set of rooms
	//	//GameManager.Instance.DestroyArea(PlayerManager.Instance.roomsIndex);
	//	//GameManager.Instance.GenerateArea();
	//}

	//#region enable/disable inputs
	////private void OnEnable()
	////{
	////	playerInputActions.Enable();
	////	playerInputActions.Player.Move.performed += Move;
	////	playerInputActions.Player.Move.canceled += MoveStop;

	////	playerInputActions.Player.Die.performed += DecreaseHealth;
	////}

	////private void OnDisable()
	////{
	////	playerInputActions.Disable();
	////	playerInputActions.Player.Move.performed -= Move;
	////	playerInputActions.Player.Move.canceled -= MoveStop;

	////	playerInputActions.Player.Die.performed -= DecreaseHealth;
	////}
	//#endregion

	private void FixedUpdate()
	{
		//if (health <= 0)
		//{
		//	PlayerManager.Instance.SwitchPlayer();
		//}
		rigidBody.velocity = velocity * moveForce;
	}

	public void DecreaseHealth(InputAction.CallbackContext context)
	{
		health -= 10;
	}

	#region movements
	//move functions alter the velocity int the script - dont apply the velocity in the functions themselves
	public void Move(InputAction.CallbackContext context)
	{
		velocity = context.ReadValue<Vector2>();
	}

	public void MoveStop(InputAction.CallbackContext context)
	{
		velocity = Vector2.zero;
	}
	#endregion

	//public void NextPlayer()
	//{
	//	PlayerManager.Instance.NextCharacter();
	//}
}
