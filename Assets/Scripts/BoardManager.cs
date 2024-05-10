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

    public Piece temp;
    public Piece temp2;


    private void Awake()
    {
        InitBoard();
        InitGraph();
        startPositionPerTeam.Add(Team.Ally, 0);
        startPositionPerTeam.Add(Team.Enemy, graph.Nodes.Count - 1);

        temp.Setup(Team.Ally, BoardManager.Inst.GetFreeNode(Team.Ally));
        GameManager.Inst.allyPieces.Add(temp);
        temp2.Setup(Team.Enemy, GetFreeNode(Team.Enemy));
        GameManager.Inst.enemyPieces.Add(temp2);
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

    public int fromIndex = 0;
    public int toIndex = 0;

    private void OnDrawGizmos()
    {
        if (graph == null)
            return;

        var allEdges = graph.Edges;
        if (allEdges == null)
            return;

        foreach (Edge e in allEdges)
        {
            Debug.DrawLine(e.from.worldPos, e.to.worldPos, Color.black, 100);
        }

        var allNodes = graph.Nodes;
        if (allNodes == null)
            return;

        foreach (Node n in allNodes)
        {
            Gizmos.color = n.IsOccupied ? Color.red : Color.green;
            Gizmos.DrawSphere(n.worldPos, 0.1f);
        }

        if (fromIndex >= allNodes.Count || toIndex >= allNodes.Count)
            return;

        List<Node> path = graph.GetShortestPath(allNodes[fromIndex], allNodes[toIndex]);
        if (path.Count > 1)
        {
            for (int i = 1; i < path.Count; i++)
            {
                Debug.DrawLine(path[i - 1].worldPos, path[i].worldPos, Color.red, 10);
            }
        }
    }

}