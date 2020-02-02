using TMPro;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    private AudioSource sfxBleep;
    private bool textShowsGameOver = false;

    public TextMeshPro scoreText;

    public bool isFrozen = false;
    
    public double elapsedTime = 0;

    public int currentScore { get; private set; }

    void Start()
    {
        currentScore = 0;
        sfxBleep = GetComponent<AudioSource>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if(!isFrozen) {
            currentScore = (int)(elapsedTime * 100);
            scoreText.text = currentScore.ToString();
        } else if(elapsedTime > 0.65f) {
            elapsedTime = 0.0f;

            if(scoreText.text == "") {

                scoreText.text = textShowsGameOver ? "SCORE" : currentScore.ToString();
                textShowsGameOver = !textShowsGameOver;
                sfxBleep.Play();
                sfxBleep.volume = sfxBleep.volume * 0.9f;
            } else {
                scoreText.text = "";
            }
        }
    }

    public void AddSimulatorPoints(BlockMapSimulator simulator)
    {
        currentScore += simulator.GetPoints();
    }

    public void FreezeTimer()
    {
        isFrozen = true;
    }
}
