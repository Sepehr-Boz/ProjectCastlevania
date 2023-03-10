using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRoom : MonoBehaviour {
	private RoomData roomData;

	public Area area;
	//public bool extended = false;

	void Start(){
		roomData = GameManager.Instance.roomData;

		switch (area)
		{
			case Area.RED:
				roomData.roomsA.Add(this.gameObject);
				break;
			case Area.GREEN:
				roomData.roomsB.Add(this.gameObject);
				break;
			case Area.BLUE:
				roomData.roomsC.Add(this.gameObject);
				break;
			case Area.YELLOW:
				roomData.roomsD.Add(this.gameObject);
				break;
			default:
				break;
		}
	}
}