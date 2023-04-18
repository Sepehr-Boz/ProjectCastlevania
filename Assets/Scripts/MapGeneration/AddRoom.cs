using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;
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

			//check if the map is being generated for the first time and if it is add roomdata
			if (mapCreation.isNewMap)
			{
				RoomData newData = new RoomData(transform.position, name, new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST });
				GameManager.Instance.thisArea.rooms.Add(newData);

				if (name.Contains("C"))
				{
					ExtendClosedRoom();
				}
			}


			Invoke(nameof(ActivateWalls), 2f);

		}
			//	//check if room is a closed one
			//	if (name.Contains("C"))
			//	{
			//		Invoke(nameof(ExtendClosedRoom), 1f);
			//	}
			//	else if (name.Contains("--"))
			//	{
			//		//activate all walls of corridors
			//		//var walls = transform.Find("Walls");
			//		//walls.Find("North").gameObject.SetActive(true);
			//		//walls.Find("East").gameObject.SetActive(true);
			//		//walls.Find("South").gameObject.SetActive(true);
			//		//walls.Find("West").gameObject.SetActive(true);

			//		//corridors shouldnt be extended
			//	}
			//	else
			//	{
			//		//int rand = Random.Range(0, 10);

			//		//if (!extended)
			//		//{
			//		//	if (rand <= 7) { Invoke(nameof(ExtendRoom), 1f); }
			//		//}
			//		//else
			//		//{
			//		//	if (rand > 10) { Invoke(nameof(ExtendRoom), 1f); }
			//		//}
			//	}
			//}

			//if (!name.Contains("C"))
			//{
			//	//activate all walls if the room is an exit or has a limited number of empty surrounding rooms
			//	Invoke(nameof(ActivateWalls), 2f);
			//}
			//Invoke(nameof(ActivateWalls), 2f);
			//Invoke(nameof(ParentRoom), 3f);
		//}


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

			//if (adjRooms["TOP"] != null) { extensions.DisableVerticalWalls(adjRooms["TOP"], gameObject); }
			//if (adjRooms["BOTTOM"] != null) { extensions.DisableVerticalWalls(gameObject, adjRooms["BOTTOM"]); }
			//if (adjRooms["LEFT"] != null) { extensions.DisableHorizontalWalls(adjRooms["LEFT"], gameObject); }
			//if (adjRooms["RIGHT"] != null) { extensions.DisableHorizontalWalls(gameObject, adjRooms["RIGHT"]); }


			//extensions.HorizontalExtend(adjRooms);
			//extensions.VerticalExtend(adjRooms);
			//extensions.TwoxTwoRoom(adjRooms);
			//extensions.ThreexThreeRoom(adjRooms);

			//var walls = transform.Find("Walls");
			//extensions.DisableVerticalWalls(adjRooms["TOP"] == null ? null : adjRooms["TOP"], name.Contains("U") ? gameObject : null);
			//extensions.DisableVerticalWalls(name.Contains("D") ? gameObject : null, adjRooms["BOTTOM"] == null ? null : adjRooms["BOTTOM"]);

			//extensions.DisableHorizontalWalls(adjRooms["LEFT"] == null ? null : adjRooms["LEFT"], name.Contains("L") ? gameObject : null);
			//extensions.DisableHorizontalWalls(name.Contains("R") ? gameObject : null, adjRooms["RIGHT"] == null ? null : adjRooms["RIGHT"]);


			//extensions.DisableVerticalWalls(adjRooms["TOP"], name.Contains("U") ? gameObject : null);
			//extensions.DisableVerticalWalls(name.Contains("D") ? gameObject : null, adjRooms["BOTTOM"]);

			//extensions.DisableHorizontalWalls(adjRooms["LEFT"], name.Contains("L") ? gameObject : null);
			//extensions.DisableHorizontalWalls(name.Contains("R") ? gameObject : null, adjRooms["RIGHT"]);


			//walls.Find("North").gameObject.SetActive(adjRooms["TOP"] == null ? false : true);
			//walls.Find("East").gameObject.SetActive(adjRooms["RIGHT"] == null ? false : true);
			//walls.Find("South").gameObject.SetActive(adjRooms["BOTTOM"] == null ? false : true);
			//walls.Find("West").gameObject.SetActive(adjRooms["LEFT"] == null ? false : true);


			//if (adjRooms["TOP"] != null)
			//{
			//	extensions.DisableVerticalWalls(adjRooms["TOP"], gameObject);
			//}
			//if (adjRooms["BOTTOM"] != null)
			//{
			//	extensions.DisableVerticalWalls(gameObject, adjRooms["BOTTOM"]);
			//}
			//if (adjRooms["LEFT"] != null)
			//{
			//	extensions.DisableHorizontalWalls(adjRooms["LEFT"], gameObject);
			//}
			//if (adjRooms["RIGHT"] != null)
			//{
			//	extensions.DisableHorizontalWalls(gameObject, adjRooms["RIGHT"]);
			//}

			//}
			//if (adjRooms["TOP"] != null && adjRooms["BOTTOM"] != null)
			//{
			//	extensions.DisableVerticalWalls(adjRooms["TOP"], gameObject);
			//	extensions.DisableVerticalWalls(gameObject, adjRooms["BOTTOM"]);
			//}
			//else if (adjRooms["LEFT"] != null && adjRooms["RIGHT"] != null)
			//{
			//	extensions.DisableHorizontalWalls(adjRooms["LEFT"], gameObject);
			//	extensions.DisableHorizontalWalls(gameObject, adjRooms["RIGHT"]);
			//}
		}


		private void ExtendRoom()
		{
			//get the adjacent rooms
			var adjRooms = extensions.GetAdjacentRooms(transform.position);
			
			//call the correct method from templates
			//get the random index of the method to be called
			int randFunc = Random.Range(0, extensions.extendFunction.GetPersistentEventCount());
			//get the method using the method name
			System.Reflection.MethodInfo method = extensions.GetType().GetMethod(extensions.extendFunction.GetPersistentMethodName(randFunc));
			if (method != null)
			{
				//invoke the method from templates and pass in the adjRooms as a parameter
				method.Invoke(extensions, new object[] { adjRooms });
			}
		}

		private void ActivateWalls()
		{
			var adjRooms = extensions.GetAdjacentRooms(transform.position);
			var walls = transform.Find("Walls");

			//check each direction and enable the wall if there arent any rooms in that direction
			//for each wall, check if the room in that direction is empty and if so set the wall active, otherwise set the activity to what it originally is.
			walls.Find("North").gameObject.SetActive(!adjRooms["TOP"] ? true : walls.Find("North").gameObject.activeInHierarchy);
			walls.Find("East").gameObject.SetActive(!adjRooms["RIGHT"] ? true : walls.Find("East").gameObject.activeInHierarchy);
			walls.Find("South").gameObject.SetActive(!adjRooms["BOTTOM"] ? true : walls.Find("South").gameObject.activeInHierarchy);
			walls.Find("West").gameObject.SetActive(!adjRooms["LEFT"] ? true : walls.Find("West").gameObject.activeInHierarchy);
		}
	}
}