using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public Node2D CurrentNode; //starting node
    public List<Node2D> allNodes; //list to store all nodes

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

        if(greenCount == allNodes.Count)
        {
            Debug.Log("You Win");
        }

        if (greenCount == allNodes.Count)
        {
            Debug.Log("Game Over");
        }
    }

    //method to manually link connected nodes using a relationship
    public void SetConnnections()
    {
        Node2D node1 = allNodes[0];
        Node2D node2 = allNodes[1];
        node1.ConnectedNodes.Add(node2);
        node2.ConnectedNodes.Add(node1);
    }

    


}
