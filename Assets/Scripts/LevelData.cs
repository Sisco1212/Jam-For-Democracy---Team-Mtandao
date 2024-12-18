using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData : MonoBehaviour
{
    public List<Node2D> nodes; // Nodes specific to this level
    public Node2D startNode; // Start node for the level
    public List<QuestionData> questions; // Questions for the level
    public int targetGreenNodesCount;

    public float timerDuration; // New field for timer duration

}
