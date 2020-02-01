using TMPro;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public TextMeshPro scoreText;
    public long currentScore { get; private set; }

    void Start()
    {
        currentScore = 1000;
    }

    void FixedUpdate()
    {
        currentScore += 1;
        scoreText.text = "Points: \n" + currentScore;
    }
}
