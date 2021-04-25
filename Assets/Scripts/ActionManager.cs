using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    #region private fields

    //private CharacterController owningUnitCharacterController;

    //tile and position of the tile we are moving to 
    private Vector3 currentTargetTilePosition;
    private HexTile currentTargetTile;
    private List<HexTile> path;

    //i want to have only one object of script of this type, and pass it around units, so these two fields are gonna be transform of creature whose turn is now
    private Transform owningUnitTransform;
    private Animator owningUnitAnimator;

    //speed in meters per second
    private float speed = 0.0025f;
    //probably will ignore this
    private float rotationSpeed = 0.004f;
    #endregion private fields

    #region public fields

    public GameObject OwningUnit;
    public UnitBehaviour CurrentlySelectedPlayingUnit;

    //unit who is a target of an action
    public UnitBehaviour TargetedUnit;

    #endregion public fields

    #region static fields

    //distance between character and tile position when we assume we reached it and start looking for the next. Explained in detail later on
    public static float MinNextTileDist = 0.07f;

    //singleton, why:/
    public static ActionManager Instance = null;

    #endregion static fields

    #region props

    public bool IsMoving { get; private set; }
    public bool IsMovingToAttack { get; private set; }

    #endregion props

    void Awake()
    {
        //singleton pattern here is used just for the sake of simplicity.should be used in cases when this script is attached to more than one character
        Instance = this;
    }

    private void SetMovingState(bool v)
    {
        CurrentlySelectedPlayingUnit.CurrentState = UnitState.Moving;

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
        //if (CurrentlySelectedPlayingUnit.CurrentState!= UnitState.Moving)
        //{
        //    return;
        //}

        switch (CurrentlySelectedPlayingUnit.CurrentState)
        {
            case UnitState.Moving:
                
                //if the distance between the character and the center of the next tile is short enough
                if ((currentTargetTilePosition - owningUnitTransform.position).sqrMagnitude < MinNextTileDist * MinNextTileDist)
                {
                    //if we reach the destination tile
                    if (path.IndexOf(currentTargetTile) == 0)
                    {
                        if (IsMovingToAttack)
                        {
                            CurrentlySelectedPlayingUnit.CurrentState = UnitState.Attacking;
                            IsMovingToAttack = false;
                            StartAttack();
                        }
                        else
                        {
                            EndCurrentPlayingUnitTurn();
                        }

                        return;
                    }

                    //else current target tile becomes the next tile from the list
                    currentTargetTile = path[path.IndexOf(currentTargetTile) - 1];
                    currentTargetTilePosition = CalcTilePosition(currentTargetTile);
                }

                //MoveTowardsPosition(currentTargetTilePosition);
                owningUnitTransform.position = Vector3.MoveTowards(owningUnitTransform.position, currentTargetTilePosition, Time.deltaTime * speed);
                break;
            case UnitState.Attacking:
                break;
            case UnitState.CastingSpell:
                break;
            case UnitState.Flanking:
                break;
            case UnitState.Idle:
                break;
        }
       
    }

    public void StartUnitActionOnHex(HexBehaviour targetHexBehaviour)
    {
        switch (CurrentlySelectedPlayingUnit.CurrentState)
        {
            case UnitState.Attacking:
                break;
            case UnitState.CastingSpell:
                break;
            case UnitState.Flanking:
                break;
            case UnitState.Moving:
                break;
            case UnitState.Idle:
                PrepareToStartMoving(targetHexBehaviour);
                break;
        }

    }

    public Vector3 CalcTilePosition(HexTile tile)
    {
        Vector2 tileGridPosition = new Vector2(tile.X, tile.Y);
        Vector3 tilePos = BattlefieldManager.ManagerInstance.GetUnitAnchorFromATileOnBoard(tileGridPosition);

        return tilePos;
    }

    //have to solve moving to allied units
    public void PrepareToStartMoving(HexBehaviour targetHexBehaviour)
    {
        BattlefieldManager.ManagerInstance.GenerateAndShowPath();

        var path = Pathfinder.FindPath(BattlefieldManager.ManagerInstance.StartingTile.OwningTile, BattlefieldManager.ManagerInstance.DestinationTile.OwningTile);

        if (BattlefieldManager.ManagerInstance.DestinationTile.OwningTile.Occupied)
        {
            UnitBehaviour ub = (UnitBehaviour)BattlefieldManager.ManagerInstance.DestinationTile.ObjectOnHex;

            if (ub.PlayerId != CurrentlySelectedPlayingUnit.PlayerId)
            {
                BattlefieldManager.ManagerInstance.DestinationTile.ChangeHexVisual(Color.red, targetHexBehaviour.SelectedLookingHex);
               TargetedUnit = ub;

                path = path.PreviousSteps;

                BattlefieldManager.ManagerInstance.DestinationTile = path.LastStep.GetHexBehaviour();
                //we color the selected path to real white
                BattlefieldManager.ManagerInstance.DestinationTile.ChangeHexVisual(Color.white, targetHexBehaviour.SelectedLookingHex);
                StartMoving(path.ToList(), true);
            }
        }
        else
        {
            //we color the selected path to real white
            targetHexBehaviour.ChangeHexVisual(Color.white, targetHexBehaviour.SelectedLookingHex);
            StartMoving(path.ToList());
        }
    }

    //method argument is a list of tiles we got from the path finding algorithm
    public void StartMoving(List<HexTile> path, bool isMovingToAttack = false)
    {
        if (path.Count == 0)
        {
            //SetMovingState(false);
            IsMovingToAttack = false;
            return;
        }

        //the first tile we need to reach is actually path[path.Count - 2], near the end of the list just before the one the unit is currently on, the unit is on path[path.Count - 1];
        if (isMovingToAttack && path.Count == 1)
        {
            currentTargetTile = path[0];
        }
        else
        {
            currentTargetTile = path[path.Count - 2];
        }

        currentTargetTilePosition = CalcTilePosition(currentTargetTile);
        SetMovingState(true);
        IsMovingToAttack = isMovingToAttack;
        this.path = path;
    }

    //handles attack
    public void StartAttack()
    {
        //
        CurrentlySelectedPlayingUnit.CurrentState = UnitState.Attacking;
        //we show targeted unit's ui, and damage it
        TargetedUnit.TakeDamage(CurrentlySelectedPlayingUnit.Damage);

        //damage attacking unit with relation strike damage
        CurrentlySelectedPlayingUnit.TakeDamage((int)(TargetedUnit.Damage * 0.5));

        EndCurrentPlayingUnitTurn();
    }

    #region turn management

    // selects next unit in instantiated queque 
    public void SelectNextPlayingUnit()
    {
        int currentIndex = BattlefieldManager.ManagerInstance.InstantiatedUnits.IndexOf(OwningUnit);
        int nextIndex;

        if (currentIndex == BattlefieldManager.ManagerInstance.InstantiatedUnits.Count - 1)
        {
            nextIndex = 0;
        }
        else
        {
            nextIndex = currentIndex + 1;
        }

        CurrentlySelectedPlayingUnit = BattlefieldManager.ManagerInstance.InstantiatedUnits[nextIndex].GetComponent<UnitBehaviour>();
    }

    public void StartCurrentlyPlayingUnitTurn()
    {

        //sets starting tile
        BattlefieldManager.ManagerInstance.SetupStartingTile(CurrentlySelectedPlayingUnit.CurrentHexTile);


        //shows currently playing unit's ui
        CurrentlySelectedPlayingUnit.ShowUnitUI();

        SetupCurrentlyOwningUnit(CurrentlySelectedPlayingUnit);
    }

    public void StartCurrentlyPlayingUnitTurn(UnitBehaviour ub)
    {
        //UnitBehaviour ub = CurrentlySelectedPlayingUnit.GetComponent<UnitBehaviour>();
        CurrentlySelectedPlayingUnit = ub;

        //sets starting tile
        BattlefieldManager.ManagerInstance.SetupStartingTile(CurrentlySelectedPlayingUnit.CurrentHexTile);
        

        //shows currently playing unit's ui
        CurrentlySelectedPlayingUnit.ShowUnitUI();

        SetupCurrentlyOwningUnit(CurrentlySelectedPlayingUnit);
    }

    public void EndCurrentPlayingUnitTurn()
    {
        //don't know if this is necessary
        BattlefieldManager.ManagerInstance.StartingTile.ChangeHexVisualToDeselected();
        BattlefieldManager.ManagerInstance.DestinationTile.ChangeHexVisualToDeselected();

        
        //...setting previously occupied tile back to being passable...
        CurrentlySelectedPlayingUnit.CurrentHexTile.OwningTile.Occupied = false;
        CurrentlySelectedPlayingUnit.CurrentHexTile.ObjectOnHex = null;
        //...setting previous destination tile to be new current for moving unit and marking it as notpassable... 
        CurrentlySelectedPlayingUnit.CurrentHexTile = BattlefieldManager.ManagerInstance.DestinationTile;

        CurrentlySelectedPlayingUnit.CurrentHexTile.OwningTile.Occupied = true;
        CurrentlySelectedPlayingUnit.CurrentHexTile.ObjectOnHex = CurrentlySelectedPlayingUnit;
        //...reseting starting and destination tiles in order to destroy path...
        BattlefieldManager.ManagerInstance.StartingTile = null;
        BattlefieldManager.ManagerInstance.DestinationTile = null;

        //...this destroys path when the unit reaches destination...
        BattlefieldManager.ManagerInstance.GenerateAndShowPath();

        CurrentlySelectedPlayingUnit.HideUnitUI();
        CurrentlySelectedPlayingUnit.CurrentState = UnitState.Idle;

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

    public void SetupCurrentlyOwningUnit(UnitBehaviour ub)
    {
        OwningUnit = ub.gameObject;
        CurrentlySelectedPlayingUnit = ub;

        //caching the transform for better performance
        owningUnitTransform = OwningUnit.transform;

        //animation setup
        owningUnitAnimator = OwningUnit.GetComponent<Animator>();

        //SetMovingState(false);

        speed = ub.speed;
        rotationSpeed = ub.rotationSpeed;

        //ResetTilesInRange resets tiles which were in movement range of previous unit
        BattlefieldManager.ManagerInstance.ResetTilesInRange();

        BattlefieldManager.ManagerInstance.StartingTile = ub.CurrentHexTile;
        BattlefieldManager.ManagerInstance.SelectTilesInRangeSimple(ub.movementRange);
    }

    #endregion turn management
}
