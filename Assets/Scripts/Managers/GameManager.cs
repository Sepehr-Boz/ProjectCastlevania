using Assets.Scripts.Data;
using Assets.Scripts.MapGeneration;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public AreaData thisArea;
	public RoomTemplates templates;

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
		//delay setting templates active so that rooms can be pooled before having to be accessed
		StartCoroutine(TemplateDelay());

		SceneManager.activeSceneChanged += SceneChanged;


		//set target fps
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = targetFPS;
	}

	private IEnumerator TemplateDelay()
	{
		yield return new WaitForSeconds(1);
		templates.gameObject.SetActive(true);
	}

	private void SceneChanged(Scene current, Scene next)
	{
		if (current.name == "MazeA" || current.name == "MazeB")
		{
			//deletes room data before moving to the next scene
			thisArea.roomsData.Clear();
		}
		thisArea.rooms.Clear();
	}


	private void Update()
	{
		//FOR TESTING
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SceneManager.LoadScene("MazeA");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SceneManager.LoadScene("MazeB");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SceneManager.LoadScene("AreaA");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			SceneManager.LoadScene("AreaB");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			SceneManager.LoadScene("AreaC");
		}


		
	}


	//load the correct scene
	//called whenever the player is switched
	public void LoadScene(PlayerData currentPlayer)
	{
		SceneManager.LoadScene(currentPlayer.currentArea.ToString());
	}







}
