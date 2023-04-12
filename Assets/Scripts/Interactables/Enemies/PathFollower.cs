using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathFollower : EnemyController
{
	public List<Vector2> pathPoints;
	[SerializeField] private ListNode<Vector2> path;

	//[SerializeField] private float radius = 1f;

	private new void Start()
	{
		path = TurnPointsIntoTargetNode();
		target = path.val;
		//target = new Vector2(Mathf.Sin(Time.time) * radius, Mathf.Cos(Time.time) * radius);
	}

	//private new void FixedUpdate()
	//{
	//	//move towards target
	//	base.FixedUpdate();
	//}

	private void Update()
	{
		//target = new Vector2(Mathf.Sin(Time.time) * radius, Mathf.Cos(Time.time) * radius); //to make the enemy move in a circle
		//if reached target then change the target
		if ((Vector2)transform.position == target)
		{
			path = path.next;
			target = path.val;
		}
	}



	//T specified to Vector2
	private ListNode<Vector2> TurnPointsIntoTargetNode()
	{
		//List<ListNode<Vector2>> tempList = new();

		//foreach (Vector2 point in pathPoints)
		//{
		//	ListNode<Vector2> newTarget = new ListNode<Vector2>(point, null);
		//	tempList.Add(newTarget);
		//}

		//for (int i = 0; i < tempList.Count; i++)
		//{
		//	if (i == tempList.Count - 1)
		//	{
		//		tempList[i].next = tempList[0];
		//		continue;
		//	}
		//	tempList[i].next = tempList[i + 1];
		//}

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
