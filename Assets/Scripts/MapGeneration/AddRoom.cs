using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.MapGeneration
{
	public class AddRoom : MonoBehaviour
	{
		public bool extended = false;

		private ExtensionMethods extensions;
		private MapCreation mapCreation;

		private void Start()
		{
			extensions = GameObject.FindGameObjectWithTag("Rooms").GetComponent<ExtensionMethods>();
			mapCreation = GameObject.FindGameObjectWithTag("Rooms").GetComponent<MapCreation>();
			//extensions = ExtensionMethods.Instance;

			//add self to rooms
			//GameManager.Instance.thisArea.rooms.Add(this.gameObject);

			//extend rooms if the map hasnt already been generated
			//when generating maps from rooms data then the number of rooms and roomsdata wont match up so check for if the number of rooms and rooms data are the same
			//if (GameManager.Instance.thisArea.rooms.Count == GameManager.Instance.thisArea.roomsData.Count)
			if (mapCreation.isNewMap)
			{
				RoomData newData = new RoomData(transform.position, name, new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST });
				GameManager.Instance.thisArea.rooms.Add(newData);

				//check if room is a closed one
				if (name.Contains("C"))
				{
					Invoke(nameof(ExtendClosedRoom), 1f);
				}
				else
				{
					int rand = Random.Range(0, 10);

					if (!extended)
					{
						if (rand <= 6) { Invoke(nameof(ExtendRoom), 1f); }
					}
					else
					{
						if (rand == 0) { Invoke(nameof(ExtendRoom), 1f); }
					}
				}
			}

			if (!name.Contains("C"))
			{
				//activate all walls if the room is an exit or has a limited number of empty surrounding rooms
				Invoke(nameof(ActivateWalls), 2f);
			}

			//Invoke(nameof(ParentRoom), 3f);
		}

		private void ParentRoom()
		{
			Transform mapParent = mapCreation.mapParent;

			transform.parent = mapParent;
		}

		private void ExtendClosedRoom()
		{
			//get surrounding
			//
			var adjRooms = extensions.GetAdjacentRooms(transform.position);
			if (adjRooms["TOP"] != null && adjRooms["BOTTOM"] != null)
			{
				extensions.DisableVerticalWalls(adjRooms["TOP"], gameObject);
				extensions.DisableVerticalWalls(gameObject, adjRooms["BOTTOM"]);
			}
			else if (adjRooms["LEFT"] != null && adjRooms["RIGHT"] != null)
			{
				extensions.DisableHorizontalWalls(adjRooms["LEFT"], gameObject);
				extensions.DisableHorizontalWalls(gameObject, adjRooms["RIGHT"]);
			}
		}


		private void ExtendRoom()
		{
			//ExtensionMethods extensions = ExtensionMethods.Instance;
			//get the adjacent rooms
			var adjRooms = extensions.GetAdjacentRooms(transform.position);
			
			//call the correct method from templates
			//get the random index of the method to be called
			int randFunc = Random.Range(0, extensions.extendFunction.GetPersistentEventCount());
			//get the method using the method name
			System.Reflection.MethodInfo method = extensions.GetType().GetMethod(extensions.extendFunction.GetPersistentMethodName(randFunc));
			if (method != null)
			{
				//print("method performed is " + randFunc + extensions.extendFunction.GetPersistentMethodName(randFunc));
				//invoke the method from templates and pass in the adjRooms as a parameter
				method.Invoke(extensions, new object[] { adjRooms });
				//templates.Invoke(nameof(method), new object[] { adjRooms });
			}
		}

		private void ActivateWalls()
		{
			//ExtensionMethods extensions = ExtensionMethods.Instance;
			var adjRooms = extensions.GetAdjacentRooms(transform.position);

			//check for the number of empty rooms or if the name has exit in it
			if (extensions.CountEmptyRooms(adjRooms) >= 3 || name.Contains("Exit"))
			{
				foreach (Transform wall in transform.Find("Walls"))
				{
					wall.gameObject.SetActive(true);
				}
			}
		}
	}
}