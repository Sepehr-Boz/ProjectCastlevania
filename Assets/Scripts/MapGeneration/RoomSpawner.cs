using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static Unity.Burst.Intrinsics.X86.Avx;
using Random = UnityEngine.Random;

public class RoomSpawner : MonoBehaviour 
{
	[Header("Directions")]
	public int openingDirection;
	// 1 --> need bottom door --> top exit
	// 2 --> need top door --> bottom exit
	// 3 --> need left door --> right exit
	// 4 --> need right door --> left exit

	[Header("References")]
	private RoomTemplates templates;
	private MapCreation mapCreation;
	private ExtensionMethods extensions;

	private GameObject room = null;

	[Header("Spawning")]
	private int rand;
	public bool spawned = false;

	public float waitTime = 4f;

	void Start()
	{
		templates = GameManager.Instance.templates;
		mapCreation = GameManager.Instance.mapCreation;
		extensions = GameManager.Instance.extensions;

		//must destroy instead of setting inactive as the rooms will continue to spawn on top of each other even when set inactive
		Destroy(gameObject, waitTime);

		//stagger the spawn times otherwise the game lags quite a bit
		Invoke(nameof(Spawn), openingDirection / 50f);
	}

	//spawning the next room
	void Spawn(){
		if(spawned == false){
			spawned = true;

			if (openingDirection == 1){
				// Need to spawn a room with a BOTTOM door.
				rand = Random.Range(0, templates.bottomRooms.Length);
				room = templates.bottomRooms[rand];
			} 
			else if(openingDirection == 2){
				// Need to spawn a room with a TOP door.
				rand = Random.Range(0, templates.topRooms.Length);
				room = templates.topRooms[rand];
			}
			else if(openingDirection == 3){
				// Need to spawn a room with a LEFT door.
				rand = Random.Range(0, templates.leftRooms.Length);
				room = templates.leftRooms[rand];
			}
			else if(openingDirection == 4){
				// Need to spawn a room with a RIGHT door.
				rand = Random.Range(0, templates.rightRooms.Length);
				room = templates.rightRooms[rand];
			}

			//make sure that theres no exits towards corridors
			room = CheckForCorridors(room);

			//instantiate room, child it to map parent and move it to current position
			room = Instantiate(room);
			room.transform.parent = mapCreation.mapParent;
			room.transform.position = transform.position;

			switch (openingDirection)
			{
				case 1:
					Destroy(room.transform.Find("SpawnPoints").Find("DOWN").gameObject);
					break;
				case 2:
					Destroy(room.transform.Find("SpawnPoints").Find("UP").gameObject);
					break;
				case 3:
					Destroy(room.transform.Find("SpawnPoints").Find("LEFT").gameObject);
					break;
				case 4:
					Destroy(room.transform.Find("SpawnPoints").Find("RIGHT").gameObject);
					break;
			}

			//finally set rooms active
			room.SetActive(true);
		}
	}

	private GameObject CheckForCorridors(GameObject currentRoom)
	{
		var adjRooms = extensions.GetAdjacentRooms(transform.position);
		string name = currentRoom.name;

		if (adjRooms["TOP"] && adjRooms["TOP"].name.Equals("LR--"))
		{
			name = name.Replace("U", "");
		}
		if (adjRooms["BOTTOM"] && adjRooms["BOTTOM"].name.Equals("LR--"))
		{
			name = name.Replace("D", "");
		}
		if (adjRooms["LEFT"] && adjRooms["LEFT"].name.Equals("UD--"))
		{
			name = name.Replace("L", "");
		}
		if (adjRooms["RIGHT"] && adjRooms["RIGHT"].name.Equals("UD--"))
		{
			name = name.Replace("R", "");
		}

		if (name != currentRoom.name)
		{
			name = name.Replace("Trap", "").Replace(" 1", "");
			return templates.GetRoom(name);
		}



		return currentRoom;
	}


	private void SpawnConnectRoom(GameObject other)
	{
		//instead of spawning a closed room get the direction that the 2 points that triggered each other and spawn a room that connects the 2
		int dir1 = openingDirection;
		int dir2 = other.GetComponent<RoomSpawner>().openingDirection;

		string newName = "";

		//top down right left
		switch (dir1)
		{
			case 1:
				newName += "U";
				break;
			case 2:
				newName += "D";
				break;
			case 3:
				newName += "R";
				break;
			case 4:
				newName += "L";
				break;

		}

		switch (dir2)
		{
			case 1:
				newName += "U";
				break;
			case 2:
				newName += "D";
				break;
			case 3:
				newName += "R";
				break;
			case 4:
				newName += "L";
				break;

		}

		GameObject room = Instantiate(templates.GetRoom(newName));
		room.transform.parent = mapCreation.mapParent;

		room.transform.position = transform.position;
		room.SetActive(true);

	}


	void OnTriggerEnter2D(Collider2D other){
		//occurs when 2 rooms attempt to spawn a room at the same area
		if(other.CompareTag("SpawnPoint")){
			try
			{
				if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
				{
					SpawnConnectRoom(other.gameObject);
				}
			}
			catch{}

			spawned = true;
		}
	}
}
