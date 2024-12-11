using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode
{
    public bool obstructed { get; private set; }
    public Vector2Int nodePosition { get; private set; } // Position in grid space
    public PathfindingSystem owningGrid { get; private set; }


    public PathfindingNode(Vector2Int inNodePosition, PathfindingSystem inOwningGrid)
    {
        nodePosition = inNodePosition;
        owningGrid = inOwningGrid;
    }

    // Returns position in world space
    public Vector2 GetTransform2D()
    {
        return new Vector2(nodePosition.x, nodePosition.y) * owningGrid.nodeRadius + owningGrid.GetTransform2D() + owningGrid.GetNodeCenterOffset2D();
    }

    // Recalculate if node is obstructed
    public void Refresh()
    {
        obstructed = Physics2D.OverlapCircle(GetTransform2D(), owningGrid.nodeRadius, owningGrid.obstructedlayerMask) != null;
    }
}