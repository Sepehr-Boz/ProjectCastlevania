using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathFollower : EnemyController
{
	public List<Vector2> pathPoints;
	[SerializeField] private TargetNode path;

	private new void Start()
	{
		path = TurnPointsIntoTargetNode();

		target = path.position;
	}

	//private new void FixedUpdate()
	//{
	//	//move towards target
	//	base.FixedUpdate();
	//}

	private void Update()
	{
		//if reached target then change the target
		if ((Vector2)transform.position == target)
		{
			path = path.next;
			target = path.position;
		}
	}




	private TargetNode TurnPointsIntoTargetNode()
	{
		List<TargetNode> tempList = new();

		foreach (Vector2 point in pathPoints)
		{
			TargetNode newTarget = new TargetNode(point, null);
			tempList.Add(newTarget);
		}

		for (int i = 0; i < tempList.Count; i++)
		{
			if (i == tempList.Count - 1)
			{
				tempList[i].next = tempList[0];
				continue;
			}
			tempList[i].next = tempList[i + 1];
		}

		return tempList[0];
	}
}


[System.Serializable]class TargetNode
{
	public Vector2 position;
	public TargetNode next;

	public TargetNode(Vector2 pos, TargetNode nextTarget)
	{
		this.position = pos;
		this.next = nextTarget;
	}
}
