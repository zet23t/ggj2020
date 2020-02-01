using TMPro;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    private AudioSource sfxBleep;
    public TextMeshPro scoreText;

    public bool isFrozen = false;
    
    public double elapsedTime = 0;

    public long currentScore { get; private set; }

    void Start()
    {
        currentScore = 0;
        sfxBleep = GetComponent<AudioSource>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if(!isFrozen) {
            currentScore = (long)(elapsedTime * 100);
            scoreText.text = currentScore.ToString();
        } else if(elapsedTime > 0.5f) {
            elapsedTime = 0.0f;
            scoreText.text = scoreText.text == "" ? currentScore.ToString() : "";
            if(scoreText.text != "") {
                sfxBleep.Play();
                sfxBleep.volume = sfxBleep.volume * 0.9f;
            }
        }
    }

    public void FreezeTimer()
    {
        isFrozen = true;
    }
}
