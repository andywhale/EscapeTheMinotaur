using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform startPoint; // Starting point
    public Transform endPoint; // Ending point
    [SerializeField] public Dictionary<Vector2, Tile> grid = new Dictionary<Vector2, Tile>(); // Grid of Tile game objects

    public void SetGrid(Dictionary<Vector2, Tile> grid)
    {
        this.grid = grid;
    }

    public List<Tile> FindPath(Transform startPoint, Transform endPoint)
    {
        // Initialize open and closed sets
        HashSet<Tile> openSet = new HashSet<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();

        // Clear parent and cost parameters for all tiles in the grid
        grid.Values.ToList().ForEach(tile => tile.Reset());

        closedSet.Clear();

        // Add starting tile to open set
        openSet.Add(grid[startPoint.position]);

        while (openSet.Count > 0)
        {
            Tile currentTile = null;

            // Find tile with lowest F cost in open set
            foreach (Tile tile in openSet)
            {
                if (currentTile == null || tile.FCost < currentTile.FCost)
                {
                    currentTile = tile;
                }
            }

            // Move current tile from open set to closed set
            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            // Reached the destination
            if (currentTile == grid[endPoint.position])
            {
                return GeneratePath(grid[startPoint.position], currentTile);
            }

            // Explore neighboring tiles
            foreach (Tile neighbor in currentTile.GetNeighbours())
            {
                // Skip if already evaluated or not walkable
                if (closedSet.Contains(neighbor) || !neighbor.Routable)
                {
                    continue;
                }

                // Calculate G and H cost for the neighbor
                int moveCost = currentTile.GCost + CalculateDistance(currentTile, neighbor);
                if (moveCost < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = moveCost;
                    neighbor.HCost = CalculateDistance(neighbor, grid[endPoint.position]);
                    neighbor.parent = currentTile;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // Path not found
        return null;
    }

    private List<Tile> GeneratePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }

        path.Reverse(); // Reverse the path to get it in correct order
        return path;
    }

    private int CalculateDistance(Tile tileA, Tile tileB)
    {
        int distX = Mathf.Abs((int)(tileA.transform.position.x - tileB.transform.position.x));
        int distY = Mathf.Abs((int)(tileA.transform.position.y - tileB.transform.position.y));
        return distX + distY;
    }
}
