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
    Exponential
}

public enum ConsiderationInputType
{
    TargetHealth,
    TargetDistance,
    SelfHealth,
    CoverDistance
}

public class Consideration : ScriptableObject
{
    public float ConsiderationInputValue;

    public ConsiderationInputType ConsiderationInputType;

    public GraphType GraphType;
    public float K, M;

    public Consideration(ConsiderationInputType C,float X, float K, float M, GraphType GraphType )
    {
        ConsiderationInputType = C;
        ConsiderationInputValue = X;
        this.K = K;
        this.M = M;
        this.GraphType = GraphType;
    }

    public float Score()
    {
        float Y=0;

        float clampedConsiderationValue = ClampConsiderationInputValue();
        switch (GraphType)
        {
            case GraphType.Linear:
                break;
            case GraphType.Quadratic:
                break;
            case GraphType.Logistic:
                break;
            case GraphType.Exponential:
                break;
        }

        return Y;
    }

    private float ClampConsiderationInputValue()
    {
        throw new NotImplementedException();
    }
}
