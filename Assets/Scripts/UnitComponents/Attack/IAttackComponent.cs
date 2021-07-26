using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UnitComponents.Attack
{
    public interface IAttackComponent
    {
        UnitBehaviour ParentUnitBehaviour { get; set; }
        ITakesDamage TargetOfAttack { get; set; }
        int AttackRange { get; set; }
        float DamageModifier { get; set; }

        void StartAttack(ITakesDamage targetOfAttack);

        void InitializeComponent(UnitBehaviour unitBehaviour);

        List<HexBehaviour> GetAttackableTiles();
        List<HexBehaviour> GetHexesInRangeOccupiedByEnemy();

        bool AttackConditionFufilled(HexBehaviour targetHexBehaviour);

        float CalculateDamageModifiers();
    }
}
