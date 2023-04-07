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
		//StartCoroutine(TemplateDelay());

		SceneManager.activeSceneChanged += SceneChanged;


		//set target fps
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = targetFPS;
	}

	//private IEnumerator TemplateDelay()
	//{
	//	yield return new WaitForSeconds(1);
	//	templates.gameObject.SetActive(true);
	//}

	private void SceneChanged(Scene current, Scene next)
	{
		//unload the resources from the previous scene to allow more memory and cpu/gpu power for the new scene
		Resources.UnloadUnusedAssets();

		thisArea.rooms.Clear();
	}

	private void OnApplicationQuit()
	{
		thisArea.rooms.Clear();
	}


	private void Update()
	{
		//FOR TESTING
		Application.targetFrameRate = targetFPS;

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			//Resources.UnloadUnusedAssets(); //unload previous scene assets from previous scenes
			SceneManager.LoadScene("Area1"); //load new scene
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			//Resources.UnloadUnusedAssets();
			SceneManager.LoadScene("Area2");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			//Resources.UnloadUnusedAssets();
			SceneManager.LoadScene("Area3");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			//Resources.UnloadUnusedAssets();
			SceneManager.LoadScene("Area4");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			//Resources.UnloadUnusedAssets();
			SceneManager.LoadScene("Area5");
		}


		if (Input.GetKeyDown(KeyCode.Space))
		{
			Time.timeScale = 1;
		}

	}

	//load the new scene
	public void ChangeScene(string newScene, Vector2 newPos)
	{
		SceneManager.LoadScene(newScene, LoadSceneMode.Single);
		//SceneManager.LoadScene(newScene);
		PlayerManager.Instance.currentPlayer.transform.position = newPos;
	}
}
