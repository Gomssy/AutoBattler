using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    public static int max_x { get; set; } = 10;
    public static int max_y { get; set; } = 5;

    public Board boardPrefab;

    public GameObject placingMask;
    public bool[,] canPlacePiece;
    private bool[,] boards;
    private Piece[,] pieceLocations;
    public List<Piece> pieces;

    private GameState gameState;

    private void Awake()
    {
        InitBoard();
    }

    private void InitBoard()
    {
        boards = new bool[max_x, max_y];
        pieceLocations = new Piece[max_x, max_y];
        canPlacePiece = new bool[max_x, max_y];

        for(int x = 0; x < max_x; x++)
        {
            for(int y = 0; y < max_y; y++)
            {
                CreateBoard(x, y);
            }
        }
        canPlacePiece[1, 0] = false;
        canPlacePiece[9, 4] = false;
    }

    private Board CreateBoard(int x, int y)
    {
        var board = Instantiate(boardPrefab);
        board._x = x; board._y = y;
        board.transform.SetParent(transform, false);
        board.transform.position = new Vector3(x,y,0f);
        board.name = new Vector2Int(x, y).ToString();

        boards[x, y] = true;
        canPlacePiece[x, y] = true;
        pieceLocations[x, y] = null;

        return board;
    }

    public bool canPlace(Vector3Int pos)
    {
        return canPlacePiece[pos.x, pos.y] && boards[pos.x, pos.y] && pieceLocations[pos.x, pos.y] == null;
    }

    public bool PlacePiece(Vector3Int pos, Piece piece = null)
    {
        if (boards[pos.x, pos.y])
        {
            pieceLocations[pos.x, pos.y] = piece;
            return true;
        }
        return false;
    }

    public Piece AddPiece(Piece piece)
    {
        if(gameState == GameState.Idle)
        {
            pieces.Add(piece);
            return piece;
        }
        return null;
    }
}
