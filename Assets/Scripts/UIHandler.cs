using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    private void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene("Scenes/Levels/Level" + levelIndex, LoadSceneMode.Single);
    }

    public void LoadFirstLevel()
    {
        LoadLevel(1);
    }
    
    public void LoadStartingScreen()
    {
        SceneManager.LoadScene("Scenes/StartingScene", LoadSceneMode.Single);
    }
    
    public void LoadCreditsScreen()
    {
        SceneManager.LoadScene("Scenes/Credits", LoadSceneMode.Single);
    }
    
}