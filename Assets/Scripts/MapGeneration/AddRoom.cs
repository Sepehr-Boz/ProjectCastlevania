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

		private void Awake()
		{
			name = name.Replace("(Clone)", "");
		}

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
			//only enable/disable exits is there are ones already added
			try
			{
				exits.SetActive(false);
			}catch{}


			//add a focus gameobject that IS NOT A CHILD OF THE ROOM
			//the focus is linked to the room so that whenever its triggered the room is enabled
			//thus the processing power SHOULD BE reduced by quite a bit as now only n number of colliders are kept active as well as 1/2 rooms
			focus = Instantiate(templates.focus);
			focus.transform.parent = mapCreation.focusParent;
			focus.transform.SetPositionAndRotation(transform.position, transform.rotation);
			focus.GetComponent<Focuser>().room = gameObject;
			focus.SetActive(true);
		}

		public void OnDestroy()
		{
			//destroy self as well as focus
			//nest inside a try catch as only some rooms will have an exits added to it
			try
			{
				Destroy(focus);
				Destroy(exits);
			}catch{}
		}
		private List<GameObject> GetEnemiesInRoom()
		{
			List<GameObject> enem = new();

			foreach (Transform child in transform)
			{
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