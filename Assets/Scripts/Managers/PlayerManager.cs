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
	public PlayerData currentData;
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

	private void Start()
	{
		currentPlayer = Instantiate(currentData.player);
		currentPlayer.GetComponent<PlayerController>().maxHP = currentData.maxHP;
		currentPlayer.GetComponent<PlayerController>().hp = currentData.currentHP > 0 ? currentData.currentHP : currentData.maxHP;
	}

	//called on playerdeath
	public UnityEvent PlayerDeath()
	{
		SceneManager.LoadScene("ObjectTesting");

		return null;
	}

	//oneline functions are cool :D
	//find the last child in Map and move player to it
	public void MovePlayer() => currentPlayer.transform.position = GameObject.Find("Map").transform.GetChild(GameObject.Find("Map").transform.childCount -1).position - new Vector3(0, 3, 0);
}
