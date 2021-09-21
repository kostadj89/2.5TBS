using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.AIComponent.Considerations;
using Assets.Scripts.UnitComponents.Attack;
using UnityEngine;

namespace Assets.Scripts.AIComponent
{
    class AttackUnitOnHexAction : IAction
    {
        #region fields

        private HexBehaviour chosenTargetHex;
        private UnitBehaviour targetUnit;
        private float scoredValue;
        private bool scoreCalculated = false;

        #endregion fields
        #region props
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

        public UnitBehaviour TargetUnit
        {
            get
            {
                if (targetUnit == null)
                {
                    targetUnit = (UnitBehaviour)ChosenTargetHex.ObjectOnHex;
                }

                return targetUnit;
            }
        }

        public HexBehaviour ChosenTargetHex
        {
            get { return chosenTargetHex; }
            set { chosenTargetHex = value; }
        }
        public List<IConsideration> Considerations
        {
            get { return GetConsiderations(); }
        }
        public ActionType ActionType
        {
            get { return ActionType.Attack; }
        }

        public int SimulatedConsidValue { get; set; }
        

        public decimal SimulatedValue { get; set; }
        public UnitBehaviour ActionOwner { get; set; }
        #endregion props
        #region ctor

        public AttackUnitOnHexAction(UnitBehaviour Owner,HexBehaviour hex)
        {
            ActionOwner = Owner;
            ChosenTargetHex = hex;
        }

        #endregion ctor
        #region methods
        private List<IConsideration> GetConsiderations()
        {
            List<IConsideration> considerations = new List<IConsideration>();

            considerations.Add(new ConsiderEnemyHealth_Con(ActionOwner, ChosenTargetHex));
            considerations.Add(new TargetGetsKilled_Con(ActionOwner, ChosenTargetHex));
            considerations.Add(new SelfGetsKilledByRetaliation(ActionOwner, ChosenTargetHex));

            return considerations;
        }



        public void DoAction()
        {
           // Debug.Log(string.Format("Enemy {0}, attacks hex with coordinates {1}.\t Score: {2}", (AIAgent.AIAgentInstanceAgent.CurrentlyControledUnit).ToString(), (chosenTargetHex.OwningTile).ToString(), ScoredValue));
            BattlefieldManager.ManagerInstance.DestinationTile = ChosenTargetHex;
            ActionManager.Instance.StartUnitActionOnHex(ChosenTargetHex);
        }

        public void SimulateAction()
        {
            ActionOwner.CurrentHexTile.OwningTile.Occupied = false;
            ActionOwner.CurrentHexTile.ObjectOnHex = null;

            if (ActionOwner.AttackType==AttackType.Melee || (ActionOwner.AttackType == AttackType.Ranged && ((RangedAttack)(ActionOwner.AttackComponent)).IsEngagedInMelee))
            {
                ActionOwner.CurrentHexTile = GetInRangeNeighbour(ChosenTargetHex);
                ActionOwner.CurrentHexTile.OwningTile.Occupied = true;
                ActionOwner.transform.position = ActionOwner.CurrentHexTile.UnitAnchorWorldPositionVector;
            }

            UnitBehaviour targetEnemy = (UnitBehaviour) ChosenTargetHex.ObjectOnHex;
            ActionOwner.AttackComponent.ResolveDamage(targetEnemy);

        }

        public decimal SimulateScoreForHealth(decimal playerHealth, decimal enemyHealth)
        {
            decimal scoreDecimal = playerHealth - enemyHealth;
            UnitBehaviour enemyUnitBehaviour = (UnitBehaviour)ChosenTargetHex.ObjectOnHex;

            if (ActionOwner.AttackType == AttackType.Melee ||
                (ActionOwner.AttackType == AttackType.Ranged &&
                 ((RangedAttack) (ActionOwner.AttackComponent)).IsEngagedInMelee))
                scoreDecimal -= enemyUnitBehaviour.Damage * 0.5m;

            scoreDecimal += ActionOwner.Damage;

            return scoreDecimal;
        }

        //used for simulating position change of the attacker
        protected HexBehaviour GetInRangeNeighbour(HexBehaviour chosexHex)
        {
            List<HexTile> reachHexTiles = chosexHex.OwningTile.ReachableNeighbours.ToList();

            return reachHexTiles[0].GetHexBehaviour();
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

        public int Simulate(SimulatedUnit SimActionOwner,SimulatedUnit SimTarget)
        {
            if (SimActionOwner.UnitBehaviour.AttackType == AttackType.Melee || SimActionOwner.SimulatedHexBehaviour.OwningTile.AllNeighbours.Contains(SimTarget.SimulatedHexBehaviour.OwningTile))
            {
                return SimActionOwner.UnitBehaviour.Damage - SimTarget.UnitBehaviour.Damage / 2;
            }
            else
            {
                return SimActionOwner.UnitBehaviour.Damage;
            }
            
        }

        public void Print()
        {
            string s = "Attack:"+ActionOwner.ToString() + "attack unit on hex " + ChosenTargetHex.OwningTile.ToString()+". SimulatedValue:"+this.SimulatedValue;
           Debug.Log(s);
        }
        #endregion methods

    }
}
