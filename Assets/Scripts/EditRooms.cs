using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;

public class EditRooms : MonoBehaviour
{
    public List<GameObject> rooms;

    public Tilemap newTilemap;
    public GameObject wallToCopy;
    // Start is called before the first frame update
    void Start()
    {
        //loop through all prefabs
        foreach (GameObject room in rooms)
        {
            GameObject wall = room.transform.Find("Walls").gameObject;
            if (!wall.GetComponent<TilemapRenderer>())
            {
                room.transform.Find("Walls").AddComponent<TilemapRenderer>();
				//room.GetComponent<TilemapRenderer>().sortOrder = 0;
				room.transform.Find("Walls").GetComponent<TilemapRenderer>().sortingLayerID = SortingLayer.NameToID("Background");
                print("sorting layer id " + room.transform.Find("Walls").GetComponent<TilemapRenderer>().sortingLayerID);
				print("sorting layer name" + room.transform.Find("Walls").GetComponent<TilemapRenderer>().sortingLayerName);
				print("sorting order" + room.transform.Find("Walls").GetComponent<TilemapRenderer>().sortingOrder);
				print("sort order" + room.transform.Find("Walls").GetComponent<TilemapRenderer>().sortOrder);
			}
            //room.transform.Find("Walls").gameObject = wallToCopy;
            //GameObject background = room.transform.Find("Walls").gameObject;
            //background = wallToCopy;
            //background.GetComponent<Tilemap>();

            //Destroy(background.GetComponent<Tilemap>());
            //var back = background.GetComponent<Tilemap>();
            //back = newTilemap;

            //var backrend = background.AddComponent<TilemapRenderer>();
            //backrend.sortingLayerID = 0;

            //background.AddComponent<Tilemap>(newTilemap);
            //background.AddComponent(newTilemap);

            print("new tilemap changed");
        }
        //find the Walls child
        //add tilemap renderer
        //copy the tilemap to new tilemap
    }
}
