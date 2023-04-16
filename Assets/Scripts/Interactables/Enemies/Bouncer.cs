using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField] private Vector2 moveDir;
    //[SerializeField] private Vector2 endpoint;

    //[Range(0f, 1f)]
    public float moveSpeed;


    //tried to make dirs const but in C# const arrays aren't possible. Readonly should deliver the same function I want however.
    private readonly Vector2[] wallCheckDirs = new Vector2[4]
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left,
	};


    // Start is called before the first frame update
    void Start()
    {
        moveDir = moveDir == null ? new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) : moveDir;
        //endpoint = GetNewEndPoint(moveDir);
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)moveDir / moveSpeed;
        //transform.position = Vector2.MoveTowards(transform.position, endpoint, moveSpeed);
    }

    //private Vector2 GetNewEndPoint(Vector2 castDir)
    //{
    //    RaycastHit2D hit = Physics2D.RaycastAll(transform.position, -castDir)[1];

    //    return hit.point * 0.95f;
    //}


    //collisions between 2 objects NEED at least one of the objects to have a rigidbody otherwise the collision wont trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //send out 4 raycasts from the centre to 0.1f outside the circle to see which collide
        //once one collides then return the
        //Debug.DrawLine(transform.position, transform.position + new Vector3(0, 1, 0) * 5f, Color.red, 5f); //TOP
        //Debug.DrawLine(transform.position, transform.position + new Vector3(1, 0, 0) * 5f, Color.black, 5f); //RIGHT
        //Debug.DrawLine(transform.position, transform.position + new Vector3(0, -1, 0) * 5f, Color.yellow, 5f); //BOTTOM
        //Debug.DrawLine(transform.position, transform.position + new Vector3(-1, 0, 0) * 5f, Color.blue, 5f); //LEFT
        //ISSUE WAS THAT THE RAYCAST WAS COLLIDING WITH SELF


        //loop through the directions of the raycasts
        foreach (Vector2 castDir in wallCheckDirs)
        {
            try
            {
                //get the second collider from a raycast from centre outwards
                //get second hit as first will be the gameobject itself
                RaycastHit2D hit = Physics2D.RaycastAll(transform.position, castDir, 0.6f)[1];

                //check that it something thats valid
                if (hit.collider.transform.root.gameObject.GetComponent<IIndestructable>() != null)
                {
					//draw a line to see where it hits - FOR TESTING
					Debug.DrawLine(transform.position, hit.point, Color.red, 5f);

					//if hit then decrement the dir the gameobject moves in by castDir to make it 'bounce'/'reflect'
                    //if (castDir.x != 0)
                    //{
                    //    moveDir.x *= -1;
                    //}
                    //else if (castDir.y != 1)
                    //{
                    //    moveDir.y *= -1;
                    //}
					//moveDir -= castDir;


                    //times values by -1 so that the values are the same negative and positive
                    ///e.g. if y was 0.2 and the code subtracted castDir ( which could have y of 1 or -1 ) then the negative/new value of y in moveDir
                    ///would've become -0.8 when 1 was subtracted which caused a different pattern that it moves in
                    ///x -1 keeps the values the same but just changes the direction
                    if (castDir.x != 0)
                    {
                        moveDir.x *= -1;
                    }
                    else if (castDir.y != 0)
                    {
                        moveDir.y *= -1;
                    }


                    //endpoint = GetNewEndPoint(castDir);
				}
            }
            catch
            {
                //will be caught if theres no collision other than self so continue onwards
                continue;
            }
        }


        //RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, new(0, 1), 10f);

        //try
        //{
        //    RaycastHit2D hit = hits[1];

        //    Debug.DrawLine(transform.position, hit.point, Color.red, 5f);
        //}
        //catch
        //{
        //    print("only collided with self");
        //}
  //      //RaycastHit2D hit = Physics2D.Raycast(transform.position, new(0, -1), 5f);
  //      if (hits != null)
  //      {
  //          foreach (RaycastHit2D hit in hits)
  //          {
  //              print(hit.collider.name);
  //          }

  // //         print("hit something");
		//	//Debug.DrawLine(transform.position, hit.point, Color.red, 5f);
  // //         print("drawn somthing");
		//}
	}
}
