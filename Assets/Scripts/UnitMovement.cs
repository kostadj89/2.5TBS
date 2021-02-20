using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    #region private fields

    //private CharacterController owningUnitCharacterController;

    //tile and position of the tile we are moving to 
    private Vector3 currentTargetTilePosition;
    private Tile currentTargetTile;
    private List<Tile> path;

    //i want to have only one object of script of this type, and pass it around units, so these two fields are gonna be transform of creature whose turn is now
    private Transform owningUnitTransform;
    private Animator owningUnitAnimator;

    //speed in meters per second
    private float speed = 0.0025f;
    //probably will ignore this
    private float rotationSpeed = 0.004f;
    #endregion private fields

    #region public fields

    public GameObject owningUnit;

    #endregion public fields

    #region static fields

    //distance between character and tile position when we assume we reached it and start looking for the next. Explained in detail later on
    public static float MinNextTileDist = 0.07f;

    //singleton, why:/
    public static UnitMovement instance = null;

    #endregion static fields

    #region props

    public bool IsMoving { get; private set; }
    public bool IsMovingToAttack { get; private set; }

    #endregion props

    void Awake()
    {
        //singleton pattern here is used just for the sake of simplicity.should be used in cases when this script is attached to more than one character
        instance = this;
    }

    private void SetMovingState(bool v)
    {
        IsMoving = v;

        if (!v)
        {
            owningUnitAnimator.Play("idle");
        }
        else
        {
            owningUnitAnimator.Play("walking");
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsMoving)
        {
            return;
        }

        //if the distance between the character and the center of the next tile is short enough
        if ((currentTargetTilePosition - owningUnitTransform.position).sqrMagnitude < MinNextTileDist * MinNextTileDist)
        {
            //if we reach the destination tile
            if (path.IndexOf(currentTargetTile)==0)
            {
                SetMovingState(false);

                if (IsMovingToAttack)
                {
                    IsMovingToAttack = false;
                    BattlefieldManager.ManagerInstance.StartAttack();
                }
                else
                {
                    BattlefieldManager.ManagerInstance.EndCurrentPlayingUnitTurn();
                }
                
                return;
            }

            //else current target tile becomes the next tile from the list
            currentTargetTile = path[path.IndexOf(currentTargetTile) - 1];
            currentTargetTilePosition = CalcTilePosition(currentTargetTile);
        }

        //MoveTowardsPosition(currentTargetTilePosition);
        owningUnitTransform.position = Vector3.MoveTowards(owningUnitTransform.position, currentTargetTilePosition, Time.deltaTime * speed);
    }

    public Vector3 CalcTilePosition(Tile tile)
    {
        Vector2 tileGridPosition = new Vector2(tile.X, tile.Y);
        Vector3 tilePos = BattlefieldManager.ManagerInstance.GetUnitAnchorFromATileOnBoard(tileGridPosition);

        return tilePos;
    }

    //method argument is a list of tiles we got from the path finding algorithm
    public void StartMoving(List<Tile> path, bool isMovingToAttack = false)
    {
        if (path.Count == 0)
        {
            SetMovingState(false);
            IsMovingToAttack = false;
            return;
        }

        //the first tile we need to reach is actually in the end of the list just before the one the unit is currently on, the unit is on path[path.Count - 1];
        currentTargetTile = path[path.Count - 2];
        currentTargetTilePosition = CalcTilePosition(currentTargetTile);
        SetMovingState(true);
        IsMovingToAttack = isMovingToAttack;
        this.path = path;
    }

    public void SetupCurrentlyOwningUnit(GameObject currOwningUnit, UnitBehaviour ub)
    {
        owningUnit = currOwningUnit;

        //caching the transform for better performance
        owningUnitTransform = owningUnit.transform;

        //animation setup
        owningUnitAnimator = owningUnit.GetComponent<Animator>();
        
        SetMovingState(false);

        speed = ub.speed;
        rotationSpeed = ub.rotationSpeed;

        //ResetTilesInRange resets tiles which were in movement range of previous unit
        BattlefieldManager.ManagerInstance.ResetTilesInRange();

        BattlefieldManager.ManagerInstance.StartingTile = ub.currentTile;
        BattlefieldManager.ManagerInstance.SelectTilesInRangeSimple(ub.movementRange);
    }
}
