﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DigitalRuby.Tween;

public class UI : MonoBehaviour {

    Text timeText;
    Text gameOverText;
    float currentTime = 0;
    public bool RunTimer = true;
    public float FinalTime { get { return currentTime; } }

	// Use this for initialization
	void Start ()
    {
        timeText = GameObject.Find("TimeText").GetComponent<Text>();
        gameOverText = GameObject.Find("GameOver").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {

        if (RunTimer)
            currentTime += Time.deltaTime;

        int milliseconds = (int)(FinalTime * 1000) % 1000;
        int seconds = (int)FinalTime % 60;
        int minutes = (int)(FinalTime / 60) % 60;
        int hours = (int)(FinalTime / 3600);

        timeText.text = "Time\n";

        if (hours > 0)
            timeText.text += formatMultiDigits(hours, 2) + ":";

        timeText.text += formatMultiDigits(minutes, 2);
        timeText.text += ":" + formatMultiDigits(seconds, 2);
        timeText.text += ":" + formatMultiDigits(milliseconds, 3);
	}

    public void PlayerDied()
    {
        RunTimer = false;

        TweenFactory.Tween("Bigger Text", 14, 72, 2, TweenScaleFunctions.QuadraticEaseOut, (t) =>
        {  timeText.fontSize = (int)t.CurrentValue; });

        TweenFactory.Tween("Move Up", -300, -60, 1, TweenScaleFunctions.QuadraticEaseOut, (t) =>
        {
            gameOverText.rectTransform.localPosition = new Vector3(
            gameOverText.rectTransform.localPosition.x,
            t.CurrentValue,
            gameOverText.rectTransform.localPosition.z
            );
        });
    }

    string formatMultiDigits(int value, int digits)
    {
        string s = "";
        for (int i = digits - 1; i >= 0; i--)
        {
            int amount = (int)Mathf.Pow(10.0f, (float)i);
            if (value < amount)
                s += "0";
            else
                return s + value;
        }

        return s;
    }
}
