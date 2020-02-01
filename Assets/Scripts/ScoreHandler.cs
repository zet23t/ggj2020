using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public TextMeshPro scoreText;
    private long currentScore = 1000;


    // Update is called once per frame
    void FixedUpdate()
    {
        currentScore += 1;
        scoreText.text = "Points: \n" + currentScore;
    }
}
