using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGeneration
{
	public class AddRoom : MonoBehaviour
	{
		public bool extended = false;

		private void Start()
		{
			//add self to rooms
			GameManager.Instance.thisArea.rooms.Add(this.gameObject);

			//extend rooms if the map hasnt already been generated
			//when generating maps from rooms data then the number of rooms and roomsdata wont match up so check for if the number of rooms and rooms data are the same
			if (GameManager.Instance.thisArea.rooms.Count == GameManager.Instance.thisArea.roomsData.Count)
			{
				//List<Wall> walls = new List<Wall> { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
				//GameManager.Instance.thisArea.roomsData.Add(new RoomData(transform.position, transform.rotation, walls, name, null, null));
				int rand = Random.Range(0, 10); 

				if (extended)
				{
					if (rand <= 3) { Invoke(nameof(ExtendRoom), 1.5f); }
				}
				else
				{
					if (rand <= 9) { Invoke(nameof(ExtendRoom), 1.5f); }
				}


				//if (Random.Range(0, 10) <= 9) {Invoke(nameof(ExtendRoom), 1.5f);}
			}

			//activate all walls if the room is an exit or has a limited number of empty surrounding rooms
			Invoke(nameof(ActivateWalls), 3f);
		}

		private void ExtendRoom()
		{
			//get the adjacent rooms
			var adjRooms = GameManager.Instance.templates.GetAdjacentRooms(this.gameObject);

			//call the correct method from templates
			GameManager.Instance.templates.extendFunction.Invoke(adjRooms);
		}

		private void ActivateWalls()
		{
			RoomTemplates templates = GameManager.Instance.templates;
			var adjRooms = templates.GetAdjacentRooms(gameObject);

			//check for the number of empty rooms or if the name has exit in it
			if (templates.CountEmptyRooms(adjRooms) >= 5 || name.Contains("Exit"))
			{
				foreach (Transform wall in transform.Find("Walls"))
				{
					wall.gameObject.SetActive(true);
				}
			}
		}
	}
}