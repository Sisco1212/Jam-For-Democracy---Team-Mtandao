using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    public Button[] lvlButtons;

    // Start is called before the first frame update
    void Start()
    {
        int level = PlayerPrefs.GetInt("level", 2);
        
         for (int i = 0; i < lvlButtons.Length; i++)
        {
            if (i + 2 > level)
            {
                lvlButtons[i].interactable = false;
            }
        }
    }

}
