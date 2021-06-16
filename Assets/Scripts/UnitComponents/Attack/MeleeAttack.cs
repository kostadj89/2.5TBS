using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void InitializeComponent(UnitBehaviour unitBehaviour)
        {
            ParentUnitBehaviour = unitBehaviour;
        }

        public void StartAttack()
        {
            //
            ParentUnitBehaviour.CurrentState = UnitState.Attacking;
            //we show targeted unit's ui, and damage it
            ((UnitBehaviour)TargetOfAttack).TakeDamage(ParentUnitBehaviour.Damage);

            //damage attacking unit with relation strike damage
            ParentUnitBehaviour.TakeDamage((int)(((UnitBehaviour)TargetOfAttack).Damage * 0.5));

            TargetOfAttack = null;
        }
    }
}
