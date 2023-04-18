using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "PlayerData")]
public class PlayerData : ScriptableObject
{
	public GameObject player;
	public int currentHP;
	public int maxHP;


	public Vector2 currentPos;
	public string currentScene;
}