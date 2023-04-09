using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
	//[SerializeField] public Dictionary<PlayerData, GameObject> players = new(2);
	public List<PlayerData> playerInfo;
	private PlayerData currentData;
	private PlayerData inactiveData;

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

		DontDestroyOnLoad(gameObject);

		//playerA = Instantiate(players[0].player);
		//playerA.transform.position = Vector2.zero;
		//playerB = Instantiate(players[1].player);
		//playerB.transform.position = Vector2.zero;
		//playerB.SetActive(false);

		//DontDestroyOnLoad(players[0].player);
		//DontDestroyOnLoad(players[1].player);

		currentData = playerInfo[0];
		inactiveData = playerInfo[1];

		//currentPlayer = Instantiate(currentData.player);

		//currentPlayer.transform.position = currentData.currentPos;

		//currentPlayer.GetComponent<PlayerController>().hp = currentData.currentHP;
		//currentPlayer.GetComponent<PlayerController>().maxHP = currentData.maxHP;
		//StartCoroutine(LoadNewPlayer());
	}
	#endregion

	private void Start()
	{
		//check that the current scene is correct
		//if (currentData.currentScene != SceneManager.GetActiveScene().name)
		//{
		//	GameManager.Instance.ChangeScene(currentData.currentScene);
		//}

		//if (currentPlayer == playerInfo[0].player)
		//{
		//	//check the current scene with player info 0 name
		//	if (playerInfo[0].currentScene != SceneManager.GetActiveScene().name)
		//	{
		//		GameManager.Instance.ChangeScene(playerInfo[0].currentScene);
		//	}
		//}
		//else if (currentPlayer == playerInfo[1].player)
		//{
		//	//check the current scene with player info 1 name
		//	if (playerInfo[1].currentScene != SceneManager.GetActiveScene().name)
		//	{
		//		GameManager.Instance.ChangeScene(playerInfo[1].currentScene);
		//	}
		//}
	}

	//FOR TESTING
	public void StartGame()
	{
		SceneManager.LoadScene("Area1");
		StartCoroutine(DelaySpawn());
	}

	private IEnumerator DelaySpawn()
	{
		yield return new WaitForSeconds(2);
		StartCoroutine(LoadNewPlayer());
	}




	public void SwitchPlayer(GameObject currentPlayer)
	{
		//update the info at currentdata
		//set players hp back to max
		currentData.currentHP = currentPlayer.GetComponent<PlayerController>().maxHP;
		currentData.maxHP = currentPlayer.GetComponent<PlayerController>().maxHP;

		currentData.currentPos = currentPlayer.transform.position;
		currentData.currentScene = SceneManager.GetActiveScene().name;

		Destroy(currentPlayer);

		//switch the 2 data
		var temp = currentData;
		currentData = inactiveData;
		inactiveData = temp;

		//load the new player
		StartCoroutine(LoadNewPlayer());


		//load the data from inactive data
		//currentPlayer = Instantiate(inactiveData.player);

		//currentPlayer.transform.position = currentData.currentPos;

		//currentPlayer.GetComponent<PlayerController>().hp = currentData.currentHP;
		//currentPlayer.GetComponent<PlayerController>().maxHP = currentData.maxHP;
		//LoadNewPlayer();



		////update the information in the player data
		//if (currentPlayer == playerInfo[0].player)
		//{
		//	//update playerdata[0]
		//	playerInfo[0].currentHP = currentPlayer.GetComponent<PlayerController>().hp;
		//	playerInfo[0].maxHP = currentPlayer.GetComponent<PlayerController>().maxHP;

		//	playerInfo[0].currentPos = currentPlayer.transform.position;
		//	playerInfo[0].currentScene = SceneManager.GetActiveScene().name;

		//	//destroy current player
		//	Destroy(currentPlayer);

		//	//set the currentplayer to the other player
		//	currentPlayer = Instantiate(playerInfo[1].player);
		//}
		//else if (currentPlayer == playerInfo[1].player)
		//{
		//	//update playerdata[1]
		//	playerInfo[1].currentHP = currentPlayer.GetComponent<PlayerController>().hp;
		//	playerInfo[1].maxHP = currentPlayer.GetComponent<PlayerController>().maxHP;

		//	playerInfo[1].currentPos = currentPlayer.transform.position;
		//	playerInfo[1].currentScene = SceneManager.GetActiveScene().name;

		//	//destroy current player
		//	Destroy(currentPlayer);

		//	//set the currentplayer to the other player
		//	currentPlayer = Instantiate(playerInfo[0].player);
		//}
		//else
		//{
		//	print("player doesnt match either");
		//}

		//load the scene the other player is in

		//set previous player inactive and new player active
	}


	private IEnumerator LoadNewPlayer()
	{
		//check scene FIRST because the spawned player is tied to the scene so if the scene is changed then the player disappears
		//check if current scene is correct
		if (currentData.currentScene != SceneManager.GetActiveScene().name)
		{
			GameManager.Instance.ChangeScene(currentData.currentScene);
			//LoadNewPlayer();
			yield return new WaitForSeconds(2);
		}


		currentPlayer = Instantiate(currentData.player);

		currentPlayer.transform.position = currentData.currentPos;

		currentPlayer.GetComponent<PlayerController>().hp = currentData.currentHP;
		currentPlayer.GetComponent<PlayerController>().maxHP = currentData.maxHP;
	}
}
