using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class CompassTest
{
    private const int CubeSize = 10;

    [TestCaseSource(nameof(GetTopCases))]
    public void TestGetTop(Vector3Int gravity, Vector3Int position, Vector3Int expectedTop)
    {
        var compass = new Compass(gravity, CubeSize);
        var actualTop = compass.GetTop(position);

        Assert.AreEqual(expectedTop, actualTop);
    }
    
    [TestCaseSource(nameof(GetBottomCases))]
    public void TestGetBottom(Vector3Int gravity, Vector3Int position, Vector3Int expectedTop)
    {
        var compass = new Compass(gravity, CubeSize);
        var actualTop = compass.GetBottom(position);

        Assert.AreEqual(expectedTop, actualTop);
    }

    private static object[] GetTopCases =
    {
        new object[] { Vector3Int.right, new Vector3Int(2, 5, 8), new Vector3Int(0, 5, 8) },
        new object[] { Vector3Int.down, new Vector3Int(3, 2, 7), new Vector3Int(3, CubeSize - 1, 7) },
        new object[] { Vector3Int.up, new Vector3Int(5, 1, 2), new Vector3Int(5, 0, 2) },
            new object[] { Vector3Int.left, new Vector3Int(7, 2, 5), new Vector3Int(CubeSize - 1, 2, 5) },
            new object[] { Vector3Int.forward, new Vector3Int(8, 8, 8), new Vector3Int(8, 8, 0) },
            new object[] { Vector3Int.back, new Vector3Int(5, 1, 0), new Vector3Int(5, 1, CubeSize - 1) },
    };
    
    private static object[] GetBottomCases =
    {
        new object[] { Vector3Int.right, new Vector3Int(2, 5, 8), new Vector3Int(CubeSize - 1, 5, 8) },
            new object[] { Vector3Int.down, new Vector3Int(3, 2, 7), new Vector3Int(3, 0, 7) },
            new object[] { Vector3Int.up, new Vector3Int(5, 1, 2), new Vector3Int(5, CubeSize - 1, 2) },
            new object[] { Vector3Int.left, new Vector3Int(7, 2, 5), new Vector3Int(0, 2, 5) },
            new object[] { Vector3Int.forward, new Vector3Int(8, 8, 8), new Vector3Int(8, 8, CubeSize - 1) },
            new object[] { Vector3Int.back, new Vector3Int(5, 1, 0), new Vector3Int(5, 1, 0) },
    };
}
