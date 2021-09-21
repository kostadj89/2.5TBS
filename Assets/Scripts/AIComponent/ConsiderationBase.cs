using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.AIComponent
{
    public abstract class ConsiderationBase : IConsideration
    {
        private float X=-1f;
        public ConsiderationBase(UnitBehaviour Owner,ConsiderationInputType CI, HexBehaviour hex, float K, float M, float B, float C, GraphType GraphType)
        {
            OwnerOFConsideration = Owner;
            ConsiderationInputType = CI;
            targetHexContex = hex;
            this.K = K;
            this.M = M;
            this.B = B;
            this.C = C;
            this.GraphType = GraphType;
        }

        public UnitBehaviour OwnerOFConsideration { get; set; }
        public HexBehaviour targetHexContex { get; set; }

        public float ConsiderationInputValue
        {
            get
            {
                if (X ==-1f)
                {
                    X = GetInputFromContext(ConsiderationInputType, OwnerOFConsideration, targetHexContex);
                }

                return X;
            }
        }

        public ConsiderationInputType ConsiderationInputType { get; set; }
        public GraphType GraphType { get; set; }

        public float K { get; set; }
        public float M { get; set; }
        public float C { get; set; }
        public float B { get; set; }
       

        public virtual float Score()
        {
            switch (GraphType)
            {
                case GraphType.Exponential:
                    return 0f;
                case GraphType.Logistic:
                    return K * (1.0f / (1.0f + Mathf.Pow((1000.0f * M * Mathf.Exp(1)), -ConsiderationInputValue + C))) + B; 
                case GraphType.Linear:
                    return M*Mathf.Pow(ConsiderationInputValue - C,K)+B;
                case GraphType.Quadratic:
                    return 0f;
            }

            return -1;
        }

        public static float GetInputFromContext(ConsiderationInputType c,UnitBehaviour Owner,HexBehaviour targetHexBehaviour)
        {
            float inputValue=-10;
            switch (c)
            {
                case ConsiderationInputType.CoverValue:
                    //temp
                    inputValue = 0.5f;
                    break;

                case ConsiderationInputType.EnemyTargetDistance:

                    inputValue = Vector3.Distance(Owner.transform.position,targetHexBehaviour.UnitAnchorWorldPositionVector);
                    break;

                case ConsiderationInputType.NearestAllyDistance:
                    //temp
                    inputValue = 0.5f;
                    break;

                case ConsiderationInputType.SelfHealth:

                    UnitBehaviour ub = Owner;
                    inputValue = (float) ub.CurrentHealth / (float) ub.MaxHealth;
                    break;

                case ConsiderationInputType.NearestEnemyDistance:
                    //temp
                    inputValue = 0.5f;
                    break;

                case ConsiderationInputType.TargetEnemyRetaliationStrike:
                    UnitBehaviour enemyUB1 = (UnitBehaviour)targetHexBehaviour.ObjectOnHex;

                    inputValue = (float)enemyUB1.Damage/2;
                    break;

                case ConsiderationInputType.TargetHealth:
                    UnitBehaviour enemyUB = (UnitBehaviour) targetHexBehaviour.ObjectOnHex;
                    if (enemyUB != null)
                        inputValue = enemyUB.CurrentHealth;
                    else
                        inputValue = 0;
                    break;
            }

            return inputValue;
        }
    }
}
