using Assets.Scripts.MapGeneration;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
	public Cinemachine.CinemachineVirtualCamera virtualCamera;
	public int targetFPS;

	public RoomTemplates templates;
	public MapCreation mapCreation;
	public ExtensionMethods extensions;


	public int currentLevel = 1;
	public int coins = 0;

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
		//add the references if they havent already been manually added
		if (templates == null || mapCreation == null || extensions == null)
		{
			GameObject room = GameObject.FindGameObjectWithTag("Rooms");
			templates = room.GetComponent<RoomTemplates>();
			mapCreation = room.GetComponent<MapCreation>();
			extensions = room.GetComponent<ExtensionMethods>();
		}

		//set target fps
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = targetFPS;
	}

	private void OnApplicationQuit()
	{
		//update the player player infos before quitting the game to save progress
		PlayerManager.Instance.currentData.currentHP = PlayerManager.Instance.currentData.maxHP;
	}


	private void Update()
	{
		//FOR TESTING
		//enabled fast remaking of map to check for any problems that could occur
		if (Input.GetKeyDown(KeyCode.T))
		{
			//update the player hp when moving between levels
			PlayerManager.Instance.currentData.currentHP = PlayerManager.Instance.currentPlayer.GetComponent<PlayerController>().hp;
			mapCreation.CreateMap();
		}

		if (Input.GetKeyDown(KeyCode.Y))
		{
			Time.timeScale = 1;
		}

	}

	//load the new scene
	public void ChangeScene(string newScene)
	{
		SceneManager.LoadScene(newScene, LoadSceneMode.Single);
	}
}