using System.Collections;
using UnityEngine;

public class MapCreation : MonoBehaviour
{
	private RoomTemplates templates;

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

}