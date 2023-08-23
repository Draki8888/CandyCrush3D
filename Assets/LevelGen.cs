using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGen : MonoBehaviour
{
    [SerializeField] private Ball ballPrefab;

    private readonly Dictionary<Vector3Int, Ball> ballsDict = new();
    private Ball lastClickedBall;
    private const int CubeSize = 10;


    private Compass compass;

    private const float Eps = 0.01f;

    void Start()
    {
        compass = new Compass(Vector3Int.forward, CubeSize);

        List<Color> colorList = new List<Color>
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            new(1f, 0.09f, 0.84f)
        };
        var gridPositions = GetGridPositions().ToList();
        Debug.Assert(gridPositions.Count == gridPositions.Distinct().Count());
        Debug.Log(string.Join(",", gridPositions.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key).ToList()));
        foreach (var gridPosition in GetGridPositions())
        {
            Vector3 position = new Vector3(gridPosition.x, gridPosition.y, gridPosition.z);
            var ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<MeshRenderer>().material.color = colorList[Random.Range(0, colorList.Count)];
            ball.OnClicked += () => OnBallClicked(ball);
            ballsDict[gridPosition] = ball;
            ball.GridPosition = gridPosition;
        }
    }


    private IEnumerable<Vector3Int> GetGridPositions()
    {
        for (int i = 0; i < CubeSize; i++)
        {
            for (int j = 0; j < CubeSize; j++)
            {
                // Top 
                yield return new Vector3Int(i, CubeSize - 1, j);
                // Bottom
                yield return new Vector3Int(i, 0, j);
            }
        }

        for (int i = 1; i < CubeSize - 1; i++)
        {
            for (int j = 0; j < CubeSize; j++)
            {
                // Left
                yield return new Vector3Int(0, i, j);
                // Right
                yield return new Vector3Int(CubeSize - 1, i, j);
            }
        }

        for (int i = 1; i < CubeSize - 1; i++)
        {
            for (int j = 1; j < CubeSize - 1; j++)
            {
                // Front
                yield return new Vector3Int(i, j, 0);
                // Back
                yield return new Vector3Int(i, j, CubeSize - 1);
            }
        }
    }

    private void ApplyGravity()
    {
        var hangingBalls = GetAllHangingBalls();
        foreach (var hangingBall in hangingBalls)
        {
            foreach (Vector3Int position in compass.IterateColumnAbove(hangingBall.GridPosition))
            {
                var ball = ballsDict[position];
                if(ball != null)
                {
                    ball.Color = Color.black;
                }
            }

            var groundBall = GetFirstBallBelow(hangingBall);
        }
    }

    private Ball GetFirstBallBelow(Ball hangingBall)
    {
        return null;
    }


    private IEnumerable<Ball> GetAllHangingBalls()
    {
        foreach (var (pos, c) in Balls)
        {
            var posUnder = pos + compass.GravityAxis;
            if (ballsDict.ContainsKey(posUnder) && ballsDict[posUnder] == null)
            {
                yield return c;
            }
        }
        
    }


    private IEnumerable<KeyValuePair<Vector3Int, Ball>> Balls => ballsDict
        .Where(p => p.Value != null);

    private void OnBallClicked(Ball ball)
    {
        if (lastClickedBall != null &&
            CaculateHammingDistance(lastClickedBall.transform.position, ball.transform.position) == 1)
        {
            SwitchPlaces(lastClickedBall, ball, () =>
            {
                var connectedRegion = GetConnectedRegion(ball);
                if (connectedRegion.Count > 2)
                {
                    foreach (var c in connectedRegion)
                    {
                        DestroyBall(c);
                    }
                }

                var enumerable = GetConnectedRegion(lastClickedBall);
                if (enumerable.Count > 2)
                {
                    foreach (var c in enumerable)
                    {
                        DestroyBall(c);
                    }
                }

                ApplyGravity();
                lastClickedBall = null;
            });
        }
        else
        {
            lastClickedBall = ball;
        }
    }

    private void DestroyBall(Ball c)
    {
        Destroy(c.gameObject);
        ballsDict[c.GridPosition] = null;
    }

    private void SwitchPlaces(Ball ball1, Ball ball2, Action onAnimationFinish)
    {
        var g1 = ball1.GridPosition;
        var g2 = ball2.GridPosition;
        ball1.GridPosition = g2;
        ball2.GridPosition = g1;
        ballsDict[g1] = ball2;
        ballsDict[g2] = ball1;
        ball1.transform.LeanMove(ball2.transform.position, .2f).setEaseOutCubic();
        ball2.transform.LeanMove(ball1.transform.position, .2f).setEaseOutCubic();
        LeanTween.tweenEmpty.LeanValue(0, 1, 0.2f + Eps).setOnComplete(onAnimationFinish);
    }

    private int CaculateHammingDistance(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.RoundToInt(Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y) + Mathf.Abs(pos1.z - pos2.z));
    }

    private List<Ball> GetConnectedRegion(Ball startingBall)
    {
        Stack<Ball> candies = new Stack<Ball>();
        candies.Push(startingBall);
        List<Ball> region = new List<Ball>();
        while (candies.Count > 0)
        {
            var c = candies.Pop();
            region.Add(c);
            foreach (var neighbor in GetNeighbors(c).Where(x => x.Color == c.Color))
            {
                if (!region.Contains(neighbor))
                {
                    candies.Push(neighbor);
                }
            }
        }

        return region;
    }

    private List<Ball> GetNeighbors(Ball ball)
    {
        var pos = ball.GridPosition;
        List<Ball> neighbors = new List<Ball>();
        var dirs = new List<Vector3Int>
        {
            Vector3Int.right,
            Vector3Int.left,
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.forward,
            Vector3Int.back
        };
        foreach (var dir in dirs)
        {
            if (ballsDict.TryGetValue(pos + dir, out Ball neighbor) && neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}