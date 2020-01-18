using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public GameObject gameObjectBoard;
    public Text scoreText;
    int score;
    private Board board;
    
    void Start()
    {
        board = gameObjectBoard.GetComponent<Board>();
    }
    void Update()
    {
        score = board.NewScore();
        scoreText.text = "Score: " + score.ToString();
    }
}
