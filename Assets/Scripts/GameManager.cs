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


}

public enum Team
{
    Ally,
    Enemy
}