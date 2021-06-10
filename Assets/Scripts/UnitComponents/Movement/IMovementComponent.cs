using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UnitComponents.Movement
{
    public interface IMovementComponent
    {
        Vector3 CurrentTargetTilePosition { get; set; }
        HexTile CurrentTargetTile { get; set; }
        bool IsMovingToAttack { get; set; }
        bool HasFinishedMoving { get; set; }
        UnitBehaviour ParentUnitBehaviour { get; set; }

        Path<HexTile> GetBestPath(HexTile start, HexTile finish);
        void InitializeMoving(HexBehaviour targetHexBehaviour);
        void StartMovingAlongPath(List<HexTile> path, bool isMovingToAttack = false);
        void Move();
        void InitializeComponent(UnitBehaviour unitBehaviour);
    }
}
