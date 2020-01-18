using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Blocks : MonoBehaviour
{
    [SerializeField] private Sprite[] blockSprite;
    [SerializeField] private GameObject blockPrefab;
    public GameObject gameObjectBoard;
    private Board board;
    [HideInInspector] public int choiceFigure;
    [HideInInspector] public float dropSpeed = 0.4f;
    void Start()
    {
        board = gameObjectBoard.GetComponent<Board>();
        block = new Block[board.W, board.H];
        Generate();
    }
    public struct Block
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

    public Block[] piece = new Block[4]
    {
        new Block(),
        new Block(),
        new Block(),
        new Block(),
    };
    
    public Block[,] block;

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
    
    public void Generate()
    {
        if (board.GameOver()) 
            board.enabled = false;
        dropSpeed = 0.4f;
        choiceFigure = Random.Range(0, shapes.GetLength(0));
        //choiceFigure = 6;
        for (int i = 0; i < 4; i++)
        { 
            piece[i].x = shapes[choiceFigure, i] % 2;
            piece[i].y = - shapes[choiceFigure, i] / 2;
        }
        Sprite sprite = blockSprite[Random.Range(0, blockSprite.Length)];
        for (int i = 0; i < 4; i++)
        {
            piece[i].obj = Instantiate(blockPrefab, new Vector2(piece[i].x, piece[i].y), Quaternion.identity);
            SpriteRenderer sr = piece[i].obj.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
        }
        board.Move(4, 0);
    }
}
