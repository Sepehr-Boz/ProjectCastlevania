using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
	//[SerializeField] public Dictionary<PlayerData, GameObject> players = new(2);
	public List<PlayerData> players;
	public GameObject currentPlayer;

	public GameObject playerA;
	public GameObject playerB;
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

		playerA = Instantiate(players[0].player);
		playerA.transform.position = Vector2.zero;
		playerB = Instantiate(players[1].player);
		playerB.transform.position = Vector2.zero;
		playerB.SetActive(false);

		DontDestroyOnLoad(players[0].player);
		DontDestroyOnLoad(players[1].player);

		currentPlayer = playerA;
	}
	#endregion

	private void Start()
	{
		//currentPlayer = players[0];
	}
}
