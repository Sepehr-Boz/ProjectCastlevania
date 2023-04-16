using Assets.Scripts.Data;
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
	public AreaData thisArea;

	public Cinemachine.CinemachineVirtualCamera virtualCamera;

	public int targetFPS;

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
		//PlayerManager.Instance.gameObject.SetActive(true);

		SceneManager.activeSceneChanged += SceneChanged;

		//set target fps
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = targetFPS;
	}

	private void SceneChanged(Scene current, Scene next)
	{
		//unload the resources from the previous scene to allow more memory and cpu/gpu power for the new scene
		Resources.UnloadUnusedAssets();

		//thisArea.rooms.Clear();
	}

	private void OnApplicationQuit()
	{
		//thisArea.rooms.Clear();

		//update the player player infos before quitting the game to save progress
		PlayerManager.Instance.currentData.currentScene = SceneManager.GetActiveScene().name;
		PlayerManager.Instance.currentData.currentPos = PlayerManager.Instance.currentPlayer.transform.position;
		PlayerManager.Instance.currentData.currentHP = PlayerManager.Instance.currentPlayer.GetComponent<PlayerController>().hp;
	}


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			Time.timeScale = 1;
		}
	}

	//load the new scene
	public void ChangeScene(string newScene)
	{
		SceneManager.LoadScene(newScene, LoadSceneMode.Single);
		//PlayerManager.Instance.currentPlayer.transform.position = newPos;
	}
}
