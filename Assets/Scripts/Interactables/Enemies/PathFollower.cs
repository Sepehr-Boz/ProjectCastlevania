using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathFollower : MonoBehaviour
{

	[Header("Paths")]
	[Space(2)]
	[Range(0.01f, 1f)]
	public float moveSpeed; //need to change movespeed based on the number of points
	/// <summary>
	/// e.g. with 4 points travelling all of them will take 4 seconds at a moveSpeed of 1
	/// but with 90 points, travelling all of them will take 90 seconds at a moveSpeed of 1
	/// 
	/// so moveSpeed needs to be proportional to the number of points
	/// 
	/// moveSpeed could be the time taken to do a full cycle then divide by the number of points
	/// </summary>

	[SerializeField] private float stateInterval;
	[SerializeField] private Vector2 restPos;

	[Range(2, 45)] //360 or 90 isnt needed to get a smooth circle
	/// a max of 45 gets a pretty smooth circle with a relatively similar time taken with other num points still
	public int numPoints;
	[Range(0.1f, 10f)]
	[SerializeField] private float radius = 1f;
	[SerializeField] private bool isFollowPath = false;
	private ListNode<Vector2> target;


	[Header("Components")]
	private SpriteRenderer spriteRenderer;
	private new Collider2D collider;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		collider = GetComponent<Collider2D>();

		target = TurnPointsIntoTargetNode(GeneratePoints());
		transform.position = target.val;

		moveSpeed = 0.05f * numPoints;

		StartCoroutine(SwitchTarget());
	}

	private void FixedUpdate()
	{
		//switch between rest and circle based on isFollowPath
		if (isFollowPath)
		{
			transform.position = Vector2.MoveTowards(transform.position, target.val, moveSpeed);
		}
		else
		{
			transform.position = Vector2.MoveTowards(transform.position, restPos, moveSpeed);
		}
	}

	private void Update()
	{
		//if reached target then change the target
		if ((Vector2)transform.position == target.val)
		{
			target = target.next;
		}
	}

	private IEnumerator SwitchTarget()
	{
		while (true)
		{
			isFollowPath = false;
			//set the sprite renderer and collider false so that it goes 'invisible'
			spriteRenderer.enabled = false;
			collider.enabled = false;

			yield return new WaitForSeconds(stateInterval);

			isFollowPath = true;
			//set the sprite renderer and collider true
			spriteRenderer.enabled = true;
			collider.enabled = true;

			yield return new WaitForSeconds(stateInterval);
		}
	}


	private Vector2[] GeneratePoints()
	{
		Vector2[] points = new Vector2[numPoints];

		float degrees = 360 / numPoints;
		//generate points based on circle calculation (sin(x), cos(x))
		for (int i = 1; i < numPoints + 1; i++)
		{
			float d = degrees * Mathf.PI / 180f;
			Vector2 newPoint = new Vector2(Mathf.Sin(d * i), Mathf.Cos(d * i)) * radius + (Vector2)transform.position;

			points[i - 1] = newPoint;
		}

		return points;

	}


	//T specified to Vector2
	private ListNode<Vector2> TurnPointsIntoTargetNode(Vector2[] pathPoints)
	{
		ListNode<Vector2> head = new(pathPoints[0], null);

		var newNode = head;
		for (int i = 0; i < pathPoints.Length; i++)
		{
			if (i == pathPoints.Length - 1)
			{
				newNode.next = head;
				continue;
			}
			newNode.next = new ListNode<Vector2>(pathPoints[i+1], null);

			newNode = newNode.next;
		}

		return head;
	}
}
