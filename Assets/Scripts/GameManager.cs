using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
}
