using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public struct Point
{
    public int X, Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public abstract class GridObject
{
    public Point Location;

    public int X
    {
        get { return Location.X; }
    }

    public int Y
    {
        get { return Location.Y; }
    }

    public GridObject(Point point)
    {
        Location = point;
    }

    public GridObject(int x, int y)
        :this(new Point(x, y))
    {
        
    }

    public override string ToString()
    {
        return "("+X+", " +Y+")";
    }

    public Vector2 VectorLocation()
    {
        return new Vector2(X, Y);
    }
}

public interface IHasNeighbours<N>
{
    IEnumerable<N> InRangeNeighbours { get; }
}
