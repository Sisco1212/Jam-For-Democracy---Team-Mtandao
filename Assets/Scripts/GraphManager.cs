using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    public Node2D CurrentNode; // Starting node
    public List<Node2D> allNodes; // List to store all nodes

    public List<GameObject> connections; // Store the connection GameObjects for later reference
    public GameObject connectionPrefab; // Reference to the connection sprite prefab

    public GameObject glowingCirclePrefab; //reference to the glowing particle prefab
    private GameObject activeParticleEffect; // to store the current particle effect


    public GameObject messageUI; // Reference to the Message UI Panel
    public TMP_Text messageText; // Text for the message
    public Button optionAButton; // Button for Option A
    public Button optionBButton; // Button for Option B
    public TMP_Text explanationText; // Optional: Text for explanation

    private Coroutine timerCoroutine; // Reference to the timer coroutine


    private int greenCount = 0;
    private int redCount = 0;

    private void Start()
    {
        // Find the node with a specific ID or randomly select one
        int targetID = 0;
        CurrentNode = allNodes.Find(node => node.ID == targetID);
        if (CurrentNode != null)
        {
            Debug.Log($"Starting node set to {CurrentNode.ID} (found by ID)");
        }
        else
        {
            Debug.LogWarning($"Node with ID {targetID} not found! Defaulting to a random node.");
            CurrentNode = allNodes[Random.Range(0, allNodes.Count)];
        }

        TriggerTask(CurrentNode); // Trigger task on starting node
        HighlightNode(CurrentNode); // Highlight the active node with the particle effect on start
        SetConnections(); // Set the connections for the nodes (visualize them)

        //toogle messages UI visibility off on start
        messageUI.SetActive(false); 

<<<<<<< Updated upstream
=======
        //add listener to the starting node to trigger ShowMessages when clicked
        //CurrentNode.OnClick += () => ShowMessageUI(CurrentNode);
        CurrentNode.OnClick += () => OnFirstNodeClick(CurrentNode);

    }



    private void OnFirstNodeClick(Node2D node)
    {
        // Check if the timer has already been started
        if (!isTimerStarted)
        {
            // Start the timer only once
            isTimerStarted = true;
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine); // Stop any existing timers
            }
            timerCoroutine = StartCoroutine(AnswerTimer(node, levelData.timerDuration)); // Start the timer for the selected node
        }

        // Now show the message UI and proceed with the usual flow
        ShowMessageUI(node);
