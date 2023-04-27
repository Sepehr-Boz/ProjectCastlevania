using System.Collections;
using UnityEngine;

public class MapCreation : MonoBehaviour
{
	private RoomTemplates templates;

	public Transform mapParent;
	public Transform focusParent; //focusparent is purely used to group together the Focus gameobjects so that they hierarchy feels more organised
	public int mapSize = 10;


	private void Start()
	{
		//find mapParent if not added
		if (!mapParent)
		{
			mapParent = GameObject.Find("Map").transform;
		}

		templates = GetComponent<RoomTemplates>();



		CreateMap();
	}


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
			yield return new WaitForSeconds(0.25f);
			int start = mapParent.childCount;
			yield return new WaitForSeconds(0.25f);
			int end = mapParent.childCount;

			//check if map is within mapsize bounds and if not regenerate it
			if (end > mapSize)
			{
				CreateMap();
			}

			if (start == end)
			{
				//map is finished
				//extend the map
				//extensions.extendFunction.Invoke((Vector2)GetRandomRoom().transform.position);
				//move player
				PlayerManager.Instance.MovePlayer();

				//disable all the rooms so that the focus gameobjects do their job
				//invoke after a delay so that closed rooms can extend
				//DisableRooms();
				//Invoke(nameof(DisableRooms), 3f);

				//stop coroutine
				StopCoroutine(IsMapFinished());
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

	private void DisableRooms()
	{
		foreach (Transform child in mapParent)
		{
			child.gameObject.SetActive(false);
		}
	}

}