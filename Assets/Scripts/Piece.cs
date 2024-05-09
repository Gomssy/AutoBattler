using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour
{
    public BoardManager boardManager;
    private PieceState pieceState;
    public Animator animator;

    private Vector3 defaultPos;
    public Vector3 currentPos => transform.position;
    public Vector3 offset = new Vector3(0.5f, 0.5f, 0f);

    public bool canDrag = false;
    private bool canPlace;

    public bool CanPlace
    {
        get
        {
            return canPlace;
        }
        set
        {
            canPlace = value;
        }
    }

    public bool canClick
    {
        get
        {
            return canDrag || canPlace;
        }
        set
        {
            if(value == false)
            {
                canDrag = false;
                canPlace = false;
            }
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        defaultPos = transform.position;
    }

    public void OnMouseDown()
    {
       // if (!canClick) return;
        canDrag = true;
        defaultPos = transform.position;
    }
    public void OnMouseDrag()
    {
        if (canPlace)
        {
            if (!BoardManager.Inst.placingMask.activeSelf)
                BoardManager.Inst.placingMask.SetActive(true);
        }
        if (canDrag)
        {
            Vector3 curPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            Vector3 curPos = Camera.main.ScreenToWorldPoint(curPoint);
            curPos.z = 0f;
            transform.position = curPos;
        }

    }

    public void OnMouseUp()
    {
        if(canDrag)
        {
            var curCellPos = Vector3Int.RoundToInt(currentPos - offset);
            if (currentPos.x - curCellPos.x > offset.x)
                curCellPos.x += (int)(2 * offset.x);
            if (currentPos.y - curCellPos.y > offset.y)
                curCellPos.y += (int)(2 * offset.y);

            if (boardManager.canPlace(curCellPos))
            {
                boardManager.PlacePiece(Vector3Int.RoundToInt(defaultPos), null);
                boardManager.PlacePiece(curCellPos, this);

                transform.position = curCellPos;
                defaultPos = transform.position;
            }
            else
            {
                transform.position = defaultPos;
            }
        }
        
        if(canPlace)
        {
            boardManager.placingMask.SetActive(false);
            var curCellPos = Vector3Int.RoundToInt(currentPos - offset);
            if(boardManager.canPlace(curCellPos))
            {
                var addedPiece = boardManager.AddPiece(this);
                if (addedPiece != null)
                {
                    addedPiece.transform.position = curCellPos;
                    addedPiece.defaultPos = addedPiece.transform.position;
                    addedPiece.canPlace = false;
                    addedPiece.canDrag = true;
                    boardManager.PlacePiece(curCellPos, addedPiece);
                }
                else
                    transform.position = defaultPos;
            }
            else
                transform.position= defaultPos;
        }
    }
}

    public enum PieceState
{
    Idle,
    Move,
    Attack,
    Dead
}
