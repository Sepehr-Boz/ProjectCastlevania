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
	}
}

//public class AddRoom : MonoBehaviour
//{
//	//private RoomData roomData;

//	//public Area area;
//	//public bool extended = false;

//	void Start(){
//		//add the room to the rooms in the area
//		GameManager.Instance.thisArea.rooms.Add(this.gameObject);

//		//roomData = GameManager.Instance.roomData;

//		//switch (area)
//		//{
//		//	case Area.RED:
//		//		roomData.roomsA.Add(this.gameObject);
//		//		break;
//		//	case Area.GREEN:
//		//		roomData.roomsB.Add(this.gameObject);
//		//		break;
//		//	case Area.BLUE:
//		//		roomData.roomsC.Add(this.gameObject);
//		//		break;
//		//	case Area.YELLOW:
//		//		roomData.roomsD.Add(this.gameObject);
//		//		break;
//		//	default:
//		//		break;
//		//}
//	}
//}