using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameLogic : MonoBehaviour
{
    public BlockMapVisualizer blockMapVisualizer;
    public Animator trainAnimator;
    public Animator camShakeAnimator;
    public ScoreHandler scoreHandler;

    public AudioSource bgMusic;
    public AudioSource sfxCrowdShock;
    public AudioSource sfxExplosion;

    public float BlockPushInterval = 3.0f;
    public float BlockPushMinInterval = 0.5f;
    public float BlockPushSpeedRetainPercentage = 0.8f;

    private float timeElaspedSinceLastTrigger = 0.0f;
    private bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        UpdateTrainAnimSpeed();
        
        if (!blockMapVisualizer.IsEditor)
        {
            blockMapVisualizer.ExplodeRandomBlock();
            camShakeAnimator.SetBool("IsShaking", true);
        }

        blockMapVisualizer.Simulator.SetScoreHandler(scoreHandler);
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameOver || blockMapVisualizer.IsEditor) {
            return;
        }
        
        UpdateBlockPush();
    }

    private void UpdateTrainAnimSpeed()
    {
        trainAnimator.SetFloat("TrainSpeed", 1.0f  / BlockPushInterval);
    }

    private void UpdateBlockPush()
    {
        timeElaspedSinceLastTrigger += Time.deltaTime;

        if (timeElaspedSinceLastTrigger > BlockPushInterval)
        {
            if(!blockMapVisualizer.ExplodeRandomBlock())
            {
                OnGameOver();
                return;
            }

            timeElaspedSinceLastTrigger = 0.0f;

            BlockPushInterval = Math.Max(BlockPushMinInterval, BlockPushInterval * BlockPushSpeedRetainPercentage);
            UpdateTrainAtEnd();
        }
    }

    private void UpdateTrainAtEnd()
    {
        UpdateTrainAnimSpeed();
        camShakeAnimator.SetBool("IsShaking", true);
        sfxExplosion.Play();
    }

    private void OnGameOver()
    {
        Debug.Log("Game Over!");
        UpdateTrainAtEnd();

        bgMusic.Stop();
        sfxCrowdShock.Play();

        isGameOver = true;

        if(scoreHandler != null) { // This is null in the main menu
            scoreHandler.FreezeTimer();
            scoreHandler.AddSimulatorPoints(blockMapVisualizer.Simulator);
            trainAnimator.SetBool("IsExploded", true);
        }

        HandleHighscore();
    }

    private void HandleHighscore()
    {
        var previousHighscore = 0;
        if (PlayerPrefs.HasKey("Highscore"))
        {
            previousHighscore = PlayerPrefs.GetInt("Highscore");
        }

        if (scoreHandler.currentScore > previousHighscore)
        {
            PlayerPrefs.SetInt("Highscore", scoreHandler.currentScore);
            PlayerPrefs.Save();
        }
    }
}
