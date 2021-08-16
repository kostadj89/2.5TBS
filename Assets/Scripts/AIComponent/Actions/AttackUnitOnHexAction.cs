using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.AIComponent.Considerations;
using UnityEngine;

namespace Assets.Scripts.AIComponent
{
    class AttackUnitOnHexAction : IAction
    {
        private HexBehaviour chosenTargetHex;
        private UnitBehaviour targetUnit;

        public UnitBehaviour TargetUnit
        {
            get
            {
                if (targetUnit == null)
                {
                    targetUnit = (UnitBehaviour) ChosenTargetHex.ObjectOnHex;
                }

                return targetUnit;
            }
        }
        public AttackUnitOnHexAction(HexBehaviour hex)
        {
            ChosenTargetHex = hex;
        }

        public HexBehaviour ChosenTargetHex
        {
            get { return chosenTargetHex; }
            set { chosenTargetHex = value;}
        }
        public List<IConsideration> Considerations
        {
            get { return GetConsiderations(); }
        }

        private List<IConsideration> GetConsiderations()
        {
           List<IConsideration> considerations = new List<IConsideration>();

           considerations.Add(new ConsiderEnemyHealth_Con(ChosenTargetHex));
           considerations.Add(new TargetGetsKilled_Con(ChosenTargetHex));
           considerations.Add(new SelfGetsKilledByRetaliation(ChosenTargetHex));

           return considerations;
        }

        public ActionType ActionType
        {
            get { return ActionType.Attack; }
        }

        public void DoAction()
        {
            Debug.Log(string.Format("Enemy {0}, attacks hex with coordinates {1}.\t Score: {2}", (AIAgent.AIAgentInstanceAgent.CurrentlyControledUnit).ToString(), (chosenTargetHex.OwningTile).ToString(),GetScore()));
            BattlefieldManager.ManagerInstance.DestinationTile = ChosenTargetHex;
            ActionManager.Instance.StartUnitActionOnHex(ChosenTargetHex);
        }

        public float GetScore()
        {
            float score = 1;
            foreach (IConsideration consideration in Considerations)
            {
                score *= consideration.Score();
            }

            return score;
        }
    }
}
