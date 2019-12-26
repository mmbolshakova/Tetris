using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject blockPr;

    [SerializeField] private Sprite[] blockSprite;

    private struct Block
    {
        public int x;
        public int y;
        public GameObject obj;

        public Block(int x, int y, GameObject obj)
        {
            this.x = x;
            this.y = y;
            this.obj = obj;
        }
    }

    private Block[] piece = new Block[4]
    {
        new Block(),
        new Block(),
        new Block(),
        new Block(),
    };

    public int W = 10;
    public int H = 20;

    private Block[,] block;

    private int[,] shapes = new int[,]
    {
        {1, 3, 5, 7},
        {2, 4, 5, 7},
        {3, 4, 5, 6},
        {3, 4, 5, 7},
        {2, 3, 5, 7},
        {3, 5, 6, 7},
        {2, 3, 4, 5},
    };

    private float moveTime = 0f;
    private float moveSpeed = 0.06f;
    private float time = 0;
    private float dropSpeed = 0.4f;
    void Start()
    {
        block = new Block[W, H];
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            HoldAndMove(-1, 0);
        else if (Input.GetKey(KeyCode.RightArrow))
            HoldAndMove(1, 0);
        else if (Input.GetKey((KeyCode.UpArrow)))
            Rotate();
        else if (Input.GetKey(KeyCode.DownArrow))
            dropSpeed = 0.05f; 
        time += Time.deltaTime;
        if (time > dropSpeed)
        {
            if (!Move(0, -1))
            {
                for (int i = 0; i < 4; i++)
                    block[Math.Abs(piece[i].x), -piece[i].y] = piece[i];
                Generate();
                Clear();
            }
            time = 0;
        }
    }

    private void Generate()
    {
        dropSpeed = 0.4f;
        int n = Random.Range(0, shapes.GetLength(0));
        for (int i = 0; i < 4; i++)
        {
            piece[i].x = shapes[n, i] % 2;
            piece[i].y = - shapes[n, i] / 2;
        }

        Sprite sprite = blockSprite[Random.Range(0, blockSprite.Length)];
        for (int i = 0; i < 4; i++)
        {
            piece[i].obj = Instantiate(blockPr, new Vector2(piece[i].x, piece[i].y), Quaternion.identity);
            SpriteRenderer sr = piece[i].obj.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
        }
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

    private bool Move(int dx, int dy)
    {
        Block[] origin = piece.Clone() as Block[];
        for (int i = 0; i < 4; i++)
        {
            piece[i].x += dx;
            piece[i].y += dy;
        }

        return CheckAndSet(origin);
    }

    private void Rotate()
    {
        Block[] origin = piece.Clone() as Block[];
        Block p = piece[1];
        for (int i = 0; i < 4; i++)
        {
            int y = piece[i].x - p.x;
            int x = piece[i].y - p.y;
            piece[i].x = p.x - x;
            piece[i].y = p.y + y;
        }
        CheckAndSet(origin);
    }

    private bool CheckAndSet(Block[] ori)
    {
        bool set = true;
        for (int i = 0; i < 4; i++)
        {
            if (piece[i].x < 0 || piece[i].x >= W || piece[i].y <= -H || block[piece[i].x, -piece[i].y].obj)
                set = false;
        }
        if (set)
            for (int i = 0; i < 4; i++)
                piece[i].obj.transform.position = new Vector2(piece[i].x, piece[i].y);
        else
            piece = ori;
        return set;
    }

    private void Clear()
    {
        List<Block> blockToClear = new List<Block>();
        int k = H - 1;
        int dy = 0;

        for (int i = H - 1; i > 0; i--)
        {
            int count = 0;
            blockToClear.Clear();
            for (int j = 0; j < W; j++)
            {
                if (block[j, i].obj)
                    count++;
                block[j, i].y += dy;
                blockToClear.Add(block[j, i]);
                block[j, k] = block[j, i];
            }

            if (count < W)
                k--;
            else
            {
                dy -= 1;
                for (int l = 0; l < blockToClear.Count; l++)
                {
                    Destroy(blockToClear[l].obj);
                }
            }

            for (int j = 0; j < W; j++)
            {
                if (block[j, i].obj)
                    block[j, i].obj.transform.position = new Vector2(block[j, i].x, block[j, i].y);
            }
        }
    }
}
