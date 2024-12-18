using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    public Node2D CurrentNode; // Starting node
    private List<Node2D> allNodes; // List to store all nodes

    public List<GameObject> connections; // Store the connection GameObjects for later reference
    public GameObject connectionPrefab; // Reference to the connection sprite prefab

    public GameObject glowingCirclePrefab; //reference to the glowing particle prefab
    private GameObject activeParticleEffect; // to store the current particle effect


    public GameObject messageUI; // Reference to the Message UI Panel
    public TMP_Text messageText; // Text for the message
    public Button optionAButton; // Button for Option A
    public Button optionBButton; // Button for Option B
    public Button optionCButton; // Button for Option C

    public GameObject winUI; // Reference to the "You Win!" UI Panel
    public GameObject loseUI; // Reference to the "Game Over" UI Panel

    private int targetGreenNodesCount;

    public LevelData levelData;

    private Coroutine timerCoroutine; // Reference to the timer coroutine
    private bool isTimerStarted = false; // Track if the timer has been started

    public GameObject spamWarningText;  // Assign this in the inspector

    public TMP_Text timerText;
    private float timeRemaining;


    private int greenCount = 0;
    private int redCount = 0;

    // This will be called by the LevelManager to initialize the level
    public void InitializeLevel(LevelData levelData)
    {
    
        if (levelData == null)
        {
            Debug.LogError("levelData is null!");
            return;
        }

        this.levelData = levelData;
        SetTimerDuration(levelData.timerDuration);

        if (levelData.nodes == null || levelData.nodes.Count == 0)
        {
            Debug.LogError("levelData.nodes is null or empty!");
            return;
        }

        allNodes = levelData.nodes;
        CurrentNode = levelData.startNode;

        if (CurrentNode == null)
        {
            Debug.LogError("startNode is null in levelData!");
            return;
        }

        targetGreenNodesCount = levelData.targetGreenNodesCount;


        // Reset green and red counts
        greenCount = 0;
        redCount = 0;
        Debug.Log("Counts have been reset");

        SetConnections(); // Set the connections based on current level's nodes
        HighlightNode(CurrentNode); // Highlight the starting node

        // Reset any other state variables if needed
        ResetNodeStates(); // Reset the node colors (green/red)

        // Activate the message UI off at the start
        messageUI.SetActive(false);
    }


    // Reset the node states (green/red) at the start of each level
    private void ResetNodeStates()
    {
        foreach (var node in allNodes)
        {
            node.ResetState(); // Assuming ResetState() is a method that clears the node's color state (green/red)
        }
    }

    public void ResetNodesForNewLevel()
    {
        foreach (Node2D node in allNodes)
        {
            node.ResetState();  // Reset all nodes to their neutral state
        }
    }


    private void Start()
    {
        InitializeLevel(levelData);
        SetTimerDuration(levelData.timerDuration); // Set the timer duration for the current level

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

        //TriggerTask(CurrentNode); // Trigger task on starting node
        HighlightNode(CurrentNode); // Highlight the active node with the particle effect on start
        SetConnections(); // Set the connections for the nodes (visualize them)

        //toogle messages UI visibility off on start
        messageUI.SetActive(false);

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
    }

    // This method will create and visualize the connections between the nodes
    //public void SetConnections()
    //{
    //    //Debug.Log($"Checking connections for Node {node.ID}.");
    //    foreach (Node2D node in allNodes)
    //    {
    //        Debug.Log($"Checking connections for Node {node.ID}.");
    //        // Loop through each node's connected nodes
    //        foreach (Node2D connectedNode in node.ConnectedNodes)
    //        {
    //            Debug.Log($"Creating connection from Node {node.ID} to Node {connectedNode.ID}.");
    //            CreateConnection(node, connectedNode); // Create the connection between node and connectedNode
    //        }
    //    }

    //}


    public void SetConnections()
    {
        HashSet<string> connectionsSet = new HashSet<string>(); // To track created connections and avoid duplicates

        foreach (Node2D node in allNodes)
        {
            Debug.Log($"Checking connections for Node {node.ID}.");

            // Loop through each node's connected nodes
            foreach (Node2D connectedNode in node.ConnectedNodes)
            {
                // Generate a unique string identifier for the connection between node and connectedNode
                string connectionKey = GetConnectionKey(node, connectedNode);

                // Check if this connection has already been created
                if (!connectionsSet.Contains(connectionKey))
                {
                    // Add the connection key to the HashSet
                    connectionsSet.Add(connectionKey);

                    // Create the connection if it's unique
                    Debug.Log($"Creating connection from Node {node.ID} to Node {connectedNode.ID}.");
                    CreateConnection(node, connectedNode); // Create the connection between node and connectedNode
                }
                else
                {
                    Debug.Log($"Connection from Node {node.ID} to Node {connectedNode.ID} already exists. Skipping.");
                }
            }
        }
    }

    // Helper method to generate a unique key for each connection
    private string GetConnectionKey(Node2D node1, Node2D node2)
    {
        // Sort the node IDs to ensure the key is the same regardless of direction
        int id1 = Mathf.Min(node1.ID, node2.ID);
        int id2 = Mathf.Max(node1.ID, node2.ID);

        return $"{id1}-{id2}"; // Return a unique key based on the node IDs
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

        //// Determine the color based on the target node (node2)
        //string hexColor = GetConnectionColor(node2);

        if (connectionRenderer != null)
        {
            connectionRenderer.enabled = true; // Hide the connection
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
        if (CurrentNode != null && CurrentNode != targetNode && CurrentNode.ConnectedNodes.Contains(targetNode))

        //if (CurrentNode != null && CurrentNode != targetNode)
        //if (CurrentNode.ConnectedNodes.Contains(targetNode))
        {
            Debug.Log($"Moving to Node {targetNode.ID}");
            CurrentNode = targetNode;

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


        //activate the messages ui and set the question
        messageUI.SetActive(true);
        messageText.text = question.questionText;


        optionAButton.GetComponent<Image>().sprite = question.optionAImage;
        optionBButton.GetComponent<Image>().sprite = question.optionBImage;
        optionCButton.GetComponent<Image>().sprite = question.optionCImage;


        // Set up the buttons
        optionAButton.onClick.RemoveAllListeners();
        optionBButton.onClick.RemoveAllListeners();
        optionCButton.onClick.RemoveAllListeners();

        optionAButton.onClick.AddListener(() => HandleOptionSelected(node, question, true));
        optionBButton.onClick.AddListener(() => HandleOptionSelected(node, question, false));
        optionCButton.onClick.AddListener(() => HandleOptionSelected(node, question, null));

        // Start the timer for 10 seconds
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine); // Stop any existing timers
        }
        timerCoroutine = StartCoroutine(AnswerTimer(node, levelData.timerDuration)); // Start a new timer
    }

    private void HandleOptionSelected(Node2D node, QuestionData question, bool? isOptionA)
    {
        // Check if the node has already been answered
        if (node.isAnswered)
        {
            Debug.Log("This node has already been answered.");

            // Show the spam warning UI for 2 seconds
            StartCoroutine(ShowSpamWarning());
            return; // Exit early if the node is already answered
        }

        // Stop the timer as the player has answered
        //if (timerCoroutine != null)
        //{
        //    StopCoroutine(timerCoroutine);
        //}

        // Determine which option was selected
        if (isOptionA == true && question.isOptionACorrect)
        {
            Debug.Log("Correct answer! (Option A)");
            node.SetGreen();
            greenCount++; // Increment green count
            Debug.Log($"Green Count: {greenCount} (Node {node.ID} set to Green)");
        }
        else if (isOptionA == false && question.isOptionBCorrect)
        {
            Debug.Log("Correct answer! (Option B)");
            node.SetGreen();
            greenCount++; // Increment green count
            Debug.Log($"Green Count: {greenCount} (Node {node.ID} set to Green)");
        }
        else if (isOptionA == null && question.isOptionCCorrect) // Handle Option C
        {
            Debug.Log("Correct answer! (Option C)");
            node.SetGreen();
            greenCount++; // Increment green count
            Debug.Log($"Green Count: {greenCount} (Node {node.ID} set to Green)");
        }
        else
        {
            Debug.Log("Wrong answer.");
            node.SetRed();
            redCount++;
            Debug.Log($"Red Count: {redCount} (Node {node.ID} set to Red)");
        }

        // Start or update the timer for the next question or node
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine); // Optional: If you want to reset the timer when answering
        }
        timerCoroutine = StartCoroutine(AnswerTimer(node, levelData.timerDuration)); // Restart timer after answering

        // Delay win condition check until all nodes are processed or specific trigger
        StartCoroutine(DelayedWinCheck());
    }

    private IEnumerator ShowSpamWarning()
    {
        spamWarningText.SetActive(true); // Activate the UI element
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        spamWarningText.SetActive(false); // Deactivate the UI element
    }

    private IEnumerator DelayedWinCheck()
    {
        // Wait for a small delay (if necessary, to allow other processes to complete)
        yield return new WaitForSeconds(0.1f);

        // Check if all nodes are processed before checking win condition
        if (allNodesProcessed())
        {
            CheckWinCondition();
        }
    }

    private bool allNodesProcessed()
    {
        foreach (Node2D node in allNodes)
        {
            // If the node is neither green nor red, it hasn't been processed
            if (!node.IsGreen() && !node.IsRed())
            {
                return false; // One or more nodes are not processed yet
            }
        }

        // All nodes have been processed (green or red)
        return true;
    }

    private void CheckWinCondition()
    {
        Debug.Log($"Checking win condition: Green Count = {greenCount}, Target = {targetGreenNodesCount}");

        if (greenCount >= targetGreenNodesCount)
        {
            Debug.Log("You Win!");
            ShowWinUI();
            messageUI.SetActive(false);
        }
        else 
        {
            Debug.Log("Game Over");
            ShowLoseUI();
            messageUI.SetActive(false);
        }
    }

    private void ShowWinUI()
    {
        winUI.SetActive(true); // Show the "You Win!" UI
        loseUI.SetActive(false); // Hide the "Game Over" UI
    }
    private void ShowLoseUI()
    {
        winUI.SetActive(false); // Hide the "You Win!" UI
        loseUI.SetActive(true); // Show the "Game Over" UI
    }

    // Set the timer duration for the level
    private void SetTimerDuration(float duration)
    {
        timeRemaining = duration; // Set the initial timer to the duration of the current level
        UpdateTimerUI(timeRemaining); // Update the UI immediately with the starting time
    }
    private IEnumerator AnswerTimer(Node2D node, float duration)
    {
        timeRemaining = duration; // Reset the timer for each question

        // While there is time remaining, update the timer
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second
            timeRemaining--; // Decrease the remaining time
            UpdateTimerUI(timeRemaining); // Update the timer text on screen
        }

        // If no answer is selected, mark the node as failed (red)
        Debug.Log($"Time's up! Node {node.ID} has failed.");
        node.SetRed();

        // Hide the message UI
        messageUI.SetActive(false);
    }

    // Method to update the UI text with the remaining time
    private void UpdateTimerUI(float timeLeft)
    {
        // Convert timeLeft (seconds) into minutes and seconds
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        // Format as "mm:ss"
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GoToHomeLevel()
    {

        // Deactivate the current UI to prevent visual glitches
        winUI.SetActive(false);
        loseUI.SetActive(false);
        messageUI.SetActive(false);

        SceneManager.LoadScene("HomeLevel", LoadSceneMode.Single);

        //// Replace "HomeLevel" with the actual name of your home level scene
        //string homeLevelSceneName = "HomeLevel";
        //SceneManager.LoadScene(homeLevelSceneName);
    }

}
