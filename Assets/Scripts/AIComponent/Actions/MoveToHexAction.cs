using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.AIComponent
{
    internal class MoveToHexAction : IAction
    {
        private HexBehaviour chosenTargetHex;

        public HexBehaviour ChosenTargetHex
        {
            get { return chosenTargetHex; }
            set { chosenTargetHex = value; }
        }
        
        public List<IConsideration> Considerations { get;set;}
        public ActionType ActionType { get;set;}

        public MoveToHexAction(HexBehaviour hexBehaviour)
        {
            ChosenTargetHex = hexBehaviour;
        }

        public void DoAction()
        {
            Debug.Log(string.Format("Enemy {0}, moves to hex with coordinates {1}", (AIAgent.AIAgentInstanceAgent.CurrentlyControledUnit).ToString(), (chosenTargetHex.OwningTile).ToString()));
            BattlefieldManager.ManagerInstance.DestinationTile = ChosenTargetHex;
            ActionManager.Instance.StartUnitActionOnHex(ChosenTargetHex);
        }
        
        public float GetScore()
        {
            return 0.5f;
        }
    }
}
