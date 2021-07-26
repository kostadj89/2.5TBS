using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Attack,
    Move
}

public interface IAction 
{
    void DoAction();
    float GetScore();
    //List<HexBehaviour> GetPossibleTargetHexes();
    HexBehaviour ChosenTargetHex { get; set; }
    List<Consideration> Considerations { get; }
    ActionType ActionType { get;}
}
