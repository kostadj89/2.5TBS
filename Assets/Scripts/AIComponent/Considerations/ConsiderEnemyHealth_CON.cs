using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.AIComponent.Considerations
{
    class ConsiderEnemyHealth_Con : ConsiderationBase
    {
        public ConsiderEnemyHealth_Con(UnitBehaviour Owner, HexBehaviour targetHex)
        :this(Owner, ConsiderationInputType.TargetHealth, targetHex, 1, 1,0.1f,0, GraphType.Linear)
        {

        }

        public ConsiderEnemyHealth_Con(UnitBehaviour Owner, ConsiderationInputType CI, HexBehaviour targetHex, float K, float M, float B, float C, GraphType GraphType)
            : base(Owner, CI, targetHex, K,M,B,C,GraphType)
        {
            
        }

        public ConsiderationInputType ConsiderationInputType
        {
            get { return ConsiderationInputType.TargetHealth; }
        }
    }
}
