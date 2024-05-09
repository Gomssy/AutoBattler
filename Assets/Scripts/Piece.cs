using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour
{
    public BoardManager boardManager;
    public int maxHealth = 5;

    private GameState gameState;
    public Vector3 defaultPos;
    public Vector3 currentPos => transform.position;
    public Vector3 offset = new Vector3(0.5f, 0.5f, 0f);

    public bool isDragging = false;
    public bool canPlace = true;

    public void OnMouseDown()
    {
        isDragging = true;
        defaultPos = transform.position;
    }
    public void OnMouseDrag()
    {
        if(isDragging)
        {
            Vector3 curPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            Vector3 curPos = Camera.main.ScreenToWorldPoint(curPoint);
            curPos.z = 0f;
            transform.position = curPos;
        }
    }

    public void OnMouseUp()
    {
        if(canPlace)
        {
            var curCellPos = Vector3Int.RoundToInt(currentPos - offset);

            if (currentPos.x - curCellPos.x > offset.x)
                curCellPos.x += (int)(2 * offset.x) ;
            if (currentPos.y - curCellPos.y > offset.y)
                curCellPos.y += (int)(2 * offset.y);

            transform.position = curCellPos;
            Board newBoard = BoardManager.Inst.GetBoard(curCellPos);
            Board oldBoard = BoardManager.Inst.GetBoard(defaultPos);

            newBoard.pieceExist = true;
            if(oldBoard != null)
                oldBoard.pieceExist = false;
        }
        else
            transform.position = defaultPos;
    }
}

    public enum PieceState
{
    Idle,
    Move,
    Attack,
    Dead
}
