using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.AIComponent.Considerations
{
    internal class SelfGetsKilledByRetaliation : ConsiderationBase
    {
        public SelfGetsKilledByRetaliation(UnitBehaviour Owner, HexBehaviour targetHex):base(Owner,ConsiderationInputType.SelfHealth,targetHex,0,0,0,0,GraphType.Custom)
        {
            
        }

        public float ConsiderationInputValue { get;set;}
        public ConsiderationInputType ConsiderationInputType { get;set;}
        public GraphType GraphType { get;set;}
        public float K { get;set;}
        public float M { get;set;}



        public override float Score()
        {
            UnitBehaviour ub = (UnitBehaviour) targetHexContex.ObjectOnHex;
            if (ub.Damage/2f>=OwnerOFConsideration.CurrentHealth)
            {
                return 0.1f;
            }
            else
            {
                return 1;
            }
            
        }
    }
}
