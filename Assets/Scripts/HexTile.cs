using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class HexTile : GridObject, IHasNeighbours<HexTile>
{
    public bool Passable;
    public bool IsInRange;
    public bool Occupied;
    public bool Hazadours;
    public bool Cover;
    public bool HighGround;

    public HexTile(int x, int y)
        :base(x, y)
    {
        Passable = true;
        Occupied = false;
        Hazadours = false;
        Cover = false;
        HighGround = false;

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

    public IEnumerable<HexTile> AllNeighbours;

    public IEnumerable<HexTile> InRangeNeighbours => AllNeighbours.Where(o => o.Passable && o.IsInRange);// && !o.Occupied);
    public IEnumerable<HexTile> ReachableNeighbours => AllNeighbours.Where(o => o.Passable && o.IsInRange && !o.Occupied);

    //fills each tile with data for its neighbours
    public void FindNeighbours(Dictionary<Point, HexBehaviour> board, Vector2 boardSize)
    {
        List<HexTile> neighbours = new List<HexTile>();

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

    public HexBehaviour GetHexBehaviour()
    {
        Point p = new Point(this.X, this.Y);
        return BattlefieldManager.ManagerInstance.GeTileBehaviourFromPoint(p);
    }

    public bool HexTileBetweenTiles(HexTile t1, HexTile t2)
    {
        int minX, maxX, minY, maxY, sumT1,sumT2, tempSum;

        minX = t1.X <= t2.X ? t1.X : t2.X;
        maxX = t1.X <= t2.X ? t2.X : t1.X;

        minY = t1.Y <= t2.Y ? t1.Y : t2.Y;
        maxY = t1.Y <= t2.Y ? t2.Y : t1.Y;

        sumT1 = t1.X + t1.Y;
        sumT2 = t2.X + t2.Y;

        if (sumT1>=sumT2)
        {
            tempSum = sumT1;
            sumT1 = sumT2;
            sumT2 = tempSum;
        }

        if ((this.X>= minX) && (this.X <= maxX) && (this.Y>=minY) && (this.Y<=maxY) && (sumT1<= X + Y) && (sumT2>= X+Y))
        {
            return true;
        }

        return false;
    }
}
