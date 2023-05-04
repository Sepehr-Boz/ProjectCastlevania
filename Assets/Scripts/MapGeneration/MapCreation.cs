using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapCreation : MonoBehaviour
{
	private RoomTemplates templates;
	private ExtensionMethods extensions;

	public Transform mapParent;
	public Transform focusParent; //focusparent is purely used to group together the Focus gameobjects so that they hierarchy feels more organised

	public int maxMapSize = 20;
	public int minMapSize = 10;


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


	public void CreateMap()
	{
		//delete any rooms in map
		foreach (Transform child in mapParent.transform)
		{
			Destroy(child.gameObject);
		}

		//get a random boss room from templates
		int rand = Random.Range(0, templates.bossRooms.Length);
		GameObject start = Instantiate(templates.bossRooms[rand]);
		//instantiate and move to random position
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
			if (end > maxMapSize)
			{
				CreateMap();
			}

			if (start == end)
			{
				if (end < minMapSize || end > maxMapSize)
				{
					CreateMap();
					StopCoroutine(IsMapFinished());
					break;
				}
				//map is finished
				//extend the map UNSURE IF WANT EXTENSTIONS OR NOT
				//extensions.extendFunction.Invoke((Vector2)GetRandomRoom().transform.position);
				//move player
				PlayerManager.Instance.MovePlayer();

				//disable all the rooms so that the focus gameobjects do their job
				//invoke after a delay so that closed rooms can extend
				//Invoke(nameof(DisableRooms), 3f);
				//Invoke(nameof(EnableLastRoom), 3.5f);

				//Invoke(nameof(CheckRooms), 1f);
				//StartCoroutine(CheckRooms());
				//Invoke(nameof(ExtendRooms), 3f);

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
		//loop through each room
		foreach (Transform child in mapParent)
		{
			//set rooms inactive
			child.gameObject.SetActive(false);

			//destroy spawnpoints
			Destroy(child.Find("SpawnPoints").gameObject);
		}
	}

	private void EnableLastRoom()
	{
		//get the last focus in focus parent and trigger its function to activate and pass in the player so it does trigger as it should
		GameObject focus = focusParent.GetChild(focusParent.childCount - 1).gameObject;
		focus.GetComponent<Focuser>().OnTriggerEnter2D(PlayerManager.Instance.currentPlayer.GetComponent<Collider2D>());
	}

	//private void DestroySpawnPoints(GameObject room)
	//{
	//	foreach (Transform point in room.transform.Find("SpawnPoints"))
	//	{
	//		if (point.CompareTag("SpawnPoint"))
	//		{
	//			Destroy(point.gameObject);
	//		}
	//	}
	//}

	//private IEnumerator CheckRooms()
	//{
	//	//loop through each room with a small delay between
	//	//loop through each direction in the rooms name
	//	//check if that direction is valid
	//	foreach (Transform child in mapParent)
	//	{
	//		if (child.name.Contains("Boss") || child.name.Contains("--"))
	//		{
	//			continue;
	//		}

	//		var adjRooms = extensions.GetAdjacentRooms(child.position);
	//		string newName = child.name;

	//		foreach (char dir in child.name)
	//		{
	//			switch (dir)
	//			{
	//				case 'U':
	//					//check if that direction is valid
	//					if (adjRooms["TOP"] != null && adjRooms["TOP"].name.Contains("D"))
	//					{
	//						newName = newName.Replace("U", "");
	//					}
	//					break;
	//				case 'D':
	//					if (adjRooms["BOTTOM"] != null && adjRooms["BOTTOM"].name.Contains("U"))
	//					{
	//						newName = newName.Replace("D", "");
	//					}
	//					break;
	//				case 'L':
	//					if (adjRooms["LEFT"] != null && adjRooms["LEFT"].name.Contains("R"))
	//					{
	//						newName = newName.Replace("L", "");
	//					}
	//					break;
	//				case 'R':
	//					if (adjRooms["RIGHT"] != null && adjRooms["RIGHT"].name.Contains("L"))
	//					{
	//						newName = newName.Replace("R", "");
	//					}
	//					break;
	//			}
	//		}

	//		if (newName != child.name)
	//		{
	//			newName = newName.Replace("Trap", "").Replace("1", "").Replace("2", "").Replace("3", "").RemoveConsecutiveCharacters(' ');
	//			//print("ORIGINAL: " + child.name + " NEW: " + newName);
	//			GameObject room = Instantiate(templates.GetRoom(newName));
	//			//DestroySpawnPoints(room);
	//			Destroy(room.transform.Find("SpawnPoints").gameObject);
	//			room.transform.parent = mapParent;
	//			room.transform.position = child.position;

	//			Destroy(child.gameObject);
	//		}



	//		yield return new WaitForSeconds(0.5f);
	//	}
	//}

	//private void PrintDictionary(Dictionary<string, GameObject> dict)
	//{
	//	string line = "";
	//	foreach (GameObject adj in dict.Values)
	//	{
	//		try
	//		{
	//			line += adj.name;
	//		}
	//		catch
	//		{
	//			line += "null";
	//		}
	//	}

	//	print(line);
	//}

	//private IEnumerator CheckRooms()
	//{
	//	//loop through each room and get its neightbours
	//	//check if theres a blocked exit and if there is then delete the room and spawn a new one with its spawnpoints removed
	//	foreach (Transform child in mapParent)
	//	{
	//		//get the adjacent rooms
	//		var adjRooms = extensions.GetAdjacentRooms(child.position);
	//		PrintDictionary(adjRooms);

	//		if (child.name.Contains("Boss") || child.name.Contains("--"))
	//		{
	//			continue;
	//		}

	//		string newName = child.name;

	//		if (adjRooms["TOP"] != null && !adjRooms["TOP"].name.Contains("D"))
	//		{
	//			newName = newName.Replace("U", "");
	//		}
	//		if (adjRooms["BOTTOM"] != null && adjRooms["BOTTOM"].name.Contains("U"))
	//		{
	//			newName = newName.Replace("D", "");
	//		}
	//		if (adjRooms["LEFT"] != null && adjRooms["LEFT"].name.Contains("R"))
	//		{
	//			newName = newName.Replace("L", "");
	//		}
	//		if (adjRooms["RIGHT"] != null && adjRooms["RIGHT"].name.Contains("L"))
	//		{
	//			newName = newName.Replace("R", "");
	//		}


	//		if (newName != child.name && newName != "")
	//		{
	//			newName = newName.Replace("Trap", "");
	//			newName = newName.Replace("1", "");
	//			//print("ORIGINAL: " + child.name + " NEW: " + newName);
	//			GameObject room = Instantiate(templates.GetRoom(newName));
	//			//DestroySpawnPoints(room);
	//			Destroy(room.transform.Find("SpawnPoints").gameObject);
	//			room.transform.parent = mapParent;
	//			room.transform.position = child.position;

	//			Destroy(child.gameObject);
	//		}
	//		else
	//		{
	//			continue;
	//		}

	//		yield return new WaitForSeconds(0.5f);

	//		//if (child.name.Contains("U") && adjRooms["TOP"] && !adjRooms["TOP"].name.Contains("D"))
	//		//{
	//		//	newName = newName.Replace("U", "");
	//		//}
	//		//if (child.name.Contains("D") && adjRooms["BOTTOM"] && !adjRooms["BOTTOM"].name.Contains("U"))
	//		//{
	//		//	newName = newName.Replace("D", "");
	//		//}
	//		//if (child.name.Contains("L") && adjRooms["LEFT"] && !adjRooms["LEFT"].name.Contains("R"))
	//		//{
	//		//	newName = newName.Replace("L", "");
	//		//}
	//		//if (child.name.Contains("R") && adjRooms["RIGHT"] && !adjRooms["RIGHT"].name.Contains("L"))
	//		//{
	//		//	newName = newName.Replace("R", "");
	//		//}

	//		//if (newName != child.name)
	//		//{
	//		//	newName = newName.Replace("Trap", "");
	//		//	newName = newName.Replace("1", "");
	//		//	GameObject room = Instantiate(templates.GetRoom(newName));
	//		//	//DestroySpawnPoints(room);
	//		//	Destroy(room.transform.Find("SpawnPoints").gameObject);
	//		//	room.transform.parent = mapParent;
	//		//	room.transform.position = child.position;

	//		//	Destroy(child.gameObject);
	//		//}
	//		//else
	//		//{
	//		//	continue;
	//		//}
	//	}

	//	//Invoke(nameof(ExtendRooms), 2f);
	//}

	private void ExtendRooms()
	{
		foreach (Transform child in mapParent)
		{
			if (child.name.Equals("C"))
			{
				extensions.ExtendClosedRoom(child.gameObject);
			}
		}

		//extensions.extendFunction.Invoke((Vector2)GetRandomRoom().transform.position);
	}

}