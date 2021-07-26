using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.UnitComponents.Attack
{
    class MeleeAttack : IAttackComponent
    {
        public UnitBehaviour ParentUnitBehaviour { get ; set; }
        public ITakesDamage TargetOfAttack { get; set; }
        public int AttackRange {
            get { return 1; }

            set { }

        }

        public float DamageModifier { get; set; }

        public void InitializeComponent(UnitBehaviour unitBehaviour)
        {
            ParentUnitBehaviour = unitBehaviour;
        }

        public List<HexBehaviour> GetAttackableTilesInMeleeRange()
        {
            Vector3 curentTileVector = ParentUnitBehaviour.CurrentHexTile.UnitAnchorWorldPositionVector;
            return BattlefieldManager.ManagerInstance.GetTilesInRange(curentTileVector, AttackRange);
        }

        public List<HexBehaviour> GetAttackableTiles()
        {
            Vector3 curentTileVector = ParentUnitBehaviour.CurrentHexTile.UnitAnchorWorldPositionVector;

            List<HexBehaviour> reachableHexBehaviours = BattlefieldManager.ManagerInstance.GetTilesInRange(curentTileVector, ParentUnitBehaviour.movementRange);
            List<HexBehaviour> attackableHexBehaviours = new List<HexBehaviour>(reachableHexBehaviours);

            foreach (HexBehaviour hex in reachableHexBehaviours)
            {
                if (Vector3.Distance(curentTileVector,hex.UnitAnchorWorldPositionVector) == ParentUnitBehaviour.movementRange*BattlefieldManager.DISTANCE_BETWEEN_HEXES)
                {
                    foreach (HexTile hexTile in hex.OwningTile.AllNeighbours)
                    {
                        if (hexTile.Occupied)
                        {
                            attackableHexBehaviours.Add(hex);
                        }
                    }
                }
            }

            return attackableHexBehaviours;
        }

        public void StartAttack(ITakesDamage target)
        {
            TargetOfAttack = target;
            List<HexBehaviour> attackableTiles = GetAttackableTilesInMeleeRange();
            if (attackableTiles.Contains(((UnitBehaviour)TargetOfAttack).CurrentHexTile))
            {
                ParentUnitBehaviour.CurrentState = UnitState.Attacking;
                //we show targeted unit's ui, and damage it
                ((UnitBehaviour)TargetOfAttack).TakeDamage(ParentUnitBehaviour.Damage);

                //damage attacking unit with relation strike damage
                ParentUnitBehaviour.TakeDamage((int)(((UnitBehaviour)TargetOfAttack).Damage * 0.5));

                TargetOfAttack = null;

                // if i'm adding flanking it should be here
                //ParentUnitBehaviour.CurrentState = UnitState.Idle;
                ParentUnitBehaviour.SetIdleState();
                //ParentUnitBehaviour.CurrentHexTile.ChangeDestinationToThis();

                ActionManager.Instance.EndCurrentPlayingUnitTurn();
            }
            //
            else
            {
                ParentUnitBehaviour.MovementComponent.InitializeMoving(((UnitBehaviour)TargetOfAttack).CurrentHexTile);
            }
            
        }

        public bool AttackConditionFufilled(HexBehaviour targetHexBehaviour)
        {
            //((OwningTile.Passable && OwningTile.IsInRange) || (OwningTile.Occupied && OwningTile.ReachableNeighbours.Count() > 0))
            return ParentUnitBehaviour.HexContainsAnEnemy(targetHexBehaviour) &&
                   targetHexBehaviour.OwningTile.ReachableNeighbours.Count() > 0;
        }

        public float CalculateDamageModifiers()
        {
            throw new NotImplementedException();
        }

        public List<HexBehaviour> GetHexesInRangeOccupiedByEnemy()
        {
            throw new NotImplementedException();
        }
    }
}
