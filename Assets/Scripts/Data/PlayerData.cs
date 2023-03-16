using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData")]
public class PlayerData : ScriptableObject
{
	public new string name;

	public GameObject player;
	public float moveSpeed;

	public Vector3 position;
	public Quaternion rotation;
	public Area currentArea;

}