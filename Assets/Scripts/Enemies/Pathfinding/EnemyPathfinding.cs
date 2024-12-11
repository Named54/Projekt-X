using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    public Vector2 target;
    public int searchDepth = 10;
    public Vector2 originOffset;
    public float updateTime = 2.0f; // in seconds
    private float updateCooldown = 0f;
    private List<PathfindingNode> path;
    private List<Route> routes = new List<Route>();
    private int currentRouteIndex = 0;
    private bool isPlayerDetected = false;


    public List<PathfindingNode> GetPath()
    {
        return path;
    }

    private void Start()
    {
        // Initialize routes
        InitializeRoutes();
    }

    void Update()
    {
        updateCooldown -= Time.deltaTime;
        if (updateCooldown > 0f)
            return;
        updateCooldown = updateTime;

        // Check for player detection
        DetectPlayer(transform.position);

        if (isPlayerDetected)
        {
            // Chase the player
            ChasePlayer();
        }
        else
        {
            // Follow the current route
            FollowCurrentRoute();
        }
    }

    private void FollowCurrentRoute()
    {
        PathfindingSystem pathGrid = FindFirstObjectByType<PathfindingSystem>();
        PathfindingNode start = pathGrid.FindNodeAtPosition((Vector2)transform.position + originOffset);
        PathfindingNode end = pathGrid.FindNodeAtPosition(routes[currentRouteIndex].endPoint);
        List<PathfindingNode> fullPath = pathGrid.FindPath(start, end, searchDepth);
        path = pathGrid.OptimizePath(fullPath);
    }

    private void ChasePlayer()
    {
        PathfindingSystem pathGrid = FindFirstObjectByType<PathfindingSystem>();
        PathfindingNode start = pathGrid.FindNodeAtPosition((Vector2)transform.position + originOffset);
        PathfindingNode end = pathGrid.FindNodeAtPosition((Vector2)target);
        List<PathfindingNode> fullPath = pathGrid.FindPath(start, end, searchDepth);
        path = pathGrid.OptimizePath(fullPath);
    }

    private void DetectPlayer(Vector2 enemyPosition)
    {
        // Implement player detection logic here
        // Set isPlayerDetected flag accordingly
    }

    private void InitializeRoutes()
    {
        // Define your routes here
        routes.Add(new Route
        {
            startPoint = new Vector2(0, 0),
            endPoint = new Vector2(10, 10),
            priority = 1,
            obstacles = new List<Vector2>()
        });

        routes.Add(new Route
        {
            startPoint = new Vector2(0, 10),
            endPoint = new Vector2(10, 0),
            priority = 2,
            obstacles = new List<Vector2>() { new Vector2(5, 5) }
        });
    }

    private void ChangeRoute(int routeIndex)
    {
        currentRouteIndex = routeIndex;
    }
}

public class Route
{
    public Vector2 startPoint;
    public Vector2 endPoint;
    public int priority;
    public List<Vector2> obstacles;
}
