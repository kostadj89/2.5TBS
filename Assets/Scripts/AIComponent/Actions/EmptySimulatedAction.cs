using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.AIComponent.Actions
{
    class EmptySimulatedAction : IAction
    {
        public HexBehaviour ChosenTargetHex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<IConsideration> Considerations => throw new NotImplementedException();

        public ActionType ActionType => throw new NotImplementedException();

        public float ScoredValue => throw new NotImplementedException();

        public EmptySimulatedAction()
        {
            
        }

        public EmptySimulatedAction(int val)
        {
            SimulatedValue = val;
        }

        public int SimulatedValue { get; set; }
        public UnitBehaviour ActionOwner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void DoAction()
        {
            throw new NotImplementedException();
        }

        public float GetScore()
        {
            throw new NotImplementedException();
        }

        public int Simulate(SimulatedUnit SimActionOwner, SimulatedUnit target)
        {
            throw new NotImplementedException();
        }

        public int SimulatedConsidValue { get; set; }

        public void Print()
        {
            string s = "Empty action";
            Debug.Log(s);
        }
    }
}
