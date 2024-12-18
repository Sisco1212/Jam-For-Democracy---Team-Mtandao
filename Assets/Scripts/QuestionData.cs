using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Question")]
public class QuestionData : ScriptableObject
{
    [TextArea] public string questionText;   // The question itself
    public Sprite optionAImage;
    public Sprite optionBImage;
    public Sprite optionCImage; 
    public bool isOptionACorrect; // Is Option A the correct answer?
    public bool isOptionBCorrect; // Indicates if Option B is correct
    public bool isOptionCCorrect; // Indicates if Option C is correct 



}
//This defines the question data template
