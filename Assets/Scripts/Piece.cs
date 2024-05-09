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

    public int health = 5;
    public bool alive => health > 0;
    public bool isRangeAttack;

    private Vector3 targetPos;
    public Vector3 TargetPos => targetPos;
    public Vector3 lastMove;
    public Piece target;

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
            return canDrag || CanPlace;
        }
        set
        {
            if(value == false)
            {
                canDrag = false;
                CanPlace = false;
            }
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        defaultPos = transform.position;
        if (tag == "Enemy")
            canPlace = false;
        else
            canPlace = true;
    }

    public void OnMouseDown()
    {
       if (!canClick) return;
    }
    public void OnMouseDrag()
    {
        if (CanPlace)
        {
            /*if (!BoardManager.Inst.placingMask.activeSelf)
                BoardManager.Inst.placingMask.SetActive(true);*/
            Vector3 curPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            Vector3 curPos = Camera.main.ScreenToWorldPoint(curPoint);
            curPos.z = 0f;
            transform.position = curPos;
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
            var curCellPos = CalcCurCellPos();
            if (boardManager.canPlace(curCellPos))
            {
                boardManager.PlacePiece(Vector3Int.RoundToInt(defaultPos), null);
                boardManager.PlacePiece(curCellPos, this);

                transform.position = curCellPos;
                defaultPos = transform.position;
                targetPos = transform.position;
                lastMove = TargetPos;
            }
            else
            {
                transform.position = defaultPos;
            }
        }
        
        if(CanPlace)
        {
            //boardManager.placingMask.SetActive(false);
            var curCellPos = CalcCurCellPos();
            if (boardManager.canPlace(curCellPos))
            {
                var addedPiece = boardManager.AddPiece(this);
                if (addedPiece != null)
                {
                    addedPiece.transform.position = curCellPos;
                    addedPiece.defaultPos = addedPiece.transform.position;
                    addedPiece.targetPos = addedPiece.transform.position;
                    addedPiece.lastMove = TargetPos;
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

    public Vector3Int CalcCurCellPos()
    {
        var curCellPos = Vector3Int.RoundToInt(currentPos - offset);
        if (currentPos.x - curCellPos.x > offset.x)
            curCellPos.x += (int)(2 * offset.x);
        if (currentPos.y - curCellPos.y > offset.y)
            curCellPos.y += (int)(2 * offset.y);

        return curCellPos;
    }
    public bool Move(Vector3 target)
    {
        if (pieceState is PieceState.Idle)
        {
            targetPos = target;
            lastMove = TargetPos;

            animator.SetBool("isMoving", true);
            pieceState = PieceState.Move;
            StartCoroutine("MoveStep");
            return true;
        }
        else
            return false;
    }

    IEnumerator MoveStep()
    {
        var percent = 0f;
        if (target.transform.position.x < transform.position.x)
            GetComponent<SpriteRenderer>().flipX = true;

        while(pieceState is PieceState.Move)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, percent);
            if (percent >= 1f)
            {
                transform.position = targetPos;
                pieceState = PieceState.Idle;
                animator.SetBool("Stop", true);
                yield break;
            }
            else
                yield return null;
        }
        yield break;
    }
}

    public enum PieceState
{
    Idle,
    Move,
    Attack,
    Dead
}
