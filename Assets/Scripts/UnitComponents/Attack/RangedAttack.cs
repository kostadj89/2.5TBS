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
    class RangedAttack : IAttackComponent
    {
        public UnitBehaviour ParentUnitBehaviour { get; set; }
        public ITakesDamage TargetOfAttack { get; set; }
        public int AttackRange
        {
            get { return 4; }

            set { }

        }

        public void InitializeComponent(UnitBehaviour unitBehaviour)
        {
            ParentUnitBehaviour = unitBehaviour;
        }

        public List<HexBehaviour> GetAttackableTiles()
        {
            Vector3 curentTileVector = ParentUnitBehaviour.CurrentHexTile.UnitAnchorWorldPositionVector;
            return BattlefieldManager.ManagerInstance.GetTilesInRange(curentTileVector, AttackRange);
        }

        public void StartAttack(ITakesDamage target)
        {
            TargetOfAttack = target;
            List<HexBehaviour> attackableTiles = GetAttackableTiles();
            if (attackableTiles.Contains(((UnitBehaviour)TargetOfAttack).CurrentHexTile))
            {
                ParentUnitBehaviour.CurrentState = UnitState.Attacking;
                //we show targeted unit's ui, and damage it
                ((UnitBehaviour)TargetOfAttack).TakeDamage(ParentUnitBehaviour.Damage);

                //damage attacking unit with relation strike damage

                TargetOfAttack = null;

                // if i'm adding flanking it should be here
                //ParentUnitBehaviour.CurrentState = UnitState.Idle;
                ParentUnitBehaviour.SetIdleState();
                //ParentUnitBehaviour.CurrentHexTile.ChangeDestinationToThis();

                ActionManager.Instance.EndCurrentPlayingUnitTurn();
            }
        }

        public bool AttackConditionFufilled(HexBehaviour targetHexBehaviour)
        {
            return GetAttackableTiles().Contains(targetHexBehaviour) && ParentUnitBehaviour.HexContainsAnEnemy(targetHexBehaviour);
        }
    }
}
