using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Tile : GridObject, IHasNeighbours<Tile>
{
    public bool Passable;
    public bool IsInRange;

    public Tile(int x, int y)
        :base(x, y)
    {
        Passable = true;
    }

    public static List<Point> NeighbourShift
    {
        get
        {
            return new List<Point>
            {
                new Point(0, 1),
                new Point(1, 0),
                new Point(1, -1),
                new Point(0, -1),
                new Point(-1, 0),
                new Point(-1, 1)
            };
        }
    }

    public IEnumerable<Tile> AllNeighbours;

    public IEnumerable<Tile> Neighbours => AllNeighbours.Where(o => o.Passable);

    //fills each tile with data for its neighbours
    public void FindNeighbours(Dictionary<Point, TileBehaviour> board, Vector2 boardSize)
    {
        List<Tile> neighbours = new List<Tile>();

        foreach (Point point in NeighbourShift)
        {
            int neighbourX = X + point.X;
            int neighbourY = Y + point.Y;

            int xOffset = neighbourY / 2;

            if (neighbourX >= 0 - xOffset &&
                neighbourX < (int) boardSize.x - xOffset &&
                neighbourY >= 0 && neighbourY < (int) boardSize.y)
            {
                Point p = new Point(neighbourX, neighbourY);
                neighbours.Add(board[p].OwningTile);
            }
               
        }

        AllNeighbours = neighbours;
    }
}
