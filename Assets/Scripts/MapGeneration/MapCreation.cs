using System.Collections;
using UnityEngine;

public class MapCreation : MonoBehaviour
{
	[Header("Other scripts")]
	private RoomTemplates templates;
	private ExtensionMethods extensions;

	public Transform mapParent;


	public int mapSize = 10;


	private void Start()
	{
		//find mapParent if not added
		if (!mapParent)
		{
			mapParent = GameObject.Find("Map").transform;
		}

		templates = GetComponent<RoomTemplates>();
		extensions = GetComponent<ExtensionMethods>();

		CreateMap();
	}

	[ContextMenu("Create Map")]
	public void CreateMap()
	{
		//delete any rooms in map
		foreach (Transform child in mapParent.transform)
		{
			Destroy(child.gameObject);
		}


		//get a random boss room from templates
		int rand = Random.Range(0, templates.bossRooms.Length);
		GameObject start = templates.bossRooms[rand];
		//instantiate and move to origin
		start = Instantiate(start);
		start.transform.parent = mapParent;
		start.transform.position = Vector2.zero;

		StartCoroutine(IsMapFinished());
	}

	private IEnumerator IsMapFinished()
	{
		while (true)
		{
			yield return new WaitForSeconds(1);
			int start = mapParent.childCount;
			yield return new WaitForSeconds(1);
			int end = mapParent.childCount;

			if (start == end)
			{
				//check if map is within mapsize bounds and if not regenerate it
				if (mapParent.childCount > mapSize)
				{
					CreateMap();
				}
				else
				{
					//map is finished
					//extend the map
					//extensions.extendFunction.Invoke((Vector2)GetRandomRoom().transform.position);
					//move player
					PlayerManager.Instance.MovePlayer();
					//stop coroutine
					StopCoroutine(IsMapFinished());
				}
				//if dont break then will keep looping even though the coroutine is stopped? dunno why
				break;
			}
		}
	}


	//recursive function
	//keep recurving until a valid room can be returned
	private GameObject GetRandomRoom()
	{
		int rand = Random.Range(0, mapParent.childCount - 1);
		GameObject room = mapParent.GetChild(rand).gameObject;

		//a valid room is one thats not a boss or closed one
		if (!room.name.Contains("Boss") && !room.name.Equals("C"))
		{
			return room;
		}
		//keep recursing if its not valid
		return GetRandomRoom();
	}

}