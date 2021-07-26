using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.AIComponent
{
    class AttackUnitOnHexAction : IAction
    {
        private HexBehaviour chosenTargetHex;
        public AttackUnitOnHexAction(HexBehaviour hex)
        {
            ChosenTargetHex = hex;
        }

        public HexBehaviour ChosenTargetHex
        {
            get { return chosenTargetHex; }
            set { chosenTargetHex = value;}
        }
        public List<Consideration> Considerations
        {
            get { return GetConsiderations(); }
        }

        private List<Consideration> GetConsiderations()
        {
            throw new NotImplementedException();
        }

        public ActionType ActionType
        {
            get { return ActionType.Attack; }
        }

        public void DoAction()
        {
            Debug.Log(string.Format("Enemy {0}, attacks hex with coordinates {1}", (AIAgent.AIAgentInstanceAgent.CurrentlyControledUnit).ToString(), (chosenTargetHex.OwningTile).ToString()));
            BattlefieldManager.ManagerInstance.DestinationTile = ChosenTargetHex;
            ActionManager.Instance.StartUnitActionOnHex(ChosenTargetHex);
        }

        public float GetScore()
        {
            return 0.75f;
        }
    }
}
