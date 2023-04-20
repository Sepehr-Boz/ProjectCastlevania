using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
	[Header("Components")]
	[Space(2)]
	public Rigidbody2D rigidBody;
	public CircleCollider2D circleCollider;
	private PlayerInputActions playerInputActions;

	public bool isCurrent = false;

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
		circleCollider.enabled = false;
		Invoke(nameof(EnableCollider), 2f);
		
		//add inputs

		if (isCurrent)
		{
			playerInputActions = new PlayerInputActions();

			playerInputActions.Enable();
			playerInputActions.Player.Move.performed += Move;
			playerInputActions.Player.Move.canceled += MoveStop;
			playerInputActions.Player.Die.performed += DecreaseHealth;
		}
	}

	private void EnableCollider()
	{
		circleCollider.enabled = true;
	}

	private void FixedUpdate()
	{
		if (hp <= 0)
		{
			//update playerinfo
			//PlayerManager.Instance.currentData.currentScene = scene; //if dying then scene would stay the same so doesnt need to be updated

			PlayerManager.Instance.currentData.currentPos = (Vector2)transform.position;
			PlayerManager.Instance.currentData.currentHP = hp;


			//PlayerManager.Instance.SwitchPlayer();
		}


		rigidBody.velocity = velocity * moveForce;
	}

	#region movements

	//FOR TESTING
	public void DecreaseHealth(InputAction.CallbackContext context)
	{
		hp -= 10;
	}

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