using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private static readonly Bounds WorldBounds = new(Vector2.zero, new Vector2(2, 2));
    
    private void OnDrawGizmos()
    {
        const int rows = 2; // How many cells the grid have vertically
        const int columns = 2; // How many cells the grid have horizontally
        
        var mouseScreenPos = Input.mousePosition;
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Gizmos.DrawSphere(mouseWorldPos, 0.1f);
        
        var dir = (Vector2)(mouseWorldPos - WorldBounds.min);
        dir.x /= WorldBounds.size.x;
        dir.y /= WorldBounds.size.y;

        dir.x *= columns;
        dir.y *= rows;
        
        dir.x = (int)Math.Floor(dir.x);
        dir.y = (int)Math.Floor(dir.y);
        
        Debug.LogError(dir);
    }
}