>>>>>>> Stashed changes
    }

    // This method will create and visualize the connections between the nodes
    public void SetConnections()
    {
        foreach (Node2D node in allNodes)
        {
            Debug.Log($"Checking connections for Node {node.ID}.");
            // Loop through each node's connected nodes
            foreach (Node2D connectedNode in node.ConnectedNodes)
            {
                Debug.Log($"Creating connection from Node {node.ID} to Node {connectedNode.ID}.");
                CreateConnection(node, connectedNode); // Create the connection between node and connectedNode
            }
        }
    }

    // This method instantiates a connection (sprite/line) between two nodes
    public void CreateConnection(Node2D node1, Node2D node2)
    {
        Debug.Log($"Creating connection between Node {node1.ID} and Node {node2.ID}");

        // Get the position of both nodes
        Vector3 startPos = node1.transform.position;
        Vector3 endPos = node2.transform.position;

        // Instantiate the connection sprite between the two nodes
        GameObject connection = Instantiate(connectionPrefab, startPos, Quaternion.identity);
        connection.transform.SetParent(transform); // Attach it to the GraphManager for easy management

        // Calculate the distance and scale the connection accordingly
        float distance = Vector3.Distance(startPos, endPos);
        float width = 0.1f; // Set your desired width here
        connection.transform.localScale = new Vector3(distance, width, 1f);

        // Rotate the connection to face from node1 to node2
        Vector3 direction = endPos - startPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        connection.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Position the connection to be at the correct midpoint between the nodes
        Vector3 midPoint = (startPos + endPos) / 2;
        connection.transform.position = midPoint;

        // Get the SpriteRenderer component from the instantiated connection
        SpriteRenderer connectionRenderer = connection.GetComponent<SpriteRenderer>();

        // Determine the color based on the target node (node2)
        string hexColor = GetConnectionColor(node2);

        // Parse the hex color string and set it to the connection
        if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
        {
            connectionRenderer.color = newColor;
            Debug.Log($"Connection color set to {newColor}");
        }
        else
        {
            Debug.LogError("Invalid hex color code: " + hexColor);
        }

        // Add the connection to the list for later management
        connections.Add(connection);
        Debug.Log($"Connection from Node {node1.ID} to Node {node2.ID} created and added to the list.");
    }

    // Helper method to return the color based on the node's state
    private string GetConnectionColor(Node2D targetNode)
    {
        // Check the state of the target node (connected node)
        if (targetNode.IsGreen())
        {
            return "#45FF32"; // Green color (hex)
        }
        else if (targetNode.IsRed())
        {
            return "#FF0052"; // Red color (hex)
        }
        else
        {
            return "#0064C5"; // Default blue color (hex) if no specific state
        }
    }

    // Highlight the active node with a particle effect (implement this as per your needs)
    public void HighlightNode(Node2D node)
    {
        //Destory the previous particle effect if it exists
        if (activeParticleEffect != null)
        {
            Destroy(activeParticleEffect);
            Debug.Log("particle effect destroyed");
        }

        //instantiate the new particle effect at the node's position
        activeParticleEffect = Instantiate(glowingCirclePrefab, node.transform.position, Quaternion.identity);

        //attach the particle effect to the node so it follows the node's postition
        activeParticleEffect.transform.SetParent(node.transform);

        //play the particle effect if its not playing automatically
        ParticleSystem particleSystem = activeParticleEffect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play(); //manually start the particle effect
            Debug.Log("Particle effect started");
        }
        else
        {
            Debug.LogWarning("No particle system compnent found ont he glowing cirlce prefab");
        }

    }


    public void MoveToNode(Node2D targetNode)
    {
        if (CurrentNode != null && CurrentNode != targetNode)
            //if (CurrentNode.ConnectedNodes.Contains(targetNode))
        {
            Debug.Log($"Moving to Node {targetNode.ID}");
            CurrentNode = targetNode;
            //TriggerTask(CurrentNode);

            HighlightNode(CurrentNode);

            //display the messages ui 
            ShowMessageUI(targetNode);
        }
        else
        {
            Debug.Log($"Invalid move! Node {targetNode.ID} is not connected.");
        }
    }

    private void ShowMessageUI(Node2D node)
    {

        // Get the question assigned to this node
        QuestionData question = node.assignedQuestion;

        if (question == null)
        {
            Debug.LogWarning($"Node {node.name} has no assigned question!");
            return;
        }


        //activate the messafes ui and set the question
        messageUI.SetActive(true);
        messageText.text = question.questionText;

        optionAButton.GetComponentInChildren<Text>().text = question.optionA;
        optionBButton.GetComponentInChildren<Text>().text = question.optionB;

        //messageText.text = $"You've clicked Node {node.ID}. Choose your answer:";

        // Set up the buttons
        optionAButton.onClick.RemoveAllListeners();
        optionBButton.onClick.RemoveAllListeners();

        optionAButton.onClick.AddListener(() => HandleOptionSelected(node, question, true));
        optionBButton.onClick.AddListener(() => HandleOptionSelected(node, question, false));

        // Start the timer for 10 seconds
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine); // Stop any existing timers
        }
        timerCoroutine = StartCoroutine(AnswerTimer(node, levelData.timerDuration)); // Start a new timer
    }

    private void HandleOptionSelected(Node2D node, QuestionData question, bool isOptionA)
    {
        // Stop the timer as the player has answered
        //if (timerCoroutine != null)
        //{
        //    StopCoroutine(timerCoroutine);
        //}

        // Example logic for correct or incorrect answers, set node state and Display explanation
        if (isOptionA == question.isOptionACorrect)
            //if (isOptionA)
        {
            Debug.Log("Correct answer!");
            //explanationText.text = "Option A is correct because...";
            explanationText.text = question.explanationForCorrect;
            node.SetGreen(); // Turn the node green
        }
        else
        {
            Debug.Log("Wrong answer.");
            explanationText.text = question.explanationForIncorrect;
            //explanationText.text = "Option B is incorrect because...";
            node.SetRed(); // Turn the node red
        }

<<<<<<< Updated upstream
        // Show explanation and hide the UI after a short delay
        StartCoroutine(HideMessageUI());
=======
        // Start or update the timer for the next question or node
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine); // Optional: If you want to reset the timer when answering
        }
        timerCoroutine = StartCoroutine(AnswerTimer(node, levelData.timerDuration)); // Restart timer after answering

        // Delay win condition check until all nodes are processed or specific trigger
        StartCoroutine(DelayedWinCheck());
>>>>>>> Stashed changes
    }

    private IEnumerator AnswerTimer(Node2D node, float duration)
    {
<<<<<<< Updated upstream
        Debug.Log($"Starting timer for {duration} seconds.");
        yield return new WaitForSeconds(duration);
=======
        timeRemaining = duration; // Reset the timer for each question

        // While there is time remaining, update the timer
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second
            timeRemaining--; // Decrease the remaining time
            UpdateTimerUI(timeRemaining); // Update the timer text on screen
        }
>>>>>>> Stashed changes

        // If no answer is selected, mark the node as failed (red)
        Debug.Log($"Time's up! Node {node.ID} has failed.");
        node.SetRed();

        // Hide the message UI
        messageUI.SetActive(false);
    }

    private IEnumerator HideMessageUI()
    {
        explanationText.gameObject.SetActive(true); // Show the explanation
        yield return new WaitForSeconds(3f); // Wait for 3 seconds (optional)
        messageUI.SetActive(false); // Hide the UI
        explanationText.gameObject.SetActive(false); // Reset the explanation visibility
    }










    // Trigger task for a node (green/red logic)
    private void TriggerTask(Node2D node)
    {
        bool taskSuccess = Random.value > 0.5f;
        if (taskSuccess)
        {
            node.SetGreen();
            greenCount++;
            Debug.Log($"Node {node.ID} turned green!");
        }
        else
        {
            node.SetRed();
            redCount++;
            Debug.Log($"Node {node.ID} turned red!");
        }

        if (greenCount == allNodes.Count)
        {
            Debug.Log("You Win!");
        }
        if (redCount == allNodes.Count)
        {
            Debug.Log("Game Over");
        }
    }
}
