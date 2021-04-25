using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattlefieldManager : MonoBehaviour
{
    #region Consts

    private const float DISTANCE_BETWEEN_HEXES = 1.58f;

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
    public HexBehaviour StartingTile = null;
    public HexBehaviour DestinationTile = null;

    //List of units to be placed on the board, not instantiated
    public GameObject[] UnitsToPlace;
    //List Of BattlefieldObjects
    public GameObject BattlefieldObjectToPlace;
    public List<GameObject> InstantiatedUnits;

    public List<GameObject> InstantiatedBattlefieldObjects;
    //unit whose turn it is
    public UnitBehaviour CurrentlySelectedPlayingUnit;
    //unit who is a target of an action
    public UnitBehaviour TargetedUnit;

    //Path
    //game object which represents lines of path, dots
    public GameObject PathLine;

    //list which hold the path lines, dots, and represents our path
    public List<GameObject> Path;

    //Board
    private Dictionary<Point, HexBehaviour> Board;


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
        SetupBattlefieldObjects();
        SetupUnits();
        SetHazadoursTiles();
       
        //setup first playing unit
        CurrentlySelectedPlayingUnit = InstantiatedUnits[0].GetComponent<UnitBehaviour>();
        StartCurrentlyPlayingUnitTurn();
    }

    private void SetHazadoursTiles()
    {
        int intRandom = Random.Range(25, 30);
        int rX, rY;
        Point p;
        HexBehaviour hb;
        
        int i = 0;
        while( i < intRandom)
        {
            rX = Random.Range(1, 8);
            rY = Random.Range(1, 8);

            p = new Point(rX, rY);

            if (Board.ContainsKey(p))
            {
                hb = Board[p];

                if (hb.OwningTile.Passable && !hb.OwningTile.Occupied)
                {
                    hb.OwningTile.Hazadours = true;
                    hb.ChangeHexVisual(Color.yellow, hb.HazardousLookingHex);
                    i++;
                }
            }
        }
    }

    private void SetupBattlefieldObjects()
    {
        int intRandom = Random.Range(5, 10);
        int rX, rY;
        Point p;

        InstantiatedBattlefieldObjects = new List<GameObject>();

        GameObject currentBattlefieldObject;
        BattlefieldObject battlefieldObject;

        for (int i = 0; i < intRandom; i++)
        {
           rX = Random.Range(0, 5);
           rY = Random.Range(0, 5);
 
           p = new Point(rX, rY);

           currentBattlefieldObject =
               Instantiate(BattlefieldObjectToPlace, Board[p].transform.position, Quaternion.identity);

           battlefieldObject = currentBattlefieldObject.GetComponent<BattlefieldObject>();
           battlefieldObject.CurrentHexTile = Board[p];
           Board[p].OwningTile.Passable = false;
           InstantiatedBattlefieldObjects.Add(currentBattlefieldObject);
        }
    }

    //Called once on Start of the battle, it takes all the units instantiates them, sorts them by the initiative and places them on the battlefield
    //Also setups the first unit, with the highest initiative, and begins it's turn
    private void SetupUnits()
    {
        //instantiating and placing units
        int k = 0;
        InstantiatedUnits = new List<GameObject>();

        for (int i = 0; i < UnitsToPlace.Length; i++)
        {
            GameObject unitGameObject = (GameObject)Instantiate(UnitsToPlace[i]);
            //Point placementPoint = new Point(-k, i);
            Point placementPoint = i==0?new Point(2,5) : new Point(-k, i);

            if (i % 2 == 1) k++;

            UnitBehaviour ub = unitGameObject.GetComponent<UnitBehaviour>();
            SetupPlayerId(ub,i);
            PlaceUnitOnCoordinates(ub, placementPoint);

            //add to instantiated list
            InstantiatedUnits.Add(unitGameObject);

        }

        //ordering by initiative
        InstantiatedUnits.OrderByDescending(x => x.GetComponent<UnitBehaviour>());
    }

    private void SetupPlayerId(UnitBehaviour ub, int i)
    {
        ub.PlayerId = i % 2;
    }

    public void SelectNextPlayingUnit()
    {
        int currentIndex = InstantiatedUnits.IndexOf(CurrentlySelectedPlayingUnit.gameObject);
        int nextIndex;

        if (currentIndex == InstantiatedUnits.Count-1)
        {
            nextIndex = 0;
        }
        else
        {
            nextIndex = currentIndex + 1;
        }

        CurrentlySelectedPlayingUnit = InstantiatedUnits[nextIndex].GetComponent<UnitBehaviour>();
    }

    private void StartCurrentlyPlayingUnitTurn()
    {
        //UnitBehaviour ub = CurrentlySelectedPlayingUnit.GetComponent<UnitBehaviour>();
        StartingTile = CurrentlySelectedPlayingUnit.CurrentHexTile;
        StartingTile.ChangeVisualToSelected();

        //shows currently playing unit's ui
        CurrentlySelectedPlayingUnit.ShowUnitUI();

        UnitMovement.instance.SetupCurrentlyOwningUnit(CurrentlySelectedPlayingUnit);
    }
    

    public void EndCurrentPlayingUnitTurn()
    {
        //don't know if this is necessary
        StartingTile.ChangeHexVisualToDeselected();
        DestinationTile.ChangeHexVisualToDeselected();

        //movement stuff...
        UnitBehaviour ub = CurrentlySelectedPlayingUnit.GetComponent<UnitBehaviour>();
        //...setting previously occupied tile back to being passable...
        ub.CurrentHexTile.OwningTile.Occupied = false;
        ub.CurrentHexTile.ObjectOnHex = null;
        //...setting previous destination tile to be new current for moving unit and marking it as notpassable... 
        ub.CurrentHexTile = DestinationTile;

        ub.CurrentHexTile.OwningTile.Occupied = true;
        ub.CurrentHexTile.ObjectOnHex = ub;
       //...reseting starting and destination tiles in order to destroy path...
       StartingTile = null;
        DestinationTile = null;

        //...this destroys path when the unit reaches destination...
        GenerateAndShowPath();
        
        ub.HideUnitUI();

        //detarget targeted unit
        if (TargetedUnit != null)
        {
            TargetedUnit.HideUnitUI();
            TargetedUnit = null;
        }

        //...selects next playing unit...
        SelectNextPlayingUnit();
        StartCurrentlyPlayingUnitTurn();
    }

    public HexBehaviour GeTileBehaviourFromPoint(Point tileLocation)
    {
        return Board[tileLocation];
    }

    public void GetAllEnemiesInRange()
    {
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
        if (!Board.ContainsValue(this.StartingTile))
        {
            return;
        }

        Point currentUnitPoint = Board.FirstOrDefault(x => x.Value == this.StartingTile).Key;
        List<HexBehaviour> reachableTiles = Board.Values.Where(b =>
            Vector3.Distance(this.StartingTile.UnitAnchorWorldPositionVector, b.UnitAnchorWorldPositionVector) <=
            movementRange * DISTANCE_BETWEEN_HEXES).ToList();

        foreach (HexBehaviour reachableTile in reachableTiles)
        {
            if (reachableTile.OwningTile.Passable)
            {
                reachableTile.OwningTile.IsInRange = true;
                reachableTile.ChangeHexVisual(Color.gray);
            }

            //Debug.Log("Distance from (" + currentUnitPoint.X.ToString() + ", " + currentUnitPoint.Y.ToString() + ") to the (" + i + ", " + j + ") is: " + Vector3.Distance(StartingTile.UnitAnchorWorldPositionVector, reachableTile.UnitAnchorWorldPositionVector));
        }
    }

    internal void SelectTilesInRange(int movementRange)
    {
        if (!Board.ContainsValue(this.StartingTile))
        {
            return;
        }

        Point currentUnitPoint = Board.FirstOrDefault(x => x.Value == this.StartingTile).Key;

        Debug.Log("Unit's starting coordiantes: ("+ currentUnitPoint.X.ToString()+", "+ currentUnitPoint.Y.ToString()+")");

        
        //int LowerXLimit = -1 * currentUnitPoint.Y / 2;
        //int UpperXLimit = 
        //int lowerX = 
        //for (int i = currentUnitPoint.X - movementRange; i < currentUnitPoint.X + movementRange+1; i++)
        //{

        //}

        HexBehaviour selectedTileBehaviour = null;

        //i is row index, j is column index 
        for (int i = currentUnitPoint.X-movementRange; i < currentUnitPoint.X+movementRange+1; i++)
        {
            for (int j = currentUnitPoint.Y - movementRange; j < currentUnitPoint.Y + movementRange + 1; j++)
            {
                // range of x changes depending on Y:
                // for Y = 0,1 => 0<=X<=9 range
                // for Y = 8,9 => -4<=X<=5 range
                // upper left tile coordiantes are always 0,0
                if (j >= 0 && j < gridWidthInHexes && i >=(-1* j/2) && i < (gridHeightInHexes - j / 2))
                {
                    //my random formula for getting hex shaped selection
                    if (currentUnitPoint.X + currentUnitPoint.Y - movementRange <= i + j && i + j <= currentUnitPoint.X + currentUnitPoint.Y + movementRange)
                    {
                        selectedTileBehaviour = Board[new Point(i, j)];
                        
                        if (selectedTileBehaviour.OwningTile.Passable)
                        {
                            selectedTileBehaviour.OwningTile.IsInRange = true;
                            selectedTileBehaviour.ChangeHexVisual(Color.gray);

                            Debug.Log("Distance from (" + currentUnitPoint.X.ToString() + ", " + currentUnitPoint.Y.ToString() + ") to the (" +i+", "+ j +") is: "+ Vector3.Distance(StartingTile.UnitAnchorWorldPositionVector, selectedTileBehaviour.UnitAnchorWorldPositionVector));
                        }
                    }
                }
            }
        }
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentlySelectedPlayingUnit.GetComponent<UnitBehaviour>().TakeDamage(1);
        }
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
            if (tile!=StartingTile.OwningTile && tile != DestinationTile.OwningTile)
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
        if (StartingTile==null || (DestinationTile == null))
        {
            DrawPath(new List<HexTile>());
            return;
        }

        var path = Pathfinder.FindPath(StartingTile.OwningTile, DestinationTile.OwningTile);
        DrawPath(path);
    }

    //handles attack
    public void StartAttack()
    {
        //get currently playing unit's behaviour
        UnitBehaviour currentPlayingUnit = CurrentlySelectedPlayingUnit.GetComponent<UnitBehaviour>();

        //we show targeted unit's ui, and damage it
        TargetedUnit.TakeDamage(currentPlayingUnit.Damage);

        //damage attacking unit with relation strike damage
        currentPlayingUnit.TakeDamage((int) (TargetedUnit.Damage * 0.5));

        BattlefieldManager.ManagerInstance.EndCurrentPlayingUnitTurn();
    }
    #endregion Public Methods


}
