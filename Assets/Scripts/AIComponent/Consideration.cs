using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum GraphType
{
    Linear,
    Quadratic,
    Logistic,
    Exponential,
    Custom
}

public enum ConsiderationInputType
{
    TargetHealth,
    SelfHealth,
    NearestEnemyDistance,
    NearestAllyDistance,
    EnemyTargetDistance,
    TargetEnemyRetaliationStrike,
    CoverValue
}

public interface IConsideration
{
    UnitBehaviour OwnerOFConsideration { get; set; }
    HexBehaviour targetHexContex { get; set; }
    float ConsiderationInputValue { get;}

    ConsiderationInputType ConsiderationInputType { get; set; }

    GraphType GraphType { get; set; }

    float K { get; set; }
    float M { get; set; }
    float C { get; set; }
    float B { get; set; }

    float Score();

}
