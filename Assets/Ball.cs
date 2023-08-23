using System;
using UnityEngine;

public class Ball : MonoBehaviour {

    public event Action OnClicked;
    public Vector3Int GridPosition { get; set; }
    private MeshRenderer meshRenderer;
    public Color Color
    {
        get => meshRenderer.material.color;
        set => meshRenderer.material.color = value;
    }

    private void OnMouseDown() {
        OnClicked?.Invoke();
    }

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
}
