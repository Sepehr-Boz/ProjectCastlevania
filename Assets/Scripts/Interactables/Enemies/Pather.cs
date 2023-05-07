using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Pather : MonoBehaviour, IDamageable
{
    [Header("HP")]
    public int hp;
    public int maxHP;


    [Header("Movement")]
    public float moveSpeed = 1f;
    public float checkInterval = 5f;

    protected Vector2 start;
    protected Vector2 target;
    protected PathNode path;
    private readonly Vector2[] directions = new Vector2[8]
    {
        new Vector2(-1, 1),  new Vector2(0, 1),  new Vector2(1, 1), 
        new Vector2(-1, 0),                      new Vector2(1, 0),
		new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1),
	};


    protected void Start()
    {
        hp = maxHP;

        InvokeRepeating(nameof(FindPath), 1f, checkInterval);
    }

    protected void Update()
    {
        if (hp <= 0)
        {
            //get and spawn coin from coinpool
            GameObject tmp = CoinPool.Instance.GetPooledObject();
            tmp.transform.position = transform.position;


            //if hp is less than or 0 destroy gameobject
            Destroy(gameObject);
        }

        //find path and move along it
        try{
            if ((Vector2)transform.position == target)
            {
                path = (PathNode)path.next;
                target = path.val;
            }
        }catch{}
        if (target != Vector2.zero)
        {
			GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude((target - start), moveSpeed);
		}
    }


    /// <summary>
    /// FindPath() and GeneratePath() are made public virtual so they can be inherited by childrent on EnemyController but also overriden even though they are complete methods
    /// so they can be used as is or overriden
    /// </summary>
    public void FindPath()
    {
        start = (Vector2)transform.position;
        //check if player is in bounds
        //if not then target is a random position inside the room

        //get the room gameobject that the enemy is in
        //check if theres a player gameobject within the bounds of the room
        GameObject room = transform.parent.gameObject;
        Collider2D[] overlap = Physics2D.OverlapBoxAll((Vector2)room.transform.position, new Vector2(5, 5), 0f);


        //////FOR TESTING TO SEE THE AREA WHICH IS CHECKED
        //Debug debug = new();
        //debug.DrawBox((Vector2)room.transform.position, new Vector2(5, 5));
        //////// BREAKS PATHING FOR SOME REASON DUNNO WHY BRUH

        foreach (var hit in overlap)
        {
            if (hit.GetComponent<PlayerController>())
            {
                target = (Vector2)hit.transform.position;
            }
        }
        try
        {
			path = GeneratePath(start, target);
		}catch{}
	}

	public PathNode GeneratePath(Vector2 start, Vector2 end)
    {
        start = new Vector2(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));
        end = new Vector2(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y));

        Queue<PathNode> active = new();
        active.Enqueue(new PathNode(start, 0, null, null));
        List<PathNode> traversed = new();

        //repeat while there are nodes in active
        while (active.Count > 0)
        {
            print(active.Count);
            //dequeu a node from active
            PathNode node = active.Dequeue();
            //check if the node is in traversed
            //loop through traversed instead of .Contains() as the nodes wont be completely similar, the similarity to check for is the position not the cost or previous
            for (int i = 0; i < traversed.Count; i++)
            {
                var n = traversed[i];
                //if at the same position then compare the costs to get to that distance and reassign the value in traversed to the shorter cost
                if (n.val == node.val && n.cost > node.cost)
                {
                    traversed[i] = node;
                    continue;
                }
            }

            //if a node has reached the end position then dont continue to any neighbouring nodes
            //and stop the pathing algorithm as it will be quick enough for the path
            //continuing made me see some lag issues as with multiple enemies it creates a lag spike as each enemy searches the entire room to find a path so just return the fastest path not the shortest
            if (node.val == end)
            {
                break;
            }

            if (active.Count < 50)
            {
				//get the possible traversal nodes and add them to active
				foreach (Vector2 dir in directions)
				{
					//cost += 10 if straight and += 15 if diagonal
					//direction is straight if one of the axis has a 0

					//check if the new direction is valid
					//direction is valid if theres no collider at the point
					Vector2 newPos = node.val + dir;
					var hits = Physics2D.OverlapBoxAll(newPos, Vector2.one, 0f);
					bool valid = true;
					foreach (var hit in hits)
					{
						if (hit.GetComponent<Collider2D>())
						{
							valid = false;
						}
					}

					if (!valid)
					{
						continue;
					}

					int newCost = node.cost;
					if (dir.x == 0 || dir.y == 0)
					{
						newCost += 10;
					}
					else
					{
						newCost += 15;
					}

					PathNode newNode = new(node.val + dir, newCost, node, null);
					if (newCost > 50)
					{
						traversed.Add(newNode);
						continue;
					}

					active.Enqueue(newNode);
				}
				traversed.Add(node);
			}

        }

        //traverse back through the last node in traversed, and while doing so add the current to the next value of the previous node
        //in C#, the equivalent of -x is ^x
        PathNode endNode = traversed[^1];
        //loop back through endnode until start has been reached
        while (endNode.previous != null)
        {
            //first add the current node to the next of the previous
            endNode.previous.next = endNode;
            //then iterate to the previous one
            endNode = endNode.previous;
        }
        //return endnode as path
        return endNode;
    }


    public void Damage(int damage)
    {
        hp -= damage;
    }

}


[System.Serializable]
public class ListNode<T>
{
	public T val; //T has to be specified in the class name, but when calling it the T has to specified
	public ListNode<T> next;

	public ListNode(T value, ListNode<T> next)
	{
		this.val = value;
		this.next = next;
	}
}


[System.Serializable]
public class PathNode : ListNode<Vector2>
{
    public int cost;
    public PathNode previous;
    public new PathNode next;

    public PathNode(Vector2 pos, int newCost, PathNode prev, PathNode next) : base(pos, next)
    {
        this.cost = newCost;
        this.previous = prev;

        this.next = next;
    }
}
