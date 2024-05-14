using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Graph
{
    private List<Node> nodes;
    private List<Edge> edges;

    public List<Node> Nodes => nodes;
    public List<Edge> Edges => edges;

    public Graph() 
    {
        nodes = new List<Node>();
        edges = new List<Edge>();
    }

    public bool Adjacent(Node from, Node to)
    {
        foreach(Edge e in edges)
        {
            if(e.from == from && e.to == to)
                return true;
        }
        return false;
    }

    public List<Node> Neighbors(Node from)
    {
        List<Node> results = new List<Node>();
        foreach(Edge e in edges)
        {
            if(e.from == from)
                results.Add(e.to);
        }
        return results;
    }

    public void AddNode(Vector3 worldPos)
    {
        nodes.Add(new Node(nodes.Count, worldPos));
    }

    public void AddEdge(Node from, Node to)
    {
        edges.Add(new Edge(from, to, 1));
    }

    public float Distance(Node from, Node to)
    {
        foreach(Edge e in edges)
        {
            if(e.from == from && e.to == to)
                return e.GetWeight();
        }
        return Mathf.Infinity;
    }

    public virtual List<Node> GetShortestPath(Node start, Node end)
    {
        List<Node> path = new List<Node>();

        if(start == end)
        {
            path.Add(start);
            return path;
        }
        // unvisited nodes
        List<Node> unvisited = new List<Node>();
        // prev nodes from source
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        // distance
        Dictionary<Node, float> dists = new Dictionary<Node, float>();

        for(int i = 0; i < nodes.Count; i++)
        {
            Node node = nodes[i];
            unvisited.Add(node);
            // Set Node distance to Infinity at first
            dists.Add(node, float.MaxValue);
        }

        dists[start] = 0f;
        while(unvisited.Count > 0)
        {
            //sort nodes by smallest distance
            unvisited = unvisited.OrderBy(node => dists[node]).ToList();
            Node curNode = unvisited[0];
            unvisited.Remove(curNode);

            if(curNode == end)
            {
                while(prev.ContainsKey(curNode))
                {
                    path.Insert(0, curNode);
                    curNode = prev[curNode];
                }

                path.Insert(0, curNode);
                break;
            }

            foreach(Node neighbor in Neighbors(curNode))
            {
                float length = Vector3.Distance(curNode.worldPos, neighbor.worldPos);
                float alt = dists[curNode] + length;

                if(alt < dists[neighbor])
                {
                    dists[neighbor] = alt;
                    prev[neighbor] = curNode;
                }
            }
        }
        return path;
    }
}

public class Node
{
    public int idx;
    public Vector3 worldPos;

    private bool occupied = false;

    public Node(int idx, Vector3 worldPos)
    {
        this.idx = idx;
        this.worldPos = worldPos;
        occupied = false;
    }

    public void SetOccupied(bool value)
    {
        occupied = value;
    }

    public bool IsOccupied => occupied;
}

public class Edge
{
    public Node from;
    public Node to;

    private float weight;

    public Edge(Node from, Node to, float weight)
    {
        this.from = from;
        this.to = to;
        this.weight = weight;
    }

    public float GetWeight()
    {
        if(to.IsOccupied)
        {
            return Mathf.Infinity;
        }
        return weight;
    }
}