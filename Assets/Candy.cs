using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour {

    public event Action onClicked;
    public Vector3Int gridPos { get; set; }
    public Color color => GetComponent<MeshRenderer>().material.color;

    private void OnMouseDown() {
        onClicked?.Invoke();
    }
}
