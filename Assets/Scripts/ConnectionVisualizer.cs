using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionVisualizer : MonoBehaviour
{
    public GameObject connectionPrefab; // Drag your line sprite prefab here
    private GameObject currentConnection;


    public GameObject greenConnectionPrefab; // Assign a green connection sprite
    public GameObject redConnectionPrefab;   // Assign a red connection sprite


    public void CreateConnection(Vector3 startPos, Vector3 endPos, Node2D targetNode)
    {
        // If there's an existing connection, destroy it
        if (currentConnection != null)
        {
            Destroy(currentConnection);
        }

        // Instantiate a new connection between the two nodes
        currentConnection = Instantiate(connectionPrefab, startPos, Quaternion.identity);

        // Calculate the distance and set the scale of the line
        float distance = Vector3.Distance(startPos, endPos);
        currentConnection.transform.localScale = new Vector3(distance, 1f, 1f); // Adjust the scale for length

        // Rotate the connection to point towards the end position
        Vector3 direction = endPos - startPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentConnection.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Change the sprite based on the target node's state
        ChangeConnectionSprite(targetNode);
    }

    // Method to change the sprite of the connection based on the target node's state
    private void ChangeConnectionSprite(Node2D targetNode)
    {
        SpriteRenderer spriteRenderer = currentConnection.GetComponent<SpriteRenderer>();

        if (targetNode.IsGreen()) // Check if the node is green
        {
            //spriteRenderer.sprite = greenConnectionPrefab; // Set the green connection sprite
        }
        else if (targetNode.IsRed()) // Check if the node is red
        {
            //spriteRenderer.prefa = greenConnectionPrefab; // Set the red connection sprite
        }
        else
        {
            // Set a default or neutral sprite if necessary
            spriteRenderer.sprite = null; // Or a default sprite if needed
        }
    }
}
