using UnityEngine;

namespace Assets.Scripts.MapGeneration
{
	public class AddRoom : MonoBehaviour
	{
		public bool extended = false;

		private void Start()
		{
			//add self to rooms
			GameManager.Instance.thisArea.rooms.Add(this.gameObject);

			//extend rooms if the map hasnt already been generated
			//when generating maps from rooms data then the number of rooms and roomsdata wont match up so check for if the number of rooms and rooms data are the same
			if (GameManager.Instance.thisArea.rooms.Count == GameManager.Instance.thisArea.roomsData.Count)
			{
				int rand = Random.Range(0, 10);

				if (rand <= 1)
				{
					Invoke(nameof(ExtendRoom), 5f);
				}
			}

			//if the room is an exit one then set all walls active
			if (name.Contains("Exit"))
			{
				Invoke(nameof(ActivateWalls), 6f);
			}
		}

		private void ExtendRoom()
		{
			//get the adjacent rooms
			var adjRooms = GameManager.Instance.templates.GetAdjacentRooms(this.gameObject);

			//call the correct method from templates
			GameManager.Instance.templates.extendFunction.Invoke(adjRooms);
		}

		private void ActivateWalls()
		{
			foreach (Transform wall in transform.Find("Walls"))
			{
				wall.gameObject.SetActive(true);
			}
		}

		//public void OnCollisionEnter2D(Collision2D collision)
		//{
		//	print("AHHHHH A COLLIDION HATH COOCOCCURED BETWEEBNTH THE 2 GAMEOBJECTS" + collision.collider.name + "    " + collision.otherCollider.name);
		//	//check if collided with a wall by comparing the parent of the collided object
		//	if (collision.transform.parent.name.Equals("Walls") && collision.gameObject != collision.otherCollider.gameObject)
		//	{
		//		//set the wall inactive
		//		collision.gameObject.SetActive(false);
		//	}
		//}

		//public void OnCollisionEnter2D(Collision2D collision)
		//{
		//	print(gameObject.name + " has collided with " + collision.gameObject.name);
		//}
	}
}