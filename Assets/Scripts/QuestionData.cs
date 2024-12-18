using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Question")]
public class QuestionData : ScriptableObject
{
    [TextArea] public string questionText;   // The question itself
    public string optionA;                   // Option A
    public string optionB;                   // Option B
    public bool isOptionACorrect;            // Is Option A the correct answer?
    [TextArea] public string explanationForCorrect;   // Why the answer is correct
    [TextArea] public string explanationForIncorrect; // Why the answer is incorrect

}
//This defines the question data template
