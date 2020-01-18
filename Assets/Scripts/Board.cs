using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    public GameObject gameObjectBoard;
    public Canvas gameOver;
    public Canvas pause;
    public AudioClip delete;
    public AudioClip fall;
    private Blocks blocks;
    public int W = 10;
    public int H = 20;
    private float moveTime = 0f;
    private float moveSpeed = 0.06f;
    private float time = 0;
    private int score = 0;
    private string saveRating;
    private bool set;
    public int NewScore()
    {
        return score; 
    }
    public void MenuPressed()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PausePressed()
    {
        Time.timeScale = 0;
        pause.gameObject.SetActive(true);
    }
    public void BackPressed()
    {
        pause.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    int [] marks = {0, 0, 0, 0, 0};
    void Save()
    {
        int count = 0;
        saveRating = PlayerPrefs.GetString("New");
        string[] help = saveRating.Split(',');
        for (int i = 0; i < 5 ; i++)
        {
            marks[i] = int.Parse(help[i]);
            if (score > marks[i])
                count++;
        }
        switch (count)
        {
            case 5:
                marks[0] = score;
                break;
            case 4:
                marks[1] = score;
                break;
            case 3:
                marks[2] = score;
                break;
            case 2:
                marks[3] = score;
                break;
            case 1:
                marks[4] = score;
                break;
        }
        string delimiter = ", ";
        string tmp = marks.Select(i => i.ToString()).Aggregate((i, j) => i + delimiter + j);
        PlayerPrefs.SetString("New", tmp);
        PlayerPrefs.Save();
        saveRating = PlayerPrefs.GetString("New");
    }
    void Start()
    {
        blocks = gameObjectBoard.GetComponent<Blocks>();
    }
    public void PlayAudio(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            HoldAndMove(-1, 0);
        else if (Input.GetKey(KeyCode.RightArrow))
            HoldAndMove(1, 0);
        else if (Input.GetKeyDown((KeyCode.UpArrow)))
            Rotate();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            blocks.dropSpeed = 0.05f; 
        time += Time.deltaTime;
        if (time > blocks.dropSpeed)
        {
            if (!Move(0, -1))
            {
                PlayAudio(fall);
                for (int i = 0; i < 4; i++)
                    blocks.block[Math.Abs(blocks.piece[i].x), -blocks.piece[i].y] = blocks.piece[i];
                blocks.Generate();
                Clear();
            }
            time = 0;
        }
    }
    public bool GameOver()
    {
        for (int i = 0; i < W; i++)
        {
            int count = 0;
            for (int j = 0; j < H; j++)
            {
                if (blocks.block[i,j].obj)
                    count++;
                if (count == (H - 5))
                {
                    Save();
                    gameOver.gameObject.SetActive(true);
                    return true;
                }
            }
        }
        return false;
    }
    private void HoldAndMove(int dx, int dy)
    {
        moveTime += Time.deltaTime;
        if (moveTime > moveSpeed)
        {
            Move(dx, dy);
            moveTime = 0f;
        }
    }
    public bool Move(int dx, int dy)
    {
        Blocks.Block[] origin = blocks.piece.Clone() as Blocks.Block[];
        for (int i = 0; i < 4; i++)
        {
            blocks.piece[i].x += dx;
            blocks.piece[i].y += dy;
        }
        return Check(origin);
    }
    private void Rotate()
    {
        if (blocks.choiceFigure != 6)
        {
            Blocks.Block[] origin = blocks.piece.Clone() as Blocks.Block[];
            Blocks.Block p = blocks.piece[1];
            for (int i = 0; i < 4; i++)
            {
                int y = blocks.piece[i].x - p.x;
                int x = blocks.piece[i].y - p.y;
                blocks.piece[i].x = p.x - x;
                blocks.piece[i].y = p.y + y;
            }
            Check(origin);
        }
    }
    private void Set(Blocks.Block[] ori, bool isset)
    {
        if (isset)
            for (int i = 0; i < 4; i++)
                blocks.piece[i].obj.transform.position = new Vector2(blocks.piece[i].x, blocks.piece[i].y);
        else
            blocks.piece = ori;
    }
    private bool Check(Blocks.Block[] ori)
    {
        set = true;
        for (int i = 0; i < 4; i++)
        {
            if (blocks.piece[i].x < 0 || blocks.piece[i].x >= W || blocks.piece[i].y <= -H || blocks.piece[i].y > 0)
                set = false;
            else if (blocks.block[blocks.piece[i].x, -blocks.piece[i].y].obj)
                set = false;
        }
        Set(ori, set);
        return set;
    }
    private void Clear()
    {
        List<Blocks.Block> blockToClear = new List<Blocks.Block>();
        int k = H - 1;
        int dy = 0;
    
        for (int i = H - 1; i > 0; i--)
        {
            int count = 0;
            blockToClear.Clear();
            for (int j = 0; j < W; j++)
            {
                if (blocks.block[j, i].obj)
                    count++;
                blocks.block[j, i].y += dy;
                blockToClear.Add(blocks.block[j, i]);
                blocks.block[j, k] = blocks.block[j, i];
            }
            if (count < W)
                k--;
            else
            {
                score++;
                dy -= 1;
                for (int l = 0; l < blockToClear.Count; l++)
                {
                    PlayAudio(delete);
                    Destroy(blockToClear[l].obj);
                }
            }
    
            for (int j = 0; j < W; j++)
            {
                if (blocks.block[j, i].obj)
                    blocks.block[j, i].obj.transform.position = new Vector2(blocks.block[j, i].x, blocks.block[j, i].y);
            }
        }
    }
}
