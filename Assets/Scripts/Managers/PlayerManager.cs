using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
	//[SerializeField] public Dictionary<PlayerData, GameObject> players = new(2);
	public List<PlayerData> playerInfo;
	public PlayerData currentData;
	[SerializeField] private PlayerData inactiveData;

	public GameObject currentPlayer;
	//public GameObject currentPlayer;

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

	private void Start()
	{
		currentData = playerInfo[0];

		currentPlayer = Instantiate(currentData.player);
		currentPlayer.GetComponent<PlayerController>().maxHP = currentData.maxHP;
		currentPlayer.GetComponent<PlayerController>().hp = currentData.currentHP > 0 ? currentData.currentHP : currentData.maxHP;
	}

	//private void Start()
	//{
	//	//check for which player is active in the scene
	//	foreach (PlayerData data in playerInfo)
	//	{
	//		//check if the scene the player is in is the current scene
	//		if (data.currentScene == SceneManager.GetActiveScene().name)
	//		{
	//			//check for which player is the active one by checking which hp is more than 0
	//			if (data.currentHP > 0)
	//			{
	//				//if its the active player then set currentdata and currentplayer
	//				currentData = data;
	//				currentPlayer = Instantiate(data.player);
	//				currentPlayer.transform.position = data.currentPos;
	//				currentPlayer.GetComponent<PlayerController>().maxHP = data.maxHP;
	//				//return the current hp if its not 0 otherwise return max hp
	//				currentPlayer.GetComponent<PlayerController>().hp = data.currentHP != 0 ? data.currentHP : data.maxHP;

	//				currentPlayer.GetComponent<PlayerController>().isCurrent = true;
	//				continue;
	//			}

	//			//otherwise just spawn the player in
	//			inactiveData = data;
	//			GameObject tmp = Instantiate(data.player);
	//			tmp.transform.position = data.currentPos;
	//tmp.GetComponent<PlayerController>().maxHP = data.maxHP;
	//			tmp.GetComponent<PlayerController>().hp = data.currentHP > 0 ? data.currentHP : data.maxHP;
	//			//also update playerinfo hp - as the inactive players data wont update on current players death as it's hp would stay the same
	//			inactiveData.currentHP = tmp.GetComponent<PlayerController>().hp;

	//			//disable the inputs for the inactive player
	//			tmp.GetComponent<PlayerController>().isCurrent = false;
	//		}
	//	}
	//}

	////one line function :D proud of myself. Just short form of switching to the next players scene on SwitchPlayer call
	//public void SwitchPlayer() => SceneManager.LoadScene(inactiveData.currentScene);


	//oneline functions are cool :D
	//find the last child in Map and move player to it
	public void MovePlayer() => currentPlayer.transform.position = GameObject.Find("Map").transform.GetChild(GameObject.Find("Map").transform.childCount -1).position;
}
