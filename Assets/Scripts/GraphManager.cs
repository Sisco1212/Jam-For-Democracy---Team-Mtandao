using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public Node2D CurrentNode; //starting node
    public List<Node2D> allNodes; //list to store all nodes
    public List<GameObject> connections; // Store the connection GameObjects for later reference

    private int greenCount = 0;
    private int redCount = 0;

    private void Start()
    {
        //set the first random node as the starting node
        CurrentNode = allNodes[Random.Range(0, allNodes.Count)];
        TriggerTask(CurrentNode); //Trigger a task for the first node ///Add code to bring up the timed questions

        SetConnnections();
    }

    public void MoveToNode(Node2D targetNode)
    {
        if (CurrentNode.ConnectedNodes.Contains(targetNode))
        {
            CurrentNode = targetNode;
            TriggerTask(CurrentNode);
        } 
        else
        {
            Debug.Log("Invalid move! Node not connected.");
        }
    }


    //code for the win/loss condition
    //->This tracks how many green and red nodes are there
    private void TriggerTask(Node2D node)
    {
        bool taskSuccess = Random.value > 0.5; //placehodler task logic(random success)
        if(taskSuccess)
        {
            node.SetGreen(); //set the node to green on success
            greenCount++;
            Debug.Log($"Node {node.ID} turned green!");
        } 
        else
        {
            node.SetRed(); //set the node to red on failure
            redCount++;
            Debug.Log($"Node {node.ID} turned Red!");
        }

        //win condition
        if(greenCount == allNodes.Count)
        {
            Debug.Log("You Win");
        }

        //lose condition
        if (redCount == allNodes.Count)
        {
            Debug.Log("Game Over");
        }
    }

    //method to manually link connected nodes using a relationship
    public void SetConnnections()
    {
        for (int i = 0; i < allNodes.Count - 1; i++)
        {
            // Connect each node to the next
            allNodes[i].ConnectedNodes.Add(allNodes[i + 1]);
            allNodes[i + 1].ConnectedNodes.Add(allNodes[i]);

            // Draw connection and store it in the list
            GameObject lineObject = DrawConnection(allNodes[i], allNodes[i + 1]);
            connections.Add(lineObject);
        }
    }

    // Method to draw a connection between two nodes
    private GameObject DrawConnection(Node2D node1, Node2D node2)
    {
        GameObject lineObject = new GameObject($"Connection_{node1.ID}_{node2.ID}");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Configure LineRenderer properties
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        // Set the positions to the nodes' positions
        lineRenderer.SetPosition(0, node1.transform.position);
        lineRenderer.SetPosition(1, node2.transform.position);

        // Set default color (gray for neutral state)
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.gray;
        lineRenderer.endColor = Color.gray;

        return lineObject;
    }

    private void UpdateConnectionColors()
    {
        foreach (var connection in connections)
        {
            LineRenderer lineRenderer = connection.GetComponent<LineRenderer>();
            Node2D node1 = allNodes[connection.GetInstanceID() % allNodes.Count];
            Node2D node2 = allNodes[(connection.GetInstanceID() + 1) % allNodes.Count];

            // Check node colors and update connection line colors
            if (node1.spriteRenderer.color == Color.green || node2.spriteRenderer.color == Color.green)
            {
                lineRenderer.startColor = Color.green;
                lineRenderer.endColor = Color.green;
            }
            else if (node1.spriteRenderer.color == Color.red || node2.spriteRenderer.color == Color.red)
            {
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
            }
            else
            {
                lineRenderer.startColor = Color.gray;
                lineRenderer.endColor = Color.gray;
            }
        }
    }


}
