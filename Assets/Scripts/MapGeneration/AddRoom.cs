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