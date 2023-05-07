using UnityEngine;

namespace Additional
{
	public static class AdditionalMethods
	{

		//extension method to draw a box as debug.draw only has ray and line not box
		public static void DrawBox(this Debug debug, Vector2 centre, Vector2 size)
		{
			Vector3 pointA = new(centre.x - size.x, centre.y + size.y, 0);
			Vector3 pointB = new(centre.x + size.x, centre.y + size.y, 0);
			Vector3 pointC = new(centre.x + size.x, centre.y - size.y, 0);
			Vector3 pointD = new(centre.x - size.x, centre.y - size.y, 0);


			Debug.DrawLine(pointA, pointB, Color.red, 5f);
			Debug.DrawLine(pointB, pointC, Color.red, 5f);
			Debug.DrawLine(pointC, pointD, Color.red, 5f);
			Debug.DrawLine(pointD, pointA, Color.red, 5f);
		}


	}
}
