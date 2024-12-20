using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node2D : MonoBehaviour
{
    public int ID;
    public List<Node2D> ConnectedNodes = new List<Node2D>(); //list of nodes connected to this one
    //public List<Node2D> ConnectedNodes; //list of nodes connected to this one

    public QuestionData assignedQuestion; // The question assigned to this node

    // Delegate for the OnClick event
    public delegate void NodeClickHandler();
    public event NodeClickHandler OnClick;

    //sprite changes on win/lose
    [Header("Sprites for states")]
    public Sprite neutralSprite;  // Default sprite
    public Sprite redSprite;      // Sprite for failure state
    public Sprite greenSprite;    // Sprite for success state

    public SpriteRenderer spriteRenderer;

    public bool isGreen = false;  // Track if the node is green
    public bool isRed = false;    // Track if the node is red
    public bool isAnswered = false;

    private void Awake()
    {
        // Automatically find the SpriteRenderer if not manually assigned in Inspector
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Debug.Log("SpriteRenderer automatically assigned in Awake.");
        }
    }
    private void Start()
    {
        SetNeutral();
        //Debug.Log("Node2D started. Neutral sprite set.");
    }

    // Example method to assign a question dynamically
    public void AssignQuestion(QuestionData question)
    {
        assignedQuestion = question;
    }


    public bool IsGreen()
    {
        return isGreen;
    }

    // Call this to check if the node is red
    public bool IsRed()
    {
        return isRed;
    }



    public void SetRed()
    {
        isRed = true;
        isGreen = false;  // Reset green state
        isAnswered = true; // Mark the node as answered
        spriteRenderer.sprite = redSprite;
        Debug.Log("Node2D set to Red (failure state).");

        
    }
    public void SetGreen()
    {
        isGreen = true;
        isRed = false;  // Reset red state
        isAnswered = true; // Mark the node as answered
        spriteRenderer.sprite = greenSprite;
        Debug.Log("Node2D set to Green (success state).");

       

    }

    private void SetNeutral()
    {
        spriteRenderer.sprite = neutralSprite;
        Debug.Log("Node2D set to Neutral (default state).");
    }

    // Method to reset the node to neutral state
    public void ResetState()
    {
        isGreen = false;
        isRed = false;
        isAnswered = false; // Mark the node as answered
        SetNeutral(); // Revert to the neutral state
        Debug.Log("Node2D state reset to Neutral.");
    }

    private void OnMouseDown()
    {
        // Prevent further clicks if the node has already been answered
        if (isAnswered)
        {
            Debug.Log("Node has already been answered. Cannot change.");
            return; // Early exit to prevent further clicks
        }

        

        // Trigger the OnClick event when the node is clicked
        OnClick?.Invoke();
    }

}
