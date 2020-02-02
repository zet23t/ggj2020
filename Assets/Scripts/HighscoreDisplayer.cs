using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreDisplayer : MonoBehaviour
{
    public TextMeshProUGUI TextToRenderInto;
    
    // Start is called before the first frame update
    void Start()
    {
        int highscoreToDisplay = 100;
        if (PlayerPrefs.HasKey("Highscore"))
        {
            highscoreToDisplay = PlayerPrefs.GetInt("Highscore");
        }

        TextToRenderInto.text = "Highscore: " + highscoreToDisplay;
    }
}
