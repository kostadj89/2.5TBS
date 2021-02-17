using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Path<Node> : IEnumerable<Node>
{
    public Node LastStep { get; private set; }
    public Path<Node> PreviousSteps { get; private set; }
    public double TotalCost { get; private set; }

    public Path(Node lastNode, Path<Node> previousSteps, double totalCost)
    {
        LastStep = lastNode;
        PreviousSteps = previousSteps;
        TotalCost = totalCost;
    }

    public Path(Node start)
    : this(start, null,0)
    {
        
    }

    public Path<Node> AddStep(Node step, double stepCost)
    {
        return new Path<Node>(step, this, TotalCost + stepCost);
    }

    public IEnumerator<Node> GetEnumerator()
    {
        for (var p = this; p != null; p = p.PreviousSteps)
        {
            yield return p.LastStep;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class PriorityQueue<P, V>
{
    private readonly  SortedDictionary<P,Queue<V>> list = new SortedDictionary<P, Queue<V>>();

    public bool IsEmpty
    {
        get { return !list.Any(); }
    }

    public void Enqueue(P priority, V value)
    {
        Queue<V> q;

        if (!list.TryGetValue(priority,out q))
        {
            q = new Queue<V>();
            list.Add(priority,q);
        }

        q.Enqueue(value);
    }

    public V Dequeue()
    {
        var pair = list.First();
        var v = pair.Value.Dequeue();

        if (pair.Value.Count == 0)
        {
            list.Remove(pair.Key);
        }

        return v;
    }
 
}

public static class Pathfinder
{
    public static Path<Tile> FindPath(Tile start, Tile destination)
    {
        HashSet<Tile> closed = new HashSet<Tile>();

        PriorityQueue<double, Path<Tile>> queue = new PriorityQueue<double,Path<Tile>>();
        queue.Enqueue(0, new Path<Tile>(start));

        while (!queue.IsEmpty)
        {
            Path<Tile> path = queue.Dequeue();

            if (closed.Contains(path.LastStep))
                continue;

            if (path.LastStep.Equals(destination))
            {
                return path;
            }

            closed.Add(path.LastStep);

            foreach (Tile tile in path.LastStep.Neighbours)
            {
                double d = Distance(path.LastStep, tile);
                Path<Tile> newPath = path.AddStep(tile, d);
                queue.Enqueue(newPath.TotalCost + Estimate(tile,destination), newPath);
            }
        }

        return null;

    }

    private static double Estimate(Tile tile1, Tile tile2)
    {
        return 1;
    }

    private static double Distance(Tile tile, Tile destTile)
    {
        float dx = Mathf.Abs(destTile.X - tile.X);
        float dy = Mathf.Abs(destTile.Y - tile.Y);

        int z1 = -(tile.X + tile.Y);
        int z2 = -(destTile.X + destTile.Y);
        float dz = Mathf.Abs(z2 - z1);

        return Mathf.Max(dx, dy, dz);
    }
}
