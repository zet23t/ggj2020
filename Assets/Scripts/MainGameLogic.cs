﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameLogic : MonoBehaviour
{
    public BlockMapVisualizer blockMapVisualizer;
    public Animator trainAnimator;
    public ScoreHandler scoreHandler;

    public AudioSource bgMusic;
    public AudioSource sfxCrowdShock;

    public float BlockPushInterval = 3.0f;
    public float BlockPushMinInterval = 0.5f;
    public float BlockPushSpeedRetainPercentage = 0.8f;

    private float timeElaspedSinceLastTrigger = 0.0f;
    private bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        UpdateTrainAnimSpeed();
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
            UpdateTrainAnimSpeed();
        }
    }

    private void OnGameOver()
    {
        Debug.Log("Game Over!");

        bgMusic.Stop();
        sfxCrowdShock.Play();

        isGameOver = true;
        scoreHandler.FreezeTimer();
        trainAnimator.SetBool("IsExploded", true);
    }

}