using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UnitComponents.Movement
{
    public class GroundMovement : IMovementComponent
    {
        #region Const
        private const float MIN_NEXT_TILE_DIST = 0.07f;
        #endregion Const

        #region Private fields

        private List<HexTile> path;
        private Transform parentTransform;
        //speed in meters per second
        private float speed = 0.0025f;
        //probably will ignore this
        private float rotationSpeed = 0.004f;

        #endregion Private fields

        #region Interface

        //tile and position of the tile we are moving to 
        public Vector3 CurrentTargetTilePosition { get; set; }
        public HexTile CurrentTargetTile { get; set; }

        public bool IsMovingToAttack { get; set; }
        public UnitBehaviour ParentUnitBehaviour { get; set; }
        public bool HasFinishedMoving { get; set; }

        public Path<HexTile> GetBestPath(HexTile start, HexTile finish)
        {
            return Pathfinder.FindPath(start, finish);
        }

        public void InitializeMoving(HexBehaviour targetHexBehaviour)
        {
            //drawing path
            BattlefieldManager.ManagerInstance.GenerateAndShowPath();

            var path = GetBestPath(BattlefieldManager.ManagerInstance.StartingTile.OwningTile, BattlefieldManager.ManagerInstance.DestinationTile.OwningTile);

            HasFinishedMoving = false;

            if (BattlefieldManager.ManagerInstance.DestinationTile.OwningTile.Occupied)
            {
                UnitBehaviour ub = (UnitBehaviour)BattlefieldManager.ManagerInstance.DestinationTile.ObjectOnHex;

                if (ub.PlayerId != ParentUnitBehaviour.PlayerId)
                {
                    BattlefieldManager.ManagerInstance.DestinationTile.ChangeHexVisualToOccupied();
                    ParentUnitBehaviour.AttackComponent.TargetOfAttack = ub;

                    //path = path.PreviousSteps;

                    BattlefieldManager.ManagerInstance.DestinationTile = path == null ? BattlefieldManager.ManagerInstance.StartingTile : path.LastStep.GetHexBehaviour();
                    Debug.Log("InitializeMoving(HexBehaviour targetHexBehaviour), DestinationTile: " + (BattlefieldManager.ManagerInstance.DestinationTile ? BattlefieldManager.ManagerInstance.DestinationTile.coordinates : "null"));
                    //we color the selected path to real white
                    BattlefieldManager.ManagerInstance.DestinationTile.ChangeVisualToSelected();
                    StartMovingAlongPath(path.ToList(), true);
                }
            }
            else
            {
                //we color the selected path to real white
                targetHexBehaviour.ChangeVisualToSelected();
                StartMovingAlongPath(path.ToList());
            }
        }

        public void StartMovingAlongPath(List<HexTile> path, bool isMovingToAttack = false)
        {
            if (path.Count == 0)
            {
                ParentUnitBehaviour.SetIdleState();
                IsMovingToAttack = false;
                return;
            }

            //the first tile we need to reach is actually path[path.Count - 2], near the end of the list just before the one the unit is currently on, the unit is on path[path.Count - 1];
            if (isMovingToAttack && path.Count == 1)
            {
                CurrentTargetTile = path[0];
            }
            else
            {
                CurrentTargetTile = path[path.Count - 2];
            }

            CurrentTargetTilePosition = BattlefieldManager.ManagerInstance.GetUnitAnchorFromATileOnBoard(CurrentTargetTile.VectorLocation());
            ParentUnitBehaviour.SetMovingState();
            IsMovingToAttack = isMovingToAttack;
            this.path = path;
        }

        public void Move()
        {
            //if the distance between the character and the center of the next tile is short enough
            if ((CurrentTargetTilePosition - parentTransform.position).sqrMagnitude < MIN_NEXT_TILE_DIST * MIN_NEXT_TILE_DIST)
            {
                //if we reach the destination tile
                if (path.IndexOf(CurrentTargetTile) == 0)
                {
                    //if (IsMovingToAttack)
                    //{
                    //    IsMovingToAttack = false;
                    //    StartAttack();
                    //}
                    //else
                    //{
                    //    EndCurrentPlayingUnitTurn();
                    //}
                    HasFinishedMoving = true;

                    return;
                }

                //else current target tile becomes the next tile from the list
                CurrentTargetTile = path[path.IndexOf(CurrentTargetTile) - 1];
                CurrentTargetTilePosition = BattlefieldManager.ManagerInstance.GetUnitAnchorFromATileOnBoard(CurrentTargetTile.VectorLocation());
            }

            //MoveTowardsPosition(currentTargetTilePosition);
            parentTransform.position = Vector3.MoveTowards(parentTransform.position, CurrentTargetTilePosition, Time.deltaTime * speed);
        }

        public void InitializeComponent(UnitBehaviour unitBehaviour)
        {
            this.ParentUnitBehaviour = unitBehaviour;
            this.speed = unitBehaviour.speed;
            this.rotationSpeed = unitBehaviour.rotationSpeed;

            //caching parent transform
            parentTransform = ParentUnitBehaviour.transform;
        }

        #endregion Interface

    }
}
