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