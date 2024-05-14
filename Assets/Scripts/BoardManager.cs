using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    public static int max_x { get; set; } = 10;
    public static int max_y { get; set; } = 5;

    public Board boardPrefab;
    private List<Board> boards = new List<Board>();

    protected Graph graph;
    protected Dictionary<Team, int> startPositionPerTeam = new Dictionary<Team, int>();

    private void Awake()
    {
        InitBoard();
        InitGraph();
        startPositionPerTeam.Add(Team.Ally, 0);
        startPositionPerTeam.Add(Team.Enemy, graph.Nodes.Count - 1);


    }

    private void InitBoard()
    {
        for (int x = 0; x < max_x; x++)
        {
            for (int y = 0; y < max_y; y++)
            {
                CreateBoard(x, y);
            }
        }
    }

    private Board CreateBoard(int x, int y)
    {
        var board = Instantiate(boardPrefab);
        board._x = x; board._y = y;
        board.transform.SetParent(transform, false);
        board.transform.position = new Vector3(x, y, 0f);
        board.name = new Vector2Int(x, y).ToString();
        boards.Add(board);

        return board;
    }

    public Node GetFreeNode(Team team)
    {
        int startIdx = startPositionPerTeam[team];
        int curIdx = startIdx;

        while (graph.Nodes[curIdx].IsOccupied)
        {
            if(startIdx == 0)
            {
                curIdx++;
                if (curIdx == graph.Nodes.Count)
                    return null;
            }
            else
            {
                curIdx--;
                if (curIdx == -1)
                    return null;
            }
        }
        return graph.Nodes[curIdx];
    }

    public Node GetRandNode()
    {
        int curIdx = UnityEngine.Random.Range(25, 50);
        while (graph.Nodes[curIdx].IsOccupied)
        {
            int randIdx = UnityEngine.Random.Range(25, 50);
            curIdx = randIdx;
        }
        return graph.Nodes[curIdx];
    }

    public List<Node> GetPath(Node from, Node to)
    {
        return graph.GetShortestPath(from, to);
    }

    public List<Node> GetNodesNear(Node to)
    {
        return graph.Neighbors(to);
    }

    public Node GetNodeForBoard(Board board)
    {
        var allNodes = graph.Nodes;
        for(int i = 0; i < allNodes.Count; ++i)
        {
            if(board.transform.GetSiblingIndex() == allNodes[i].idx)
                return allNodes[i];
        }
        return null;
    }

    public List<Node> GetSurroundingNodes(Node center, int radius)
    {
        List<Node> surroundings = new List<Node>();

        foreach(Node node in graph.Nodes)
        {
            if (Vector3.Distance(node.worldPos, center.worldPos) <= Mathf.Sqrt(2) && node != center)
            {
                surroundings.Add(node);
            }
        }

        return surroundings;
    }

    public Piece GetPieceOnNode(Node node)
    {
        foreach(Piece piece in GameManager.Inst.allyPieces)
        {
            if(piece.CurNode == node)
                return piece;
        }
        foreach(Piece piece in GameManager.Inst.enemyPieces)
        {
            if (piece.CurNode == node)
                return piece;
        }

        return null;
    }

    private void InitGraph()
    {
        graph = new Graph();
        for(int i = 0; i < boards.Count; i++)
        {
            Vector3 pos = boards[i].transform.position;
            graph.AddNode(pos);
        }

        var allNodes = graph.Nodes;
        foreach(Node from in allNodes)
        {
            foreach(Node to in allNodes)
            {
                if (Vector3.Distance(from.worldPos, to.worldPos) <= 1f && from != to)
                    graph.AddEdge(from, to);
            }
        }
    }
}