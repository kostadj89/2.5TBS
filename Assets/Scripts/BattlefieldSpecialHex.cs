using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattlefieldSpecialHex : MonoBehaviour, IIsOnHexGrid
{
    private HexBehaviour currHexTile;

    public HexBehaviour CurrentHexTile
    {
        get { return currHexTile;}
        set { currHexTile = value; }
    }

    private BattlefieldSpecialHexType specHexType;

    private SpriteRenderer spriteRenderer;

    public Sprite[] CoverSprites;
    public Sprite[] HazardSprites;
    public Sprite[] HighGroundSprites;

    public int damage = 0;
    public float cover = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       //CoverSprites = new List<Sprite>();
       //HazardSprites = new List<Sprite>();
       //HighGroundSprites = new List<Sprite>();
    }

    public Type GetObjectType()
    {
        return typeof(BattlefieldSpecialHex);
    }

    public void InitializeCoverHex()
    {
        int coverSpriteIndex = UnityEngine.Random.Range(0, CoverSprites.Length);
        cover = Random.Range(0.35f, 0.751f);
        specHexType = BattlefieldSpecialHexType.Cover;
        CurrentHexTile.OwningTile.Cover = true;
        CurrentHexTile.OwningTile.Passable = false;

        spriteRenderer.sprite = CoverSprites[coverSpriteIndex];
    }

    public void InitializeHazardHex()
    {
        int hazardSpriteIndex = UnityEngine.Random.Range(0, HazardSprites.Length);
        damage = Random.Range(1, 3);
        specHexType = BattlefieldSpecialHexType.Hazard;
        CurrentHexTile.OwningTile.Cover = false;
        CurrentHexTile.OwningTile.Hazadours = true;

        spriteRenderer.sprite = HazardSprites[hazardSpriteIndex];
    }

    public void InitializeHighGroundHex()
    {
        int highGroundSpriteIndex = UnityEngine.Random.Range(0, HighGroundSprites.Length);
        specHexType = BattlefieldSpecialHexType.HighGround;
        CurrentHexTile.OwningTile.HighGround = true;

        spriteRenderer.sprite = HighGroundSprites[highGroundSpriteIndex];
    }

    internal void InstatiateSpecialHex(HexBehaviour hexBehaviour)
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        this.CurrentHexTile = hexBehaviour;

        int rand = Random.Range(0, 3);

        switch (rand)
        {
            case 0:
            default:
                this.InitializeCoverHex();
                break;
            case 1:
                this.InitializeHazardHex();
                break;
            case 2:
                this.InitializeHighGroundHex();
                break;
        }

        hexBehaviour.ObjectOnHex = this;
    }
}

public enum BattlefieldSpecialHexType
{
    Cover,
    Hazard,
    HighGround
}

