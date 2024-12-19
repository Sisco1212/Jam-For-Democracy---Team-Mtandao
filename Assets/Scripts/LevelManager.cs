using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> levelObjects; // List of Empty GameObjects for levels
    public int currentLevel; // Track the current level

    public GameObject graphManagerPrefab; // Reference to the GraphManager prefab

    private GraphManager currentGraphManager; // To hold the current GraphManager instance for the level

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
                                                                                    // Destroy the previous GraphManager (if exists)
        if (currentGraphManager != null)
        {
            Destroy(currentGraphManager.gameObject);
        }

        // Instantiate a new GraphManager for this level
        currentGraphManager = Instantiate(graphManagerPrefab, levelObjects[currentLevel].transform).GetComponent<GraphManager>();

        // Initialize GraphManager with this level's data
        currentGraphManager.InitializeLevel(levelData);

        // Reset nodes for the new level
        currentGraphManager.ResetNodesForNewLevel();
    }

    // Call this to move to the next level (Assumes one moves to level 2 directly from level 1)
    public void NextLevel(int levelToStart)
    {
        StartLevel(levelToStart);
    }

    // Call this to go to a specific level by name
    public void GoToLevel(string levelName)
    {
        // Find the level index by name
        for (int i = 0; i < levelObjects.Count; i++)
        {
            if (levelObjects[i].name == levelName)
            {
                StartLevel(i);
                return;
            }
        }

        Debug.LogError($"Level with name {levelName} not found!");
    }
}

