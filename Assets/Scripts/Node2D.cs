using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node2D : MonoBehaviour
{
    public int ID;
    public List<Node2D> ConnectedNodes; //list of nodes connected to this one
    public SpriteRenderer spriteRenderer;
    public Color RedColor;
    public Color GreenColor;
    public Color NeutralColor;

    private void Awake()
    {
        // Automatically find the SpriteRenderer if not manually assigned in Inspector
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
    private void Start()
    {
        SetNeutral();
    }
    public void SetRed()
    {
        spriteRenderer.color = RedColor;
    }
    public void SetGreen()
    {
        spriteRenderer.color = GreenColor;
    }

    private void SetNeutral()
    {
        spriteRenderer.color = NeutralColor;
    }
}
