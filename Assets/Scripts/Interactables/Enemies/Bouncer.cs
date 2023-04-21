using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField] private Vector2 dir;

    //tried to make dirs const but in C# const arrays aren't possible. Readonly should deliver the same function I want however.
    private readonly Vector2[] dirs = new Vector2[4]
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left,
	};


    // Start is called before the first frame update
    void Start()
    {
        dir = dir == null ? new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) : dir;
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)dir / 35f;
    }


    //collisions between 2 objects NEED at least one of the objects to have a rigidbody otherwise the collision wont trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //loop through the directions of the raycasts
        foreach (Vector2 castDir in dirs)
        {
            try
            {
                //get the second collider from a raycast from centre outwards
                //get second hit as first will be the gameobject itself
                RaycastHit2D hit = Physics2D.RaycastAll(transform.position, castDir, 0.6f)[1];

                //check that it something thats valid
                if (hit.transform.root.GetComponent<IIndestructable>() != null)
                {
					//draw a line to see where it hits - FOR TESTING
					Debug.DrawLine(transform.position, hit.point, Color.red, 5f);

					//if hit then decrement the dir the gameobject moves in by castDir to make it 'bounce'/'reflect'
					dir -= castDir;
				}
            }
            catch
            {
                //will be caught if theres no collision other than self so continue onwards
                continue;
            }
        }
	}
}
