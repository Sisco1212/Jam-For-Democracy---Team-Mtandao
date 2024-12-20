using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public Canvas canvas;  // Reference to the Canvas
    public GameObject levelBg;

    private void Start()
    {
        InitializeLevel(levelData);
        SetTimerDuration(levelData.timerDuration); // Set the timer duration for the current level

        // Find the node with a specific ID or randomly select one
        int targetID = 0;
        CurrentNode = allNodes.Find(node => node.ID == targetID);

        if (allNodes == null || allNodes.Count == 0)
        {
            Debug.LogError("No nodes available to assign as the CurrentNode.");
            return;
        }

        //Ensure CurrentNode is assigned before use, either through editor or dynamically
        if (CurrentNode == null)
        {
            Debug.LogWarning($"Node with ID {targetID} not found! Defaulting to the first node.");
            CurrentNode = allNodes[0]; // Default to the first node

        }

        if (CurrentNode != null)
        {
            Debug.Log($"Starting node set to {CurrentNode.ID} (found by ID)");
        }
        else
        {
            Debug.LogWarning($"Node with ID {targetID} not found! Defaulting to a random node.");
            CurrentNode = allNodes[Random.Range(0, allNodes.Count)];
        }



        HighlightNode(CurrentNode); // Highlight the active node with the particle effect on start
        SetConnections(); // Set the connections for the nodes (visualize them)

        //toogle messages UI visibility off on start
        messageUI.SetActive(false);

        //add listener to the starting node to trigger ShowMessages when clicked
        //CurrentNode.OnClick += () => ShowMessageUI(CurrentNode);
        CurrentNode.OnClick += () => OnFirstNodeClick(CurrentNode);

        if (levelBg != null)
        {
            levelBg.SetActive(true);  // Ensure the background is active
            Debug.Log("Background activated in Start.");
        }

    }


    //reset game method 
    public void ResetGame()
    {
        // Reset node states (green/red)
        ResetNodeStates();

        // Reset the counts for green and red nodes
        greenCount = 0;
        redCount = 0;

        // Reset the timer
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine); // Stop any existing timer
        }
        SetTimerDuration(levelData.timerDuration); // Reset the timer to its initial duration

        // Reinitialize level setup
        InitializeLevel(levelData); // Reinitialize the level data and nodes

        // Reset message UI
        messageUI.SetActive(false); // Deactivate the message UI
        winUI.SetActive(false); // hide the "You Win!" UI
        loseUI.SetActive(false); // Hide the "Game Over" UI
        levelBg.SetActive(true);

        Debug.Log("Game Reset1");

    }

    // This will be called by the LevelManager to initialize the level
    public void InitializeLevel(LevelData levelData)
    {
        this.levelData = levelData; // Assign level data here

        if (levelData.nodes == null || levelData.nodes.Count == 0)
        {
            Debug.LogError("Level data nodes are not assigned or empty!");
            return;
        }

        SetTimerDuration(levelData.timerDuration); // Set the timer duration for the current level
        allNodes = levelData.nodes; // Set the nodes for this level
        CurrentNode = levelData.startNode; // Set the starting node
        targetGreenNodesCount = levelData.targetGreenNodesCount; // Initialize target green nodes count

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

        if (levelData == null || levelData.nodes == null || levelData.nodes.Count == 0)
        {
            Debug.LogError("Level data or nodes are not properly initialized!");
        }
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
    public void SetConnections()
    {
        // Create a HashSet to track connections to avoid duplicates
        HashSet<string> createdConnections = new HashSet<string>();

        foreach (Node2D node in allNodes)
        {
            Debug.Log($"Checking connections for Node {node.ID}.");
            // Populate `ConnectedNodes` dynamically if not already set
            if (node.ConnectedNodes == null || node.ConnectedNodes.Count == 0)
            {
                node.ConnectedNodes = new List<Node2D>(); // Ensure it is initialized
                Debug.Log($"Node {node.ID} connections initialized.");
            }

            // Loop through each node's connected nodes
            foreach (Node2D connectedNode in node.ConnectedNodes)
            {
                // Create a unique string to represent the connection (using node IDs to identify the connection)
                string connectionKey = GetConnectionKey(node.ID, connectedNode.ID);

                // Create connection only if the reverse connection is not present
                if (!connectedNode.ConnectedNodes.Contains(node))
                {
                    connectedNode.ConnectedNodes.Add(node); // Ensure it's bidirectional
                }

                // Check if this connection has already been created
                if (!createdConnections.Contains(connectionKey))
                {
                    CreateConnection(node, connectedNode); // Create the connection between node and connectedNode
                    createdConnections.Add(connectionKey); // Mark this connection as created

                    Debug.Log($"Creating connection from Node {node.ID} to Node {connectedNode.ID}.");
                }
                else
                {
                    Debug.Log($"Connection from Node {node.ID} to Node {connectedNode.ID} already exists, skipping.");
                }
            }
        }
    }

    // Helper method to create a unique key for each connection (since the connection is undirected)
    private string GetConnectionKey(int nodeId1, int nodeId2)
    {
        // Create a key that is unique for the connection between node1 and node2, no matter the order
        return nodeId1 < nodeId2 ? $"{nodeId1}-{nodeId2}" : $"{nodeId2}-{nodeId1}";
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
            connectionRenderer.enabled = false; // Hide the connection
        }


        // Add the connection to the list for later management
        connections.Add(connection);
        Debug.Log($"Connection from Node {node1.ID} to Node {node2.ID} created and added to the list.");
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
        //if (CurrentNode == null)
        //{
        //    Debug.LogError("CurrentNode is null! Cannot move.");
        //    return;
        //}

        if (CurrentNode.ConnectedNodes.Contains(targetNode))
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
        yield return new WaitForSeconds(1f); // Wait for 2 seconds
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
        if (timerText == null)
        {
            Debug.LogError("Timer Text is not assigned!");
            return;
        }

        // Convert timeLeft (seconds) into minutes and seconds
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        // Format as "mm:ss"
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GoToHomeLevel()
    {
        ResetGame();

        //disable the current active level
        // Find all game objects with the "Level" tag 
        GameObject[] levels = GameObject.FindGameObjectsWithTag("Level");

        // Deactivate each level object (or just the active one if you know which one should be)
        foreach (GameObject level in levels)
        {
            level.SetActive(false);  // Deactivate the level(s)
        }

        // Deactivate the background
        if (levelBg != null)
        {
            levelBg.SetActive(false);  // Deactivate the background
            Debug.Log("Background deactivated.");
        }
        else
        {
            Debug.LogError("Level background not assigned! Please assign it in the Inspector.");
        }



        // If canvas is assigned in the inspector, use it directly
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);  // Activate the Canvas GameObject
            Debug.Log("Canvas activated.");
        }
        else
        {
            Debug.LogError("Canvas not assigned! Please assign a Canvas in the Inspector.");
        }
    }







}