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
		//public bool extended = false; not needed

		private ExtensionMethods extensions;
		//private MapCreation mapCreation;

		private void Start()
		{
			extensions = GameObject.FindGameObjectWithTag("Rooms").GetComponent<ExtensionMethods>();
			//mapCreation = GameObject.FindGameObjectWithTag("Rooms").GetComponent<MapCreation>();

			name = name.Replace("(Clone)", "");

			if (name.Equals("C"))
			{
				ExtendRoom();
			}

			////check if the map is being generated for the first time and if it is add roomdata
			//if (mapCreation.isNewMap)
			//{
			//	RoomData newData = new RoomData(transform.position, name, new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST });
			//	GameManager.Instance.thisArea.rooms.Add(newData);

			//	if (name.Contains("C"))
			//	{
			//		ExtendRoom();
			//	}
			//}
		}


		private void ExtendRoom()
		{
			//get surrounding rooms
			var adjRooms = extensions.GetAdjacentRooms(transform.position);


			try{ extensions.DisableVerticalWalls(adjRooms["TOP"].name.Contains("D") ? adjRooms["TOP"] : null, gameObject);}
			catch{}
			try{extensions.DisableVerticalWalls(gameObject, adjRooms["BOTTOM"].name.Contains("U") ? adjRooms["BOTTOM"] : null);}
			catch{}

			try{extensions.DisableHorizontalWalls(adjRooms["LEFT"].name.Contains("R") ? adjRooms["LEFT"] : null, gameObject);}
			catch{}
			try{extensions.DisableHorizontalWalls(gameObject, adjRooms["RIGHT"].name.Contains("L") ? adjRooms["RIGHT"] : null);}
			catch{}


		}


		#region depreceated by used advanced info which i might reference back later
		//private void ExtendRoom()
		//{
		//	//get the adjacent rooms
		//	var adjRooms = extensions.GetAdjacentRooms(transform.position);

		//	//call the correct method from templates
		//	//get the random index of the method to be called
		//	int randFunc = Random.Range(0, extensions.extendFunction.GetPersistentEventCount());
		//	//get the method using the method name
		//	System.Reflection.MethodInfo method = extensions.GetType().GetMethod(extensions.extendFunction.GetPersistentMethodName(randFunc));
		//	if (method != null)
		//	{
		//		//invoke the method from templates and pass in the adjRooms as a parameter
		//		method.Invoke(extensions, new object[] { adjRooms });
		//	}
		//}

		//private void ActivateWalls()
		//{
		//	var adjRooms = extensions.GetAdjacentRooms(transform.position);
		//	var walls = transform.Find("Walls");


		//	////check each direction and enable the wall if there arent any rooms in that direction
		//	////for each wall, check if the room in that direction is empty and if so set the wall active, otherwise set the activity to what it originally is.
		//	//walls.Find("North").gameObject.SetActive(!adjRooms["TOP"] ? true : walls.Find("North").gameObject.activeInHierarchy);
		//	//walls.Find("East").gameObject.SetActive(!adjRooms["RIGHT"] ? true : walls.Find("East").gameObject.activeInHierarchy);
		//	//walls.Find("South").gameObject.SetActive(!adjRooms["BOTTOM"] ? true : walls.Find("South").gameObject.activeInHierarchy);
		//	//walls.Find("West").gameObject.SetActive(!adjRooms["LEFT"] ? true : walls.Find("West").gameObject.activeInHierarchy);
		//}
		#endregion
	}
}