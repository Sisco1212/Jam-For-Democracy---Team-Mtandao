using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node2D : MonoBehaviour
{
    public int ID;
    public List<Node2D> ConnectedNodes; //list of nodes connected to this one

    //sprite changes on win/lose
    [Header("Sprites for states")]
    public Sprite neutralSprite;  // Default sprite
    public Sprite redSprite;      // Sprite for failure state
    public Sprite greenSprite;    // Sprite for success state

    public SpriteRenderer spriteRenderer;

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
        spriteRenderer.sprite = redSprite;
    }
    public void SetGreen()
    {
        spriteRenderer.sprite = greenSprite;
    }

    private void SetNeutral()
    {
        spriteRenderer.sprite = neutralSprite;
    }


}
