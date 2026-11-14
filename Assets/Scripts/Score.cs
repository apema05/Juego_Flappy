using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    private int score;
    private TextMeshProUGUI uiScore;
    private void Start()
    {
        uiScore = GetComponent<TextMeshProUGUI>();
        uiScore.text = "";
    }

    public void addPoint()
    {
        score++;
        uiScore.text = "" + score;
    }
}
