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
    public static Path<HexTile> FindPath(HexTile start, HexTile destination)
    {
        bool DestinationIsOcupied = destination.Occupied;

        destination.Occupied = false;
        HashSet<HexTile> closed = new HashSet<HexTile>();

        PriorityQueue<double, Path<HexTile>> queue = new PriorityQueue<double,Path<HexTile>>();
        queue.Enqueue(0, new Path<HexTile>(start));

        while (!queue.IsEmpty)
        {
            Path<HexTile> path = queue.Dequeue();

            if (closed.Contains(path.LastStep))
                continue;

            if (path.LastStep.Equals(destination))
            {
                destination.Occupied = DestinationIsOcupied;
                return path;
            }

            closed.Add(path.LastStep);

            foreach (HexTile tile in path.LastStep.AvailableNeighbours)
            {
                double d = Distance(path.LastStep, tile);
                Path<HexTile> newPath = path.AddStep(tile, d+ (tile.Hazadours ? 1.5 : 0));
                queue.Enqueue(newPath.TotalCost + Estimate(tile,destination), newPath);
            }
        }

        destination.Occupied = DestinationIsOcupied;
        return null;

    }

    private static double Estimate(HexTile tile1, HexTile tile2)
    {
        return 1;//tile1.Hazadours?15:1;
    }

    private static double Distance(HexTile tile, HexTile destTile)
    {
        float dx = Mathf.Abs(destTile.X - tile.X);
        float dy = Mathf.Abs(destTile.Y - tile.Y);

        int z1 = -(tile.X + tile.Y);
        int z2 = -(destTile.X + destTile.Y);
        float dz = Mathf.Abs(z2 - z1);

        return Mathf.Max(dx, dy, dz);
    }
}
