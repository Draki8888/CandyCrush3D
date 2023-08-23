using System.Collections.Generic;
using UnityEngine;

public class Compass
{
    public int CubeSize {get; }
    public Vector3Int GravityAxis {get; }
    
    public Compass(Vector3Int gravityAxis, int cubeSize) {       
        GravityAxis = gravityAxis;
        CubeSize = cubeSize;
    }
    
    public Vector3Int GetBottom(Vector3Int position) =>
        GetPositionWithValueOnGravityAxis(position, GravitySign > 0 ? CubeSize - 1 : 0);
    
    public Vector3Int GetTop(Vector3Int position) =>
        GetPositionWithValueOnGravityAxis(position, GravitySign > 0 ? 0 : CubeSize - 1);
    
    private Vector3Int GetPositionWithValueOnGravityAxis(Vector3Int position, int value) {
        var distance = value - Mathf.Abs(Sum(position * GravityAxis));
        return position + distance * (GravitySign * GravityAxis);
    }
    
    private int GravitySign => Sum(GravityAxis);
    private int Sum(Vector3Int vector) => vector.x + vector.y + vector.z;
    
    public IEnumerable<Vector3Int> IterateColumnAbove(Vector3Int start) =>
        IterateOrientedColumn(start, GetTop(start));
    
    public IEnumerable<Vector3Int> IterateColumnBelow(Vector3Int start) =>
        IterateOrientedColumn(start, GetBottom(start));
    
    private IEnumerable<Vector3Int> IterateOrientedColumn(Vector3Int start, Vector3Int end)
    {
        if (start == end) {
            yield return start;
            yield break;
        }
        var direction = (end - start) / Sum(end - start); 
        
        var current = start;
        yield return current;
        while(current != end)
        {
            current += direction;
            yield return current;
        }
    }
}