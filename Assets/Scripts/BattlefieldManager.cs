﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

public class BattlefieldManager : MonoBehaviour
{
    #region Consts

    public const float DISTANCE_BETWEEN_HEXES = 1.58f;

    #endregion Consts

    #region Props

    //prefab of the hex
    public GameObject Hex;

    //dimensions of hex grid, how many hexes fits in one row and one column
    public int gridWidthInHexes = 10;
    public int gridHeightInHexes = 10;

    //list of available hex sprites for a single hex, in the futer will be dependant of overworld map
    public Sprite[] sprites;

    //dimensions of the single hex
    private float hexWidth;
    private float hexHeight;

    //initial position of hex, all of the hexes will have their position set based on this, starting hex position
    private Vector3 initialPos;

    //singleton
    public static BattlefieldManager ManagerInstance = null;

    //Tile and Tile behaviour
    public HexTile SelectedTile = null;
    public HexBehaviour StartingHexBehaviorTile = null;
    public HexBehaviour DestinationTile = null;

    //List of units to be placed on the board, not instantiated
    public GameObject[] UnitsToPlace;
    //List Of BattlefieldObjects
    public GameObject BattlefieldObjectToPlace;
    public List<GameObject> InstantiatedUnits;

    public List<GameObject> InstantiatedBattlefieldObjects;
    //unit whose turn it is


    //Path
    //game object which represents lines of path, dots
    public GameObject PathLine;

    //list which hold the path lines, dots, and represents our path
    public List<GameObject> Path;

    //Board
    private Dictionary<Point, HexBehaviour> Board;

    //Combat has begun
    public bool CombatStarted = false;


    #endregion Props

    #region Overrides

    void Awake()
    {
        ManagerInstance = this;
       
    }

    // Start is called before the first frame update
    void Start()
    {
        //initializing hexes
        SetHexSize();
        CalculateInitialPosition();
        CreateGrid();
        SetupSpecialHexes();
        SetupUnits();
        CreateAIAgent();
       // SetHazadoursTiles();
       
        //setup first playing unit
        CombatStarted = true;
        ActionManager.Instance.StartCurrentlyPlayingUnitTurn(InstantiatedUnits[0].GetComponent<UnitBehaviour>());
    }

    private void CreateAIAgent()
    {
        AIAgent aiAgent = new AIAgent();
    }

    //private void SetHazadoursTiles()
    //{
    //    int intRandom = Random.Range(25, 30);
    //    int rX, rY;
    //    Point p;
    //    HexBehaviour hb;

    //    int i = 0;
    //    while( i < intRandom)
    //    {
    //        rX = Random.Range(1, 8);
    //        rY = Random.Range(1, 8);

    //        p = new Point(rX, rY);

    //        if (Board.ContainsKey(p))
    //        {
    //            hb = Board[p];

    //            if (hb.OwningTile.Passable && !hb.OwningTile.Occupied)
    //            {
    //                hb.OwningTile.Hazadours = true;
    //                hb.ChangeHexVisualToHazardous();
    //                i++;
    //            }
    //        }
    //    }
    //}

    private void SetupSpecialHexes()
    {
        int intRandom = Random.Range(10, 20);
        int rX, rY;
        Point p;

        InstantiatedBattlefieldObjects = new List<GameObject>();

        GameObject currentBattlefieldObject;
        BattlefieldSpecialHex battlefieldSpecialHex;

        for (int i = 0; i < intRandom;)
        {
           rX = Random.Range(0, 10);
           rY = Random.Range(0 - rX/2, 10 - rX / 2);
 
           p = new Point(rX, rY);

           if (Board.ContainsKey(p) && Board[p].ObjectOnHex == null)
           {
               currentBattlefieldObject =
                   Instantiate(BattlefieldObjectToPlace, Board[p].transform.position, Quaternion.identity);

               battlefieldSpecialHex = currentBattlefieldObject.GetComponent<BattlefieldSpecialHex>();
            
               //instantiate special hex
               battlefieldSpecialHex.InstatiateSpecialHex(Board[p]);

               //add to the list of Objects
               InstantiatedBattlefieldObjects.Add(currentBattlefieldObject);

               i++;
           }
        }
    }

    //Called once on Start of the battle, it takes all the units instantiates them, sorts them by the Initiative and places them on the battlefield
    //Also setups the first unit, with the highest Initiative, and begins it's turn
    private void SetupUnits()
    {
        //instantiating and placing units
        int k = 0,j=0;
        InstantiatedUnits = new List<GameObject>();

        for (int i = 0; i < UnitsToPlace.Length;)
        {
            //Point placementPoint = new Point(-k, i);
            Point placementPoint = j % 2 == 0? new Point(9-k,j) : new Point(-k, j);

            if (Board.ContainsKey(placementPoint) && Board[placementPoint].ObjectOnHex == null)
            {
                GameObject unitGameObject = (GameObject)Instantiate(UnitsToPlace[i]);

                if (i % 2 == 1) k++;

                UnitBehaviour ub = unitGameObject.GetComponent<UnitBehaviour>();
                SetupPlayerId(ub, i);

                ub.InitializeMovementComponent();
                ub.InitializeAttackComponent();
                PlaceUnitOnCoordinates(ub, placementPoint);
                //Show UI
                ub.ShowUnitUI();

                //add to instantiated list
                InstantiatedUnits.Add(unitGameObject);

                i++;
            }

            if (j>9)
            {
                j = 0;
            }

            j++;
        }

        //ordering by Initiative
        InstantiatedUnits.OrderByDescending(x => x.GetComponent<UnitBehaviour>());
    }

    private void SetupPlayerId(UnitBehaviour ub, int i)
    {
        ub.PlayerId = i % 2;
    }

    public HexBehaviour GeTileBehaviourFromPoint(Point tileLocation)
    {
        return Board[tileLocation];
    }

    public List<HexBehaviour> GetAllEnemiesInRange()
    {
        UnitBehaviour currPlayingUnit = ActionManager.Instance.CurrentlySelectedPlayingUnit;
        Vector3 currPlayingunitPosition = currPlayingUnit.CurrentHexTile.UnitAnchorWorldPositionVector;

        int attackRange = currPlayingUnit.AttackComponent.AttackRange;

        List<HexBehaviour> enemyHexes = currPlayingUnit.AttackComponent.GetAttackableTiles().Where(x => x.OwningTile.Occupied).ToList();
        List<HexBehaviour> returnHexBehaviours = new List<HexBehaviour>(enemyHexes);
       UnitBehaviour enemyUnitBehaviour;
        foreach (HexBehaviour hexBehaviour in enemyHexes)
        {
            enemyUnitBehaviour = (UnitBehaviour)hexBehaviour.ObjectOnHex;
            //probs unecessarry
            if(enemyUnitBehaviour!=null && (!enemyUnitBehaviour.isAlive || enemyUnitBehaviour.PlayerId == ActionManager.Instance.CurrentlySelectedPlayingUnit.PlayerId))
            {
                returnHexBehaviours.Remove(hexBehaviour);
            }
        }

        return returnHexBehaviours;
        //temporary this will only work for melee units whose attack range is 1
        //List<GameObject> enemiesInRange = InstantiatedUnits.Where(x=>x.GetComponent<UnitBehaviour>().CurrentHexTile.OwningTile.IsInRange )
    }

    //resets all tiles
    public void ResetTilesInRange()
    {
        List<HexBehaviour> previousTilesInRange = Board.Values.Where(x => x.OwningTile.IsInRange == true).ToList();
        foreach (HexBehaviour tile in previousTilesInRange)
        {
            tile.OwningTile.IsInRange = false;
            tile.ChangeHexVisualToDeselected();
        }
    }

