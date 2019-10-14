using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSeb : MonoBehaviour
{
    /// drag on GameManger. Also needs Pathfinding and PathRequestManager scripts!
    /// this works by finding all colliders on the collidersForMap layer and excluding them from the walkable area.
    /// so every non-moving GO that you need to be accounted for in that map needs to be on the layer CollidersForMap
    /// (specifically, the GO in the hierarchy that has the collider needs to be on that layer 

    public LayerMask collidersForMap; 
    public bool displayGridGizmos;

    public Vector2 gridWorldSize;
    public float nodeRadius; //.5 suggested

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    Node[,] grid;
    //public List<Vector3> walkableWorldPointsOfGrid; //didn't end up needing this YET; see also end of CreateGrid

    void Start() //this needs to be in start because of a conflict with Pathfinding? or PathRequestManager script
    {
        nodeDiameter = nodeRadius * 2;

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); //these 2 left to what you set in inspector in this case
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    public int MaxSize  { get { return gridSizeX * gridSizeY; } }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2; 
        //Original= Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        { 
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                //Original= Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool walkable = true; //eveyrthing walkable by default

                if (Physics2D.OverlapCircle(worldPoint, nodeRadius, collidersForMap) != null) //if we find anything to collide with that's on our mentioned layer, there's an obstacle there, 
                     walkable = false; //so mark it unwalkable. 
                //(last 3 lines are mine; Original= bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                grid[x, y] = new Node(walkable, worldPoint, x, y);

                //if (walkable) //see at top
                    //walkableWorldPointsOfGrid.Add(worldPoint);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) //seraches in a 3x3 block around the node (-1, 0, 1). Center is zero. 
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) //this means we're at the center, = ourself, not our neighbors
                    continue;           //... so skip it

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX); //arrays are zero based so minus 1
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY); //arrays are zero based so minus 1

        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .05f));
            }
        }
    }
}
