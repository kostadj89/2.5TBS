using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.AIComponent.Considerations
{
    class ConsiderEnemyHealth_Con : ConsiderationBase
    {
        public ConsiderEnemyHealth_Con(HexBehaviour targetHex)
        :this(ConsiderationInputType.TargetHealth, targetHex, 1, 1,0.1f,0, GraphType.Linear)
        {

        }

        public ConsiderEnemyHealth_Con(ConsiderationInputType CI, HexBehaviour targetHex, float K, float M, float B, float C, GraphType GraphType)
            : base(CI, targetHex, K,M,B,C,GraphType)
        {
            
        }
        public float ConsiderationInputValue { get; set; }

        public ConsiderationInputType ConsiderationInputType
        {
            get { return ConsiderationInputType.TargetHealth; }
        }

        public GraphType GraphType { get; set; }
        public float K { get; set; }
        public float M { get; set; }
       
    }
}
