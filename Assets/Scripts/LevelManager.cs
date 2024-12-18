using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> levelObjects; // List of Empty GameObjects for levels
    public int currentLevel = 0; // Track the current level

    public GraphManager graphManager; // Reference to GraphManager

    // Call this method to start the level
    public void StartLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelObjects.Count)
        {
            Debug.LogError("Invalid level index");
            return;
        }

        // Set the current level
        currentLevel = levelIndex;

        // Deactivate all levels
        foreach (GameObject level in levelObjects)
        {
            level.SetActive(false);
        }

        // Activate the current level
        levelObjects[currentLevel].SetActive(true);

        // Initialize the level's data (nodes, questions, etc.)
        LevelData levelData = levelObjects[currentLevel].GetComponent<LevelData>(); // Assuming LevelData is attached to the Empty GameObject

        // Initialize GraphManager with this level's data
        graphManager.InitializeLevel(levelData);

        // Reset nodes for the new level
        graphManager.ResetNodesForNewLevel();
    }

    // Call this to move to the next level
    public void NextLevel()
    {
        StartLevel(currentLevel + 1);
    }
}

