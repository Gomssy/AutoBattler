using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public Vector3 dragOffset = new Vector3(0f, 0.5f, 0f);
    public RectInt mask = new RectInt(0, 0, 10, 5);

    private Vector3 prevPos;
    private Board prevBoard = null;
    private SpriteRenderer spriteRenderer;
    private int prevSortingOrder;
    private Camera cam;

    public bool isDragging = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
    }

    public void OnDragStart()
    {
        if(GameManager.Inst.gameState is GameState.Prepare && GetComponent<Piece>().myTeam is Team.Ally)
        {
            prevPos = transform.position;
            prevSortingOrder = spriteRenderer.sortingOrder;

            spriteRenderer.sortingOrder = 20;
            isDragging = true;
        }

    }

    public void OnDragging()
    {
        if (!isDragging)
            return;

        Vector3 newPos = cam.ScreenToWorldPoint(Input.mousePosition) + dragOffset;
        newPos.z = 0f;
        transform.position = newPos;

        Board boardUnder = GetBoardUnder();
        if (boardUnder != null )
        {
            boardUnder.SetHighlight(true, !(BoardManager.Inst.GetNodeForBoard(boardUnder).IsOccupied || BoardManager.Inst.GetNodeForBoard(boardUnder).idx >= 25));

            if (prevBoard != null && boardUnder != prevBoard)
                prevBoard.SetHighlight(false, false);
            prevBoard = boardUnder;
        }

    }

    public void OnDragEnd()
    {
        if(!isDragging) return;

        if (!TryRelease())
        {
            transform.position = prevPos;
        }

        if(prevBoard != null)
        {
            prevBoard.SetHighlight(false, false);
            prevBoard = null;
        }

        spriteRenderer.sortingOrder = prevSortingOrder;

        isDragging = false;
    }

    private bool TryRelease()
    {
        Board board = GetBoardUnder();
        if(board != null)
        {
            Piece piece = GetComponent<Piece>();
            Node candidateNode = BoardManager.Inst.GetNodeForBoard(board);
            if(candidateNode != null && piece != null && candidateNode.idx < 25)
            {
                if(!candidateNode.IsOccupied)
                {
                    piece.CurNode.SetOccupied(false);
                    piece.SetCurrentNode(candidateNode);
                    candidateNode.SetOccupied(true);
                    piece.transform.position = candidateNode.worldPos;
                    return true;
                }
            }
        }
        return false;
    }

    public Board GetBoardUnder()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        foreach(var hit in hits)
        {
            if(hit.collider != GetComponent<Collider2D>())
            {
                Board board = hit.collider.GetComponent<Board>();
                return board;
            }
        }
        return null;
    }

}
