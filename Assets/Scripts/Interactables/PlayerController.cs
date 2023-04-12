using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;

public class PlayerController : MonoBehaviour
{
	[Header("Components")]
	[Space(2)]
	public Rigidbody2D rigidBody;
	public CircleCollider2D circleCollider;
	private PlayerInputActions playerInputActions;

	[Header("Movement Variables")]
	public float moveForce;
	private Vector2 velocity;

	[Header("Health")]
	public int hp;
	public int maxHP;


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
	}

	private void OnEnable()
	{
		//set health back to max health
		hp = maxHP;
	}

	private void FixedUpdate()
	{
		if (hp <= 0)
		{
			PlayerManager.Instance.SwitchPlayer(gameObject);
		}


		rigidBody.velocity = velocity * moveForce;
	}

	//FOR TESTING
	public void DecreaseHealth(InputAction.CallbackContext context)
	{
		hp -= 10;
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
}