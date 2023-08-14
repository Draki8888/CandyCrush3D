using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGen : MonoBehaviour {
    [SerializeField] private Candy candyPrefab;
    private Candy lastClickedCandy;
    private Dictionary<Vector3Int, Candy> candiesDict = new();
    const int cubeSize = 10;
    private Vector3Int localGravity = Vector3Int.down;

    void Start() {
        List<Color> colorList = new List<Color> {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            new Color(1f, 0.09f, 0.84f)
        };
        var gridPositions = GetGridPositions().ToList();
        Debug.Assert(gridPositions.Count == gridPositions.Distinct().Count());
        Debug.Log(string.Join(",", gridPositions.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key).ToList()));
        foreach (var gridPosition in GetGridPositions()) {
            Vector3 position = new Vector3(gridPosition.x, gridPosition.y, gridPosition.z);
            var candy = Instantiate(candyPrefab, position, quaternion.identity);
            candy.GetComponent<MeshRenderer>().material.color = colorList[Random.Range(0, colorList.Count)];
            candy.onClicked += (() => OnCandyClicked(candy));
            candiesDict[gridPosition] = candy;
            candy.gridPos = gridPosition;
        }
    }


    private IEnumerable<Vector3Int> GetGridPositions() {
        for (int i = 0; i < cubeSize; i++) {
            for (int j = 0; j < cubeSize; j++) {
                // Top 
                yield return new Vector3Int(i, cubeSize - 1, j);
                // Bottom
                yield return new Vector3Int(i, 0, j);
            }
        }

        for (int i = 1; i < cubeSize - 1; i++) {
            for (int j = 0; j < cubeSize; j++) {
                // Left
                yield return new Vector3Int(0, i, j);
                // Right
                yield return new Vector3Int(cubeSize - 1, i, j);
            }
        }

        for (int i = 1; i < cubeSize - 1; i++) {
            for (int j = 1; j < cubeSize - 1; j++) {
                // Front
                yield return new Vector3Int(i, j, 0);
                // Back
                yield return new Vector3Int(i, j, cubeSize - 1);
            }
        }
    }

    private void ApplyGravity() {
        var hangingBalls = GetAllHangingBalls();
    }

    private List<Candy> GetAllHangingBalls() {
        foreach (var (pos, c) in Candies) {
            var posUnder = pos + localGravity;
            if (candiesDict.ContainsKey(posUnder) && candiesDict[posUnder] == null) {
                c.transform.localScale = Vector3.one * .5f;
            }
        }

        return new List<Candy>();
    }


    private IEnumerable<KeyValuePair<Vector3Int, Candy>> Candies => candiesDict
        .Where(p => p.Value != null);

    private void OnCandyClicked(Candy candy) {
        if (lastClickedCandy != null &&
            CaculateHammingDistance(lastClickedCandy.transform.position, candy.transform.position) == 1) {
            SwitchPlaces(lastClickedCandy, candy, () => {
                var connectedRegion = GetConnectedRegion(candy);
                if (connectedRegion.Count > 2) {
                    foreach (var c in connectedRegion) {
                        DestroyCandy(c);
                    }
                }

                var enumerable = GetConnectedRegion(lastClickedCandy);
                if (enumerable.Count > 2) {
                    foreach (var c in enumerable) {
                        DestroyCandy(c);
                    }
                }

                ApplyGravity();
                lastClickedCandy = null;
            });
        }
        else {
            lastClickedCandy = candy;
        }
    }

    private void DestroyCandy(Candy c) {
        Destroy(c.gameObject);
        candiesDict[c.gridPos] = null;
    }

    private void SwitchPlaces(Candy candy1, Candy candy2, Action onAnimationFinish) {
        var g1 = candy1.gridPos;
        var g2 = candy2.gridPos;
        candy1.gridPos = g2;
        candy2.gridPos = g1;
        candiesDict[g1] = candy2;
        candiesDict[g2] = candy1;
        LTSeq sequence = LeanTween.sequence();
        sequence.insert(candy1.transform.LeanMove(candy2.transform.position, .2f).setEaseOutCubic()
            .setOnComplete(onAnimationFinish));
        sequence.insert(candy2.transform.LeanMove(candy1.transform.position, .2f).setEaseOutCubic());
        sequence.append(onAnimationFinish);
    }

    private int CaculateHammingDistance(Vector3 pos1, Vector3 pos2) {
        return Mathf.RoundToInt(Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y) + Mathf.Abs(pos1.z - pos2.z));
    }

    private List<Candy> GetConnectedRegion(Candy startingCandy) {
        Stack<Candy> candies = new Stack<Candy>();
        candies.Push(startingCandy);
        List<Candy> region = new List<Candy>();
        while (candies.Count > 0) {
            var c = candies.Pop();
            region.Add(c);
            foreach (var neighbor in GetNeighbors(c).Where(x => x.color == c.color)) {
                if (!region.Contains(neighbor)) {
                    candies.Push(neighbor);
                }
            }
        }

        return region;
    }

    private List<Candy> GetNeighbors(Candy candy) {
        var pos = candy.gridPos;
        List<Candy> neighbors = new List<Candy>();
        var dirs = new List<Vector3Int> {
            Vector3Int.right,
            Vector3Int.left,
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.forward,
            Vector3Int.back,
        };
        foreach (var dir in dirs) {
            if (candiesDict.TryGetValue(pos + dir, out Candy neighbor) && neighbor != null) {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}