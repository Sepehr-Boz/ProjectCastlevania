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
		private ExtensionMethods extensions;

		private void Start()
		{
			extensions = GameObject.FindGameObjectWithTag("Rooms").GetComponent<ExtensionMethods>();

			name = name.Replace("(Clone)", "");
			if (name.Equals("C"))
			{
				ExtendRoom();
			}
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

	}
}