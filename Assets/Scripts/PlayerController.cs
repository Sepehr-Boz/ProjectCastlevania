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
	public int health = 20;
	public int maxHealth = 20;


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
		health = maxHealth;

		//this.Invoke(() => Debug.Log("Lambdas also work"), 1f);
		//this.Invoke(() => transform.position *= 1.1f, 3f); //move after a second to trigger FOCUS and move camera to correct position

		//this.Invoke(() => transform.position += new Vector2(0.1f, 0.1f), 1f);
	}

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
}


//public static class Utility
//{
//	public static void Invoke(this MonoBehaviour mb, Action f, float delay)
//	{
//		mb.StartCoroutine(InvokeRoutine(f, delay));
//	}

//	private static IEnumerator InvokeRoutine(System.Action f, float delay)
//	{
//		yield return new WaitForSeconds(delay);
//		f();
//	}
//}