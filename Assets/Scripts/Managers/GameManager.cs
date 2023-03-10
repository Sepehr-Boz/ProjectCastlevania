using Assets.Scripts.MapGeneration;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Space(2)]
    public RoomTemplates templates;

    public float waitTime;
	public int targetFPS;

	public RoomData roomData;
	public PlayerData playerData;


	public GameObject[] startRooms;


	#region singleton
	private static GameManager _instance;

	public static GameManager Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}
	#endregion


	private void Start()
	{
		//set the entry rooms inactive until the players are spawned
		foreach (GameObject obj in startRooms)
		{
			obj.SetActive(false);
		}

		//spawn the players
		PlayerManager.Instance.FirstSpawn();

		//move the entry rooms to the player positions
		Invoke(nameof(SetRooms), 4f);

		//check is rooms are pooled and if theyre then set the entry rooms active

		//set the target fps
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = targetFPS;

	}

	private void SetRooms()
	{
		SetRoom(0);
		SetRoom(1);
		SetRoom(2);
		SetRoom(3);

		foreach (GameObject obj in startRooms)
		{
			obj.SetActive(true);
		}

	}

	private void SetRoom(int index)
	{
		startRooms[index].transform.position = PlayerManager.Instance.transform.GetChild(index).transform.position;
	}

	//empty the rooms lists on game end
	private void OnApplicationQuit()
	{
		roomData.roomsA.Clear();
		roomData.roomsB.Clear();
		roomData.roomsC.Clear();
		roomData.roomsD.Clear();
	}

	#region map methods
	public void DestroyArea(int index)
    {
		List<GameObject> rooms = GetListFromIndex(index);

		GameObject startRoom = rooms[0];
		startRoom.SetActive(false);
		ObjectPooling.Instance.RenewPooledObject(ref startRoom);

        for (int i = 0; i < rooms.Count; i++)
		{
			rooms[i].SetActive(false);
			//adds back any missing room spawner components in the spawn points when the rooms are set inactive again
		}

		while (rooms.Count > 1)
		{
			rooms.RemoveAt(1);
		}


    }

    public void GenerateArea()
    {
		var room = startRooms[PlayerManager.Instance.roomsIndex];
		room.transform.position = PlayerManager.Instance.currentPlayer.transform.position;
		room.transform.position = GetClosestCentre(room.transform.position);
		room.SetActive(true);
    }

	public Vector2 GetClosestCentre(Vector2 pos)
	{
		//find the closest 10s in the x and y area and apply it

		//divide the x and y by 10 and round them to the closest integer
		int x = Mathf.RoundToInt(pos.x / 10);
		int y = Mathf.RoundToInt(pos.y / 10);
		//then multiply the x and y by 10 to get them back to their normal values
		pos = new Vector2(x * 10, y * 10);

		return pos;
	}

	public ref List<GameObject> GetListFromIndex(int index)
	{
		switch (index)
		{
			case 0:
				return ref roomData.roomsA;
			case 1:
				return ref roomData.roomsB;
			case 2:
				return ref roomData.roomsC;
			case 3:
				return ref roomData.roomsD;
		}

		return ref roomData.roomsA;
	}

	#endregion
}
