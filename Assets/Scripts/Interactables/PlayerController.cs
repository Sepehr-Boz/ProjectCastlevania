using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IDamageable
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

	private void FixedUpdate()
	{
		if (hp <= 0)
		{
			PlayerManager.Instance.currentData.currentHP = hp;
			PlayerManager.Instance.PlayerDeath();
		}


		rigidBody.velocity = velocity * moveForce;
	}

	private void Update()
	{
		//get all nearby colliders
		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5f);
		foreach (Collider2D hit in hits)
		{
			if (hit.GetComponent<CoinController>())
			{
				hit.attachedRigidbody.velocity += (Vector2)(transform.position - hit.transform.position) / 5f;
			}
		}
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


	#region interface methods
	public void Damage(int damage)
	{
		hp -= damage;
	}


	#endregion
}