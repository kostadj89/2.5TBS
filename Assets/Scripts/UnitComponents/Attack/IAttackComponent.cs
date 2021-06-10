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

        void StartAttack();

        void InitializeComponent(UnitBehaviour unitBehaviour);
    }
}
