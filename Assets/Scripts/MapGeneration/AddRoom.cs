using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.MapGeneration
{
	public class AddRoom : MonoBehaviour
	{
		public bool extended = false;

		//private Renderer renderer;
		//private float time = 3f;

		private void Start()
		{
			////define the renderer
			//try
			//{
			//	renderer = transform.Find("Walls").GetComponent<TilemapRenderer>();
			//}
			//catch
			//{
			//	renderer=null;
			//}

			//check if room is a closed one
			if (name.Contains("C"))
			{
				Invoke(nameof(ExtendClosedRoom), 2f);


				//if (adjRooms["TOP"] != null && adjRooms["TOP"].name.Contains("D"))
				//{
				//	transform.Find("Walls").Find("North").gameObject.SetActive(false);
				//}
				//if (adjRooms["BOTTOM"] != null && adjRooms["BOTTOM"].name.Contains("U"))
				//{
				//	transform.Find("Walls").Find("South").gameObject.SetActive(false);
				//}
				//if (adjRooms["LEFT"] != null && adjRooms["LEFT"].name.Contains("R"))
				//{
				//	transform.Find("Walls").Find("West").gameObject.SetActive(false);
				//}
				//if (adjRooms["RIGHT"] != null && adjRooms["RIGHT"].name.Contains("L"))
				//{
				//	transform.Find("Walls").Find("East").gameObject.SetActive(false);
				//}
			}




			//add self to rooms
			GameManager.Instance.thisArea.rooms.Add(this.gameObject);

			//extend rooms if the map hasnt already been generated
			//when generating maps from rooms data then the number of rooms and roomsdata wont match up so check for if the number of rooms and rooms data are the same
			if (GameManager.Instance.thisArea.rooms.Count == GameManager.Instance.thisArea.roomsData.Count)
			{
				int rand = Random.Range(0, 10); 

				if (!extended)
				{
					if (rand <= 9) { Invoke(nameof(ExtendRoom), 1.5f); }
				}
				else
				{
					if (rand <= 1) { Invoke(nameof(ExtendRoom), 1.5f); }
				}
			}

			//activate all walls if the room is an exit or has a limited number of empty surrounding rooms
			Invoke(nameof(ActivateWalls), 3f);

			//Invoke(nameof(Update), 3f);
		}

		private void ExtendClosedRoom()
		{
			//get surrounding rooms
			var adjRooms = RoomTemplates.Instance.GetAdjacentRooms(transform.position);
			if (adjRooms["TOP"] != null && adjRooms["BOTTOM"] != null)
			{
				RoomTemplates.Instance.DisableVerticalWalls(adjRooms["TOP"], gameObject);
				RoomTemplates.Instance.DisableVerticalWalls(gameObject, adjRooms["BOTTOM"]);
			}
			else if (adjRooms["LEFT"] != null && adjRooms["RIGHT"] != null)
			{
				RoomTemplates.Instance.DisableHorizontalWalls(adjRooms["LEFT"], gameObject);
				RoomTemplates.Instance.DisableHorizontalWalls(gameObject, adjRooms["RIGHT"]);
			}
		}


		private void ExtendRoom()
		{
			RoomTemplates templates = RoomTemplates.Instance;
			//get the adjacent rooms
			var adjRooms = templates.GetAdjacentRooms(transform.position);
			
			//call the correct method from templates
			//get the random index of the method to be called
			int randFunc = Random.Range(0, templates.extendFunction.GetPersistentEventCount());
			//get the method using the method name
			System.Reflection.MethodInfo method = templates.GetType().GetMethod(templates.extendFunction.GetPersistentMethodName(randFunc));
			if (method != null)
			{
				print("method performed is " + randFunc + templates.extendFunction.GetPersistentMethodName(randFunc));
				//invoke the method from templates and pass in the adjRooms as a parameter
				method.Invoke(templates, new object[] { adjRooms });
				//templates.Invoke(nameof(method), new object[] { adjRooms });
			}


			////add random chance to extend to closed off rooms
			//int rand = Random.Range(0, 1);
			//if (rand == 0)
			//{
			//	//find the closed off room

			//	//if the adjacent room isnt empty, isnt a closed room, and doesnt have an exit to the current room
			//	if (adjRooms["TOP"] != null && !adjRooms["TOP"].name.Contains("C") && !adjRooms["TOP"].name.Contains("D"))
			//	{
			//		RoomTemplates.Instance.DisableVerticalWalls(adjRooms["TOP"], gameObject);
			//		return;
			//	}
			//	if (adjRooms["BOTTOM"] != null && !adjRooms["BOTTOM"].name.Contains("C") && !adjRooms["BOTTOM"].name.Contains("U"))
			//	{
			//		RoomTemplates.Instance.DisableVerticalWalls(gameObject, adjRooms["BOTTOM"]);
			//		return;
			//	}
			//	if (adjRooms["LEFT"] != null && !adjRooms["LEFT"].name.Contains("C") && !adjRooms["LEFT"].name.Contains("R"))
			//	{
			//		RoomTemplates.Instance.DisableHorizontalWalls(adjRooms["LEFT"], gameObject);
			//		return;
			//	}
			//	if (adjRooms["RIGHT"] != null && !adjRooms["RIGHT"].name.Contains("C") && !adjRooms["RIGHT"].name.Contains("L"))
			//	{
			//		RoomTemplates.Instance.DisableHorizontalWalls(gameObject, adjRooms["RIGHT"]);
			//		return;
			//	}
			//}
			//else
			//{
			//	return;
			//}
		}

		private void ActivateWalls()
		{
			RoomTemplates templates = RoomTemplates.Instance;
			var adjRooms = templates.GetAdjacentRooms(transform.position);

			//check for the number of empty rooms or if the name has exit in it
			if (templates.CountEmptyRooms(adjRooms) >= 5 || name.Contains("Exit"))
			{
				foreach (Transform wall in transform.Find("Walls"))
				{
					wall.gameObject.SetActive(true);
				}
			}
		}

		//private void Update()
		//{
		//	if (name.Contains("C"))
		//	{
		//		CancelInvoke(nameof(Update));
		//	}


		//	if (time <= 0)
		//	{
		//		//set inactive if not visible
		//		if (!renderer.isVisible || renderer == null)
		//		{
		//			print(name + " is not visible ");
		//			gameObject.SetActive(false);
		//			print(name + "has been set inactive");
		//		}
		//		else
		//		{
		//			//set active if is visible
		//			gameObject.SetActive(true);
		//		}
		//	}
		//	else
		//	{
		//		time -= Time.deltaTime;
		//	}
		//}
	}
}