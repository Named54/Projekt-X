using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PathfindingSystem : MonoBehaviour
{
    public float nodeRadius = 0.1f;
    public Vector2 extendBox = new Vector2(10, 10);
    public LayerMask obstructedlayerMask;


    private PathfindingNode[,] nodes = new PathfindingNode[0, 0];

    private void Awake()
    {
        GenerateNodes();
    }

    // Generate nodes of grid
    void GenerateNodes()
    {
        nodes = new PathfindingNode[(int)(extendBox.x / nodeRadius), (int)(extendBox.y / nodeRadius)];
        for (int x = 0; x < extendBox.x / nodeRadius; x++)
        {
            for (int y = 0; y < extendBox.y / nodeRadius; y++)
            {
                PathfindingNode node = new PathfindingNode(new Vector2Int(x, y), this);
                node.Refresh();
                nodes[x, y] = node;
            }
        }
    }

    // Grid position in world space
    public Vector2 GetTransform2D()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    // Node center offset to make the origin match the center of the sphere
    public Vector2 GetNodeCenterOffset2D()
    {
        return new Vector2(nodeRadius / 2f, nodeRadius / 2f);
    }

    // Get node via grid space positon
    public PathfindingNode GetNode(Vector2Int nodePosition)
    {
        if (nodePosition.x < 0 || nodePosition.x >= nodes.GetLength(0))
            return null;

        if (nodePosition.y < 0 || nodePosition.y >= nodes.GetLength(1))
            return null;

        return nodes[nodePosition.x, nodePosition.y];
    }

    // Get node via world position
    public PathfindingNode FindNodeAtPosition(Vector2 inPosition)
    {
        Vector2 relativePosition = inPosition - GetTransform2D() - GetNodeCenterOffset2D();
        Vector2Int nodePosition = new Vector2Int(DivideAndRoundToNearest(relativePosition.x, nodeRadius), DivideAndRoundToNearest(relativePosition.y, nodeRadius));
        return GetNode(nodePosition);
    }

    private int DivideAndRoundToNearest(float dividend, float divisor)
    {
        if (dividend >= 0)
        {
            return Mathf.RoundToInt((dividend + divisor / 2f) / divisor);
        }
        else
        {
            return Mathf.RoundToInt((dividend - divisor / 2f + 1f) / divisor);
        }
    }
    private class PathElement
    {
        public PathfindingNode node;
        public PathElement from;

        public PathElement(PathfindingNode inNode, PathElement inFrom)
        {
            node = inNode;
            from = inFrom;
        }
    };

    // Calculate path
    public List<PathfindingNode> FindPath(PathfindingNode startNode, PathfindingNode endNode, int maxDepth = 1000)
    {
        if (startNode == null || endNode == null)
            return new List<PathfindingNode>();

        HashSet<PathfindingNode> visitedNodes = new HashSet<PathfindingNode>();
        List<PathElement> nodesToVisit = new List<PathElement>();
        int depth = 0;
        nodesToVisit.Add(new PathElement(startNode, null));

        PathElement foundPath = null;
        while (nodesToVisit.Count > 0)
        {
            PathElement path = nodesToVisit[0];
            visitedNodes.Add(path.node);

            if (path.node == endNode || depth > maxDepth)
            {
                foundPath = path;
                break;
            }

            PathfindingNode[] nextNodes = new PathfindingNode[]
            {
                GetNode(new Vector2Int(path.node.nodePosition.x, path.node.nodePosition.y + 1)),
                GetNode(new Vector2Int(path.node.nodePosition.x, path.node.nodePosition.y - 1)),
                GetNode(new Vector2Int(path.node.nodePosition.x + 1, path.node.nodePosition.y)),
                GetNode(new Vector2Int(path.node.nodePosition.x - 1, path.node.nodePosition.y))
            };

            Array.Sort(nextNodes, (node1, node2) =>
            {
                if (node1 == null) return -1;
                else if (node2 == null) return 1;

                float node1Distance = Vector2Int.Distance(node1.nodePosition, endNode.nodePosition);
                float node2Distance = Vector2Int.Distance(node2.nodePosition, endNode.nodePosition);

                if (node1Distance < node2Distance) return 1;
                else if (node1Distance > node2Distance) return -1;
                return 0;
            });

            for (int i = 0; i < 4; i++)
            {
                PathfindingNode nextNode = nextNodes[i];
                if (nextNode == null)
                    continue;

                if (nextNode.obstructed)
                    continue;

                if (visitedNodes.Contains(nextNode))
                    continue;

                nodesToVisit.Insert(0, new PathElement(nextNodes[i], path));
            }

            depth++;
        }


        if (foundPath == null)
            return new List<PathfindingNode>();

        List<PathfindingNode> pathList = new List<PathfindingNode>();
        while (foundPath.from != null)
        {
            pathList.Add(foundPath.node);
            foundPath = foundPath.from;
        }

        pathList.Reverse();
        //pathList.RemoveAt(0); // Remove starting node
        return pathList;
    }

    public List<PathfindingNode> OptimizePath(List<PathfindingNode> inPath)
    {
        if (inPath.Count <= 1)
            return inPath;

        List<PathfindingNode> outPath = new() { inPath[0] };
        PathfindingNode currentNode = inPath[0];
        for (int i = 1; i < inPath.Count; i++)
        {
            Vector2 direction = (inPath[i].GetTransform2D() - currentNode.GetTransform2D()).normalized;
            var result = Physics2D.Raycast(currentNode.GetTransform2D(), direction, direction.magnitude, obstructedlayerMask);
            if (result.collider)
            {
                outPath.Add(inPath[i - 1]);
                currentNode = inPath[i];
            }
        }

        outPath.Add(inPath[inPath.Count - 1]);
        return outPath;
    }

    public void VisualizePath(List<PathfindingNode> inPath)
    {
        if (inPath.Count <= 1)
            return;

        Debug.DrawRay(inPath[0].GetTransform2D(), Vector2.up, Color.blue);
        Debug.DrawRay(inPath[inPath.Count - 1].GetTransform2D(), Vector2.up, Color.red);

        for (int i = 0; i < inPath.Count - 1; i++)
        {
            Debug.DrawLine(inPath[i].GetTransform2D(), inPath[i + 1].GetTransform2D(), Color.magenta);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.1f, 0.1f, 1f, 0.2f);
        Gizmos.DrawCube(GetTransform2D() + (extendBox / 2), extendBox);

        foreach (var node in nodes)
        {
            Gizmos.color = node.obstructed ? new Color(1.0f, 0.1f, 0.1f, 0.5f) : new Color(0.1f, 1.0f, 0.1f, 0.5f);
            Gizmos.DrawSphere(node.GetTransform2D(), 0.05f);
        }
    }
}