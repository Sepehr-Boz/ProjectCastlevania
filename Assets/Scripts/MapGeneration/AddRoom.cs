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
			//extensions = ExtensionMethods.Instance;


			//add self to rooms
			//GameManager.Instance.thisArea.rooms.Add(this.gameObject);

			//extend rooms if the map hasnt already been generated
			//when generating maps from rooms data then the number of rooms and roomsdata wont match up so check for if the number of rooms and rooms data are the same
			//if (GameManager.Instance.thisArea.rooms.Count == GameManager.Instance.thisArea.roomsData.Count)
			//Invoke(nameof(ActivateWalls), 2f);

			if (mapCreation.isNewMap)
			{
				RoomData newData = new RoomData(transform.position, name, new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST });
				GameManager.Instance.thisArea.rooms.Add(newData);

				//check if room is a closed one
				if (name.Contains("C"))
				{
					Invoke(nameof(ExtendClosedRoom), 1f);
				}
				else if (name.Contains("--"))
				{
					//activate all walls of corridors
					//var walls = transform.Find("Walls");
					//walls.Find("North").gameObject.SetActive(true);
					//walls.Find("East").gameObject.SetActive(true);
					//walls.Find("South").gameObject.SetActive(true);
					//walls.Find("West").gameObject.SetActive(true);

					//corridors shouldnt be extended
				}
				else
				{
					//int rand = Random.Range(0, 10);

					//if (!extended)
					//{
					//	if (rand <= 7) { Invoke(nameof(ExtendRoom), 1f); }
					//}
					//else
					//{
					//	if (rand > 10) { Invoke(nameof(ExtendRoom), 1f); }
					//}
				}
			}

			//if (!name.Contains("C"))
			//{
			//	//activate all walls if the room is an exit or has a limited number of empty surrounding rooms
			//	Invoke(nameof(ActivateWalls), 2f);
			//}
			//Invoke(nameof(ActivateWalls), 2f);
			//Invoke(nameof(ParentRoom), 3f);
		}


		private void ExtendClosedRoom()
		{
			//get surrounding
			//
			var adjRooms = extensions.GetAdjacentRooms(transform.position);


			extensions.HorizontalExtend(adjRooms);
			extensions.VerticalExtend(adjRooms);
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
			print("ROOMS HATH BEEN EXTENDED");

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
			var adjRooms = extensions.GetAdjacentRooms(transform.position);
			var walls = transform.Find("Walls");

			//random chance to either disable walls or keep as is
			//int rand = Random.Range(0, 1);

			////set wall active if the room above is empty or has a direction towards the current room in its name
			//if (adjRooms["TOP"] == null || adjRooms["TOP"].name.Contains("D"))
			//{
			//	walls.Find("North").gameObject.SetActive(true);
			//}
			////else
			////{
			////	//if rand is 1 then set wall inactive
			////	if (rand == 1)
			////	{
			////		walls.Find("North").gameObject.SetActive(false);
			////	}
			////}

			//if (adjRooms["BOTTOM"] == null || adjRooms["BOTTOM"].name.Contains("U"))
			//{
			//	walls.Find("South").gameObject.SetActive(true);
			//}
			////else
			////{
			////	if (rand == 1)
			////	{
			////		walls.Find("South").gameObject.SetActive(false);
			////	}
			////}

			//if (adjRooms["LEFT"] == null || adjRooms["LEFT"].name.Contains("R"))
			//{
			//	walls.Find("West").gameObject.SetActive(true);
			//}
			////else
			////{
			////	if (rand == 1)
			////	{
			////		walls.Find("West").gameObject.SetActive(false);
			////	}
			////}

			//if (adjRooms["RIGHT"] == null || adjRooms["RIGHT"].name.Contains("L"))
			//{
			//	walls.Find("East").gameObject.SetActive(true);
			//}
			////else
			////{
			////	if (rand == 1)
			////	{
			////		walls.Find("East").gameObject.SetActive(false);
			////	}
			////}

			//check each direction and enable the wall if there arent any rooms in that direction
			//for each wall, check if the room in that direction is empty and if so set the wall active, otherwise set the activity to what it originally is.
			walls.Find("North").gameObject.SetActive(!adjRooms["TOP"] ? true : walls.Find("North").gameObject.activeInHierarchy);
			walls.Find("East").gameObject.SetActive(!adjRooms["RIGHT"] ? true : walls.Find("East").gameObject.activeInHierarchy);
			walls.Find("South").gameObject.SetActive(!adjRooms["BOTTOM"] ? true : walls.Find("South").gameObject.activeInHierarchy);
			walls.Find("West").gameObject.SetActive(!adjRooms["LEFT"] ? true : walls.Find("West").gameObject.activeInHierarchy);

			//extensions.DisableVerticalWalls(adjRooms["TOP"], name.Contains("U") ? gameObject : null);
			//extensions.DisableVerticalWalls(name.Contains("D") ? gameObject : null, adjRooms["BOTTOM"]);
			//extensions.DisableHorizontalWalls(adjRooms["LEFT"], name.Contains("L") ? gameObject : null);
			//extensions.DisableHorizontalWalls(name.Contains("R") ? gameObject : null, adjRooms["RIGHT"]);
		}
	}
}