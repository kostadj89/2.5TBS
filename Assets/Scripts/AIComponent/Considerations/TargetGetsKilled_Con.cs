using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.AIComponent.Considerations
{
    class TargetGetsKilled_Con : ConsiderationBase
    {
        public TargetGetsKilled_Con(UnitBehaviour Owner, HexBehaviour targetHex) : base(Owner,ConsiderationInputType.TargetHealth, targetHex, 0, 0, 0, 0, GraphType.Custom)
        {

        }

        public float ConsiderationInputValue { get;set;}
        public ConsiderationInputType ConsiderationInputType { get;set;}
        public GraphType GraphType { get;set;}
        public float K { get;set;}
        public float M { get;set;}

        public override float Score()
        {
            UnitBehaviour ub = (UnitBehaviour)targetHexContex.ObjectOnHex;
            if (OwnerOFConsideration.Damage >= ub.CurrentHealth )
            {
                return 1.5f;
            }
            else
            {
                return (1-(ub.CurrentHealth- OwnerOFConsideration.Damage)/ub.MaxHealth);
            }

        }
    }
}
