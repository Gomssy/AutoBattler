using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField]
    private GameObject startButton;

    public GameState gameState;
    private string resultText = "";
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private GameObject resultPage;

    private void Start()
    {
        gameState = GameState.Prepare;
        resultPage.SetActive(false);
        SpawnPiece();
    }

    private void Update()
    {
        if (allyPieces.Count <= 0 || enemyPieces.Count <= 0)
        {
            gameState = GameState.End;
            GameEnd();
        }

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
            int rand = UnityEngine.Random.Range(0, pieceList.Count);
            Piece enemy = Instantiate(pieceList[rand], enemyParent);
            enemy.GetComponent<SpriteRenderer>().color = Color.red;

            enemyPieces.Add(enemy);
            enemy.Init(Team.Enemy, BoardManager.Inst.GetRandNode());

            int rand2 = UnityEngine.Random.Range(0, pieceList.Count);
            Piece ally = Instantiate(pieceList[rand2], allyParent);
            allyPieces.Add(ally);
            ally.Init(Team.Ally, BoardManager.Inst.GetFreeNode(Team.Ally));
        }

    }

    public void StartBattle()
    {
        gameState = GameState.Battle;
        startButton.SetActive(false);
        OnRoundStart?.Invoke();
    }

    public void GameEnd()
    {
        resultPage.SetActive(true);
        resultText = allyPieces.Count > enemyPieces.Count ? "You Win!" : "You Lose...";
        text.text = resultText;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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