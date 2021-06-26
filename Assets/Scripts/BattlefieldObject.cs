using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldObject : MonoBehaviour, IIsOnHexGrid
{
    private HexBehaviour currHexTile;

    public HexBehaviour CurrentHexTile
    {
        get { return currHexTile;}
        set { currHexTile = value; }
    }

    private BattlefieldObjectType battlefieldObject;

    private SpriteRenderer spriteRenderer;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Type GetObjectType()
    {
        return typeof(BattlefieldObject);
    }
}

public enum BattlefieldObjectType
{
    Blocade,
    Damaging
}

