using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private LevelManager levelManagerScript;
    public GameObject phoneScreen;


    // Start is called before the first frame update
    void Start()
    {
        levelManagerScript = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMap(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void ShowQuestion1(GameObject panelToShow) {
        panelToShow.SetActive(true);
    }
 
    public void Show(GameObject panelToShow) {
        panelToShow.SetActive(true);
    }
    public void Hide(GameObject panelToHide) {
        panelToHide.SetActive(false);
    }

    public void LoadLevelOne() {
        phoneScreen.SetActive(false);
        levelManagerScript.NextLevel(0);
    }

    public void LoadLevelTwo() {
        phoneScreen.SetActive(false);
        levelManagerScript.NextLevel(1);
    }

    public void LoadLevelThree() {
        phoneScreen.SetActive(false);
        levelManagerScript.NextLevel(2);
    }

    public void LoadLevelFour() {
        phoneScreen.SetActive(false);
        levelManagerScript.NextLevel(3);
    }

    public void LoadLevelFive() {
        phoneScreen.SetActive(false);
        levelManagerScript.NextLevel(4);
    }
}