    //select all tiles in range and marks them as in range
    internal void SelectTilesInRangeSimple(int movementRange)
    {
        if (!Board.ContainsValue(this.StartingHexBehaviorTile))
        {
            return;
        }

        //Point currentUnitPoint = Board.FirstOrDefault(x => x.Value == this.StartingHexBehaviorTile).Key;
        //List<HexBehaviour> reachableTiles = Board.Values.Where(b =>
        //    Vector3.Distance(this.StartingHexBehaviorTile.UnitAnchorWorldPositionVector, b.UnitAnchorWorldPositionVector) <=
        //    movementRange * DISTANCE_BETWEEN_HEXES).ToList();
        List<HexBehaviour> reachableTiles = GetTilesInRange(this.StartingHexBehaviorTile.UnitAnchorWorldPositionVector,movementRange);

        foreach (HexBehaviour reachableTile in reachableTiles)
        {
            if (reachableTile.OwningTile.Passable)
            {
                reachableTile.OwningTile.IsInRange = true;
               
                reachableTile.ChangeVisualToReachable();
            }

            //Debug.Log("Distance from (" + currentUnitPoint.X.ToString() + ", " + currentUnitPoint.Y.ToString() + ") to the (" + i + ", " + j + ") is: " + Vector3.Distance(StartingHexBehaviorTile.UnitAnchorWorldPositionVector, reachableTile.UnitAnchorWorldPositionVector));
        }
    }

    internal List<HexBehaviour> GetTilesInRange(Vector3 startingTileVector3,int range)
    {
        return Board.Values.Where(b =>
            Vector3.Distance(startingTileVector3, b.UnitAnchorWorldPositionVector) <=
            range * DISTANCE_BETWEEN_HEXES).ToList();
    }

    public void SetupStartingTile(HexBehaviour currentHexTile)
    {
        StartingHexBehaviorTile = currentHexTile;
        StartingHexBehaviorTile.ChangeVisualToSelected();
    }

    private void PlaceUnitOnCoordinates(UnitBehaviour ub, Point placementPoint)
    {
        ub.transform.position = Board[placementPoint].UnitAnchorWorldPositionVector;

        
        ub.CurrentHexTile = Board[placementPoint];
        ub.CurrentHexTile.OwningTile.Occupied = true;
        ub.CurrentHexTile.ObjectOnHex = ub;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    CurrentlySelectedPlayingUnit.GetComponent<UnitBehaviour>().TakeDamage(1);
        //}
    }

    #endregion Overrides


    #region Private Methods

    private void CalculateInitialPosition()
    {
        initialPos = new Vector3(-hexWidth * gridWidthInHexes / 2f + hexWidth / 2, gridHeightInHexes / 2f * hexHeight - hexHeight / 2, 0);
    }

    private void CreateGrid()
    {
        //Vector2 gridSize = 
        GameObject HexGridGO = new GameObject("HexGrid");

        Board = new Dictionary<Point, HexBehaviour>();

        for (int y = 0; y < gridHeightInHexes; y++)
        {
            for (int x = 0; x < gridWidthInHexes; x++)
            {
                GameObject hex = (GameObject)Instantiate(Hex);
                Vector2 gridPos = new Vector2(x, y);
                hex.transform.position = CalculateWorldCoordinates(gridPos);
                hex.transform.parent = HexGridGO.transform;
                
                //temp assigning Tile behaviour
                HexBehaviour tb = (HexBehaviour)hex.GetComponent("HexBehaviour");
                tb.OwningTile = new HexTile((int)x - (int)(y / 2), (int)y);
                tb.coordinates = tb.OwningTile.ToString();
                //tb.UnitAnchorWorldPositionVector = hex.transform.position;

                //each has gets a random sprite
                SetHexSprite(hex,tb);

                //each hex get rotated for random r*60 degrees
                hex.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 6) * 60);
                hex.transform.rotation = Quaternion.Euler(60, 0, 0);
                //once we rotate the hexes we set that position to tile behaviour, and set unit anchor, place where unit will stand on tile
                tb.UnitAnchorWorldPositionVector = hex.transform.position - new Vector3(0f, 0.36f, 0f);

                //adding Tile to dictionary
                Board.Add(tb.OwningTile.Location, tb);

                /*//variable to indicate if all rows have the same number of hexes in them
        //this is checked by comparing width of the first hex row plus half of the hexWidth with groundWidth
        bool equalLineLengths = (gridSize.x + 0.5) * hexWidth <= groundWidth;
        */
            }
        }
        
        //Neighboring tile coordinates of all the tiles are calculated
        foreach (HexBehaviour behaviour in Board.Values)
            behaviour.OwningTile.FindNeighbours(Board, new Vector2(gridWidthInHexes, gridHeightInHexes));
    }

    //probbably redundant
    internal Vector3 GetUnitAnchorFromATileOnBoard(Vector2 tileGridPosition)
    {
        Point targetPoint = new Point((int)tileGridPosition.x, (int)tileGridPosition.y);
        return Board[targetPoint].UnitAnchorWorldPositionVector;
    }

    private void SetHexSprite(GameObject hex, HexBehaviour tb)
    {
        SpriteRenderer hexSpriteRenderer = hex.GetComponent<SpriteRenderer>();
        int randomSpriteIndex = 0 + Random.Range(1, sprites.Length) * Random.Range(0, 2);

        hexSpriteRenderer.sprite = sprites[randomSpriteIndex];

        tb.NormalLookingHex = hexSpriteRenderer.sprite;
    }

    public Vector3 CalculateWorldCoordinates(Vector2 gridPos)
    {
        float offset = 0;

        if (gridPos.y % 2 != 0)
        {
            offset = hexWidth / 2f;
        }

        float newX = initialPos.x + offset + gridPos.x * hexWidth;
        float newY = initialPos.y - gridPos.y * hexHeight * 0.75f;
        Vector3 helperVector3 = new Vector3(newX, newY, 0)+new Vector3(0, -4f,0);
        //rotating hexes around x axis for 60, and lowering grid height for 2.5f
        return Quaternion.Euler(60, 0, 0)* helperVector3 ;
    }

    private void SetHexSize()
    {
        hexWidth = Hex.GetComponent<SpriteRenderer>().bounds.size.x;
        hexHeight = Hex.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void DrawPath(IEnumerable<HexTile> pathTiles)
    {
        if (this.Path == null)
        {
            this.Path = new List<GameObject>();
        }

        this.Path.ForEach(Destroy);
        this.Path.Clear();

        GameObject Lines = GameObject.Find("Lines");
        if (Lines == null)
        {
            Lines = new GameObject("Lines");
        }

        foreach (HexTile tile  in pathTiles)
        {
            //didn't want to make start and destination tiles visible, but i'm not sure this is the right way to do it
            if (tile!=StartingHexBehaviorTile.OwningTile && tile != DestinationTile.OwningTile)
            {
                GameObject pathLine = (GameObject)Instantiate(PathLine);
                //calcWorldCoord method uses squiggly axis coordinates so we add y / 2 to convert x coordinate from straight axis coordinate system
                Vector2 gridPos = new Vector2(tile.X + tile.Y / 2, tile.Y);
                pathLine.transform.position = CalculateWorldCoordinates(gridPos);
                this.Path.Add(pathLine);
            }

        }
    }

    #endregion  Private Methods

    #region Public Methods

    //creates and draws the path
    public void GenerateAndShowPath()
    {
        if (StartingHexBehaviorTile==null || (DestinationTile == null))
        {
            DrawPath(new List<HexTile>());
            return;
        }

        var path = Pathfinder.FindPath(StartingHexBehaviorTile.OwningTile, DestinationTile.OwningTile);
        DrawPath(path);
    }

    #endregion Public Methods


}
