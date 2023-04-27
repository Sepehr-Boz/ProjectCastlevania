using Assets.Scripts.Interactables.Enemies;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.MapGeneration
{
	public class AddRoom : MonoBehaviour
	{
		private RoomTemplates templates;
		private ExtensionMethods extensions;
		private MapCreation mapCreation;

		private GameObject focus;
		public GameObject exits;

		[SerializeField] private List<GameObject> enemies;

		private void Start()
		{
			//delete the Grid component
			//THIS IS ONLY FOR TESTING, AS DELETING GRID MAKES IT HARDER TO SEE WHAT THE ROOMS LOOK LIKE IN THE ASSETS PAGE
			//FOR PRODUCTION THEN REMOVE THE GRID COMPONENT FROM ALL THE ROOMS AND REMOVE THIS CODE THAT DESTROYS THE GRID AS ITS NOT NEEDED
			try
			{
				Destroy(GetComponent<Grid>());
			}catch{}


			extensions = GameManager.Instance.extensions;
			templates = GameManager.Instance.templates;
			mapCreation = GameManager.Instance.mapCreation;


			//spawn exits but disable them for now
			exits = Instantiate(templates.exits);
			exits.transform.parent = transform;
			exits.transform.position = transform.position;
			exits.SetActive(false);


			//add a focus gameobject that IS NOT A CHILD OF THE ROOM
			//the focus is linked to the room so that whenever its triggered the room is enabled
			//thus the processing power SHOULD BE reduced by quite a bit as now only n number of colliders are kept active as well as 1/2 rooms
			focus = Instantiate(templates.focus);
			focus.transform.parent = mapCreation.focusParent;
			focus.transform.SetPositionAndRotation(transform.position, transform.rotation);
			focus.GetComponent<Focuser>().room = gameObject;
			focus.SetActive(true);


			name = name.Replace("(Clone)", "");
			if (name.Equals("C"))
			{
				Invoke(nameof(ExtendRoom), 1f);
			}

			//disable all rooms after a delay when the map is finished
			Invoke(nameof(DisableSelf), 7f);
		}

		private void DisableSelf()
		{
			gameObject.SetActive(false);
		}

		public void OnDestroy()
		{
			//destroy self as well as focus
			Destroy(focus);
			Destroy(exits);
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

		private List<GameObject> GetEnemiesInRoom()
		{
			List<GameObject> enem = new();

			foreach (Transform child in transform)
			{
				//if (child.GetComponent<EnemyController>() && child.name.Contains("Enemy") && child.GetComponent<EnemyController>().hp > 0)
				//checking for HasHP should return true for all child subclasses that inherit from the abstract class
				if (child.GetComponent<HasHP>() && child.GetComponent<HasHP>().hp > 0)
				{
					enem.Add(child.gameObject);
				}
			}

			return enem;
		}


		public bool CheckIfEnemiesNull() => enemies.Count == 0;


		public void TriggerExits()
		{
			//get a list of enemies childed to the room
			//reinitialise the enemies inside the room when loaded as when the room is set inactive then the references go missing
			enemies = GetEnemiesInRoom();

			try
			{
				if (CheckIfEnemiesNull())
				{
					Destroy(exits);
				}
				else
				{
					exits.SetActive(true);
				}
			}catch{}
		}

	}
}