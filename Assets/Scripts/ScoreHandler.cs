using TMPro;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    private AudioSource sfxBleep;
    private bool textShowsGameOver = false;

    public TextMeshPro scoreText;

    public bool isFrozen = false;
    
    public double elapsedTime = 0;

    public int currentScore;

    public int BlockScore;

    void Start()
    {
        currentScore = 0;
        BlockScore = 0;
        sfxBleep = GetComponent<AudioSource>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if(!isFrozen) {
            currentScore = (int)(elapsedTime * 100) + BlockScore;
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
        //currentScore += simulator.GetPoints();
    }

    public void FreezeTimer()
    {
        isFrozen = true;
    }
}
