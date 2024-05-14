using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Action OnRoundStart;
    public Action OnRoundEnd;
    public Action<Piece> OnPieceDead;

    public List<Piece> allyPieces = new List<Piece>();
    public List<Piece> enemyPieces = new List<Piece>();

    public List<Piece> pieceList = new List<Piece>();

    [SerializeField]
    private int maxUnitCount = 4;
    [SerializeField]
    private Transform enemyParent;
    [SerializeField]
    private Transform allyParent;

    public GameState gameState;

    private void Start()
    {
        gameState = GameState.Prepare;
        SpawnPiece();
    }
    public List<Piece> GetEnemyPieces(Team enemy)
    {
        if (enemy == Team.Ally)
            return enemyPieces;
        else
            return allyPieces;
    }

    public void PieceDead(Piece piece)
    {
        allyPieces.Remove(piece);
        enemyPieces.Remove(piece);

        OnPieceDead?.Invoke(piece);

        Destroy(piece.gameObject);
    }

    public void SpawnPiece()
    {
        for(int i = 0; i < maxUnitCount; i++)
        {
            int rand = UnityEngine.Random.Range(0, pieceList.Count - 1);
            Piece enemy = Instantiate(pieceList[rand], enemyParent);
            enemy.GetComponent<SpriteRenderer>().color = Color.red;

            enemyPieces.Add(enemy);
            enemy.Init(Team.Enemy, BoardManager.Inst.GetRandNode());

            int rand2 = UnityEngine.Random.Range(0, pieceList.Count - 1);
            Piece ally = Instantiate(pieceList[rand2], allyParent);
            allyPieces.Add(ally);
            ally.Init(Team.Ally, BoardManager.Inst.GetFreeNode(Team.Ally));
        }

    }

    public void StartBattle()
    {
        gameState = GameState.Battle;
        OnRoundStart?.Invoke();
    }



}

public enum Team
{
    Ally,
    Enemy
}

public enum GameState
{
    Prepare,
    Battle,
    End
}