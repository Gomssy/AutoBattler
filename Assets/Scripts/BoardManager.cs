using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    public static int max_x { get; set; } = 10;
    public static int max_y { get; set; } = 5;

    public Board boardPrefab;
    public List<Board> boards = new List<Board>();

    private void Awake()
    {
        InitBoard();
    }

    private void InitBoard()
    {
        for(int x = 0; x < max_x; x++)
        {
            for(int y = 0; y < max_y; y++)
            {
                var board = CreateBoard(x, y);
                boards.Add(board);
            }
        }
    }

    private Board CreateBoard(int x, int y)
    {
        var board = Instantiate(boardPrefab);
        board._x = x; board._y = y;
        board.transform.SetParent(transform, false);
        board.transform.position = new Vector3(x,y,0f);
        board.name = new Vector2Int(x, y).ToString();

        return board;
    }

    public Board GetBoard(Vector3 pos)
    {
        foreach(var board in boards)
        {
            if(Vector3Int.RoundToInt(board.transform.position) == Vector3Int.RoundToInt(pos))
                return board;
        }
        return null;
    }

}
