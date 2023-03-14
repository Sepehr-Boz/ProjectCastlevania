using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
	public List<PlayerData> players;
	public GameObject currentPlayer;


	#region singleton
	private static PlayerManager _instance;

	public static PlayerManager Instance
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


	//public Transform currentPlayer;
	////public int roomsIndex;

	////public Cinemachine.CinemachineVirtualCamera virtualCamera;

	//public float waitTime;

	//public int range = 5;

	//#region spawning players
	//public void FirstSpawn()
	//{
	//	SpawnPlayers();
	//	Invoke(nameof(SetActivePlayer), 2f);

	//}

	//private void SetActivePlayer()
	//{
	//	//get random int between 0 and 3
	//	int rand = Random.Range(0, 3);
	//	//set the child(random index) to the active one
	//	currentPlayer = transform.GetChild(rand);
	//	currentPlayer.gameObject.SetActive(true);

	//	//SetCamera(currentPlayer);

	//	roomsIndex = rand;
	//}

	//private void SpawnPlayer(GameObject player, Vector3 pos)
	//{
	//	var playerRef = Instantiate(player);
	//	playerRef.transform.parent = transform;
	//	playerRef.transform.position = pos;
	//	playerRef.SetActive(false);
	//}

	//private void SpawnPlayers()
	//{
	//	List<Vector2> pos = GenerateListofUniqueStartPos();
	//	print(pos);

	//	//get random positions of the players and spawn them at the locations
	//	SpawnPlayer(GameManager.Instance.playerData.playerA, pos[0]);
	//	GameManager.Instance.SetRoom(0);
	//	SpawnPlayer(GameManager.Instance.playerData.playerB, pos[1]);
	//	GameManager.Instance.SetRoom(1);
	//	SpawnPlayer(GameManager.Instance.playerData.playerC, pos[2]);
	//	GameManager.Instance.SetRoom(2);
	//	SpawnPlayer(GameManager.Instance.playerData.playerD, pos[3]);
	//	GameManager.Instance.SetRoom(3);

	//	GameManager.Instance.SetInbetweenRooms(pos);

	//	pos.Clear();
	//}

	//private List<Vector2> GenerateListofUniqueStartPos()
	//{
	//	//generate 4 unique vector2 and return them in a list
	//	List<Vector2> pos = new();

	//	while (pos.Count < 4)
	//	{
	//		Vector2 newPos = new(Random.Range(-range, range) * 10, Random.Range(-range, range) * 10);
	//		if (!pos.Contains(newPos))
	//		{
	//			pos.Add(newPos);
	//		}
	//	}

	//	return pos;

	//}
	////checks if any positions are repeating so that players arent spawned in the same area
	////private Vector3 GenerateSpawnPos(Vector3 pos)
	////{
	////	Vector3 newPos = new (Random.Range(-range, range) * 10, Random.Range(-range, range) * 10, 0);
	////	while (newPos == pos)
	////	{
	////		newPos = new Vector3(Random.Range(-range, range) * 10, Random.Range(-range, range) * 10, 0);
	////	}

	////	return newPos;

	////}
	//#endregion


	//#region callable methods
	///*
	//public void SetCamera(Transform player)
	//{
	//	//set the follow and look at in cinemachine to new player
	//	virtualCamera.Follow = player;
	//	virtualCamera.LookAt = player;
	//}*/

	//public void NextCharacter()
	//{
	//	int currentIndex = currentPlayer.GetSiblingIndex();
	//	currentPlayer.gameObject.SetActive(false);

	//	int new_index = (currentIndex + 1) % 4;
	//	roomsIndex = new_index;

	//	currentPlayer = transform.GetChild(new_index);
	//	currentPlayer.gameObject.SetActive(true);


	//	virtualCamera.Follow = currentPlayer;
	//	virtualCamera.LookAt = currentPlayer;

	//	//SetCamera(currentPlayer.transform);
	//}
	//#endregion
}
