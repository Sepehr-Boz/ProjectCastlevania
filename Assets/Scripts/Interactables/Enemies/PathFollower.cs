using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathFollower : EnemyController
{

	[Header("Paths")]
	[Space(2)]
	[SerializeField] private float stateInterval;
	[SerializeField] private Vector2 startPos;
	public List<Vector2> pathPoints;
	[SerializeField] private float radius = 1f;


	[SerializeField] private ListNode<Vector2> path;
	[SerializeField] private bool isFollowPath = false;

	//[SerializeField] private float radius = 1f;

	private new void Start()
	{
		for (int i = 0; i < pathPoints.Count; i++)
		{
			pathPoints[i] *= radius;
		}

		path = TurnPointsIntoTargetNode();
		target = path.val;

		StartCoroutine(SwitchTarget());
		//target = new Vector2(Mathf.Sin(Time.time) * radius, Mathf.Cos(Time.time) * radius);
	}

	private void Update()
	{
		//target = new Vector2(Mathf.Sin(Time.time) * radius, Mathf.Cos(Time.time) * radius); //to make the enemy move in a circle
		//if reached target then change the target
		if ((Vector2)transform.position == target)
		{
			path = path.next;
			//target = path.val;
		}

		//switch between rest and circle based on isFollowPath
		if (isFollowPath)
		{
			target = path.val;
		}
		else
		{
			target = startPos;
		}

	}

	private IEnumerator SwitchTarget()
	{
		while (true)
		{
			isFollowPath = false;
			yield return new WaitForSeconds(stateInterval);
			isFollowPath = true;
			yield return new WaitForSeconds(stateInterval);
		}
	}



	//T specified to Vector2
	private ListNode<Vector2> TurnPointsIntoTargetNode()
	{
		ListNode<Vector2> head = new(pathPoints[0], null);

		var newNode = head;
		for (int i = 0; i < pathPoints.Count; i++)
		{
			if (i == pathPoints.Count - 1)
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
