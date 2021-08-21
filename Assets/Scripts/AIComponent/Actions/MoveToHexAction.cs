﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.AIComponent
{
    internal class MoveToHexAction : IAction
    {
        #region Fields
        private HexBehaviour chosenTargetHex;
        private float scoredValue;
        private bool scoreCalculated = false;
        #endregion Fields

        #region Props
        public int SimulatedConsidValue { get; set; }

        public float ScoredValue
        {
            get
            {
                if (!scoreCalculated)
                {
                    scoredValue = GetScore();
                    scoreCalculated = true;
                }

                return scoredValue;
            }
        }

        public HexBehaviour ChosenTargetHex
        {
            get { return chosenTargetHex; }
            set { chosenTargetHex = value; }
        }

        public UnitBehaviour ActionOwner { get; set; }

        public List<IConsideration> Considerations { get; set; }
        public ActionType ActionType { get { return ActionType.Move; } }

        public int SimulatedValue { get; set; }
        #endregion Props



        #region ctor
        public MoveToHexAction(UnitBehaviour Owner,HexBehaviour hexBehaviour)
        {
            ActionOwner = Owner;
            ChosenTargetHex = hexBehaviour;
        }
        #endregion ctor

        #region Methods
        public void DoAction()
        {
            Debug.Log(string.Format("Enemy {0}, moves to hex with coordinates {1}", (ActionOwner).ToString(), (chosenTargetHex.OwningTile).ToString()));
            BattlefieldManager.ManagerInstance.DestinationTile = ChosenTargetHex;
            ActionManager.Instance.StartUnitActionOnHex(ChosenTargetHex);
        }
        
        public float GetScore()
        {
            return 0.5f;
        }

        public int Simulate(SimulatedUnit SimActionOwner, SimulatedUnit target)
        {
            int sum = 0;
            HexBehaviour hex = target.SimulatedHexBehaviour;
            if (SimActionOwner.UnitBehaviour.AttackType == AttackType.Ranged && hex.OwningTile.HighGround)
            {
                sum += 1;
            }

            return sum;
        }

        public void Print()
        {
            string s = "Move: "+ActionOwner.ToString() + " moves to hex " + ChosenTargetHex.OwningTile.ToString();
            Debug.Log(s);
        }

        #endregion Methods

    }
}
