using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.AIComponent;
using UnityEngine;

public enum ActionType
{
    Attack,
    Move
}

public interface IAction 
{
    void DoAction();
    void Print();
    float GetScore();
    //List<HexBehaviour> GetPossibleTargetHexes();
    UnitBehaviour ActionOwner { get; set; }
    HexBehaviour ChosenTargetHex { get; set; }
    List<IConsideration> Considerations { get; }
    ActionType ActionType { get;}
    float ScoredValue { get; }

    //for minimax simulates action to get score
    int Simulate(SimulatedUnit SimActionOwner, SimulatedUnit target);
    int SimulatedConsidValue { get; set; }
    int SimulatedValue { get; set; }
}
