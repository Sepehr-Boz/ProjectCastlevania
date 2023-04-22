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
				Invoke(nameof(ExtendRoom), 1f);
			}
		}


		private void ExtendRoom()
		{
			//get surrounding rooms
			var adjRooms = extensions.GetAdjacentRooms(transform.position);

			//extend vertically or horizontally if there are rooms in both directions
			if (adjRooms["TOP"] && adjRooms["BOTTOM"])
			{
				extensions.DisableVerticalWalls(adjRooms["TOP"], gameObject);
				extensions.DisableVerticalWalls(gameObject, adjRooms["BOTTOM"]);
			}
			else if (adjRooms["LEFT"] && adjRooms["RIGHT"])
			{
				extensions.DisableHorizontalWalls(adjRooms["LEFT"], gameObject);
				extensions.DisableHorizontalWalls(gameObject, adjRooms["RIGHT"]);
			}//otherwise only extend in one direction //also add extra cgeck statement so that it doesnt extend towards a corridor
			else if (adjRooms["TOP"] && adjRooms["TOP"].name.Contains("D") && !adjRooms["TOP"].name.Contains("--"))
			{
				extensions.DisableVerticalWalls(adjRooms["TOP"], gameObject);
			}
			else if (adjRooms["BOTTOM"] && adjRooms["BOTTOM"].name.Contains("U") && !adjRooms["BOTTOM"].name.Contains("--"))
			{
				extensions.DisableVerticalWalls(gameObject, adjRooms["BOTTOM"]);
			}
			else if (adjRooms["LEFT"] && adjRooms["LEFT"].name.Contains("R") && !adjRooms["LEFT"].name.Contains("--"))
			{
				extensions.DisableHorizontalWalls(adjRooms["LEFT"], gameObject);
			}
			else if (adjRooms["RIGHT"] && adjRooms["RIGHT"].name.Contains("L") && !adjRooms["RIGHT"].name.Contains("--"))
			{
				extensions.DisableHorizontalWalls(gameObject, adjRooms["RIGHT"]);
			}
		}

	}
}