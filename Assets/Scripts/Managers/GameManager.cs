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
		//load the scene the current player is in
		//LoadScene(PlayerManager.Instance.currentPlayer);

		//set target fps
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = targetFPS;
	}

	//load the correct scene
	//called whenever the player is switched
	public void LoadScene(PlayerData currentPlayer)
	{
		SceneManager.LoadScene(currentPlayer.currentArea.ToString());
	}
}
