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
	//public AreaData thisArea;

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
		if (templates == null || mapCreation == null || extensions == null)
		{
			GameObject room = GameObject.FindGameObjectWithTag("Rooms");
			templates = room.GetComponent<RoomTemplates>();
			mapCreation = room.GetComponent<MapCreation>();
			extensions = room.GetComponent<ExtensionMethods>();
		}


		//SceneManager.activeSceneChanged += SceneChanged;

		//set target fps
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = targetFPS;
	}

	//private void SceneChanged(Scene current, Scene next)
	//{
	//	//unload the resources from the previous scene to allow more memory and cpu/gpu power for the new scene
	//	Resources.UnloadUnusedAssets();
	//}

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


[System.Serializable]
public class ListNode<T>
{
	public T val; //T has to be specified in the class name, but when calling it the T has to specified
	public ListNode<T> next;

	public ListNode(T value, ListNode<T> next)
	{
		this.val = value;
		this.next = next;
	}
}