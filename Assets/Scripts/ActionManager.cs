using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    #region private fields

    //private CharacterController owningUnitCharacterController;

    

    //i want to have only one object of script of this type, and pass it around units, so these two fields are gonna be transform of creature whose turn is now
    private Transform owningUnitTransform;
    private Animator owningUnitAnimator;


    #endregion private fields

    #region public fields

    public GameObject OwningUnit;
    public UnitBehaviour CurrentlySelectedPlayingUnit;

    //unit who is a target of an action
    public UnitBehaviour TargetedUnit;

    #endregion public fields

    #region static fields

    //singleton, why:/
    public static ActionManager Instance = null;

    #endregion static fields

    #region props

    public bool IsMoving
    {
        get { return CurrentlySelectedPlayingUnit.CurrentState == UnitState.Moving; }
    }
    public bool IsMovingToAttack { get; private set; }

    #endregion props

    void Awake()
    {
        //singleton pattern here is used just for the sake of simplicity.should be used in cases when this script is attached to more than one character
        Instance = this;
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

                CurrentlySelectedPlayingUnit.MovementComponent.Move();
                if (CurrentlySelectedPlayingUnit.MovementComponent.HasFinishedMoving)
                {
                    if (CurrentlySelectedPlayingUnit.MovementComponent.IsMovingToAttack)
                    {
                        CurrentlySelectedPlayingUnit.MovementComponent.IsMovingToAttack = false;
                        
                        CurrentlySelectedPlayingUnit.AttackComponent.StartAttack();
                    }

                    CurrentlySelectedPlayingUnit.SetIdleState();

                    EndCurrentPlayingUnitTurn();
                }

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
                CurrentlySelectedPlayingUnit.MovementComponent.InitializeMoving(targetHexBehaviour);
                break;
        }

    }

    //handles attack
    public void StartAttack()
    {
        ////
        //CurrentlySelectedPlayingUnit.CurrentState = UnitState.Attacking;
        ////we show targeted unit's ui, and damage it
        //TargetedUnit.TakeDamage(CurrentlySelectedPlayingUnit.Damage);

        ////damage attacking unit with relation strike damage
        //CurrentlySelectedPlayingUnit.TakeDamage((int)(TargetedUnit.Damage * 0.5));

        //EndCurrentPlayingUnitTurn();
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
        //CurrentlySelectedPlayingUnit.ShowUnitUI();

        SetupCurrentlyOwningUnit(CurrentlySelectedPlayingUnit);
    }

    public void StartCurrentlyPlayingUnitTurn(UnitBehaviour ub)
    {
        //UnitBehaviour ub = CurrentlySelectedPlayingUnit.GetComponent<UnitBehaviour>();
        CurrentlySelectedPlayingUnit = ub;

        //sets starting tile
        BattlefieldManager.ManagerInstance.SetupStartingTile(CurrentlySelectedPlayingUnit.CurrentHexTile);
        

        //shows currently playing unit's ui
        //CurrentlySelectedPlayingUnit.ShowUnitUI();

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
        Debug.Log("EndCurrentPlayingUnitTurn, StartingTile: " + (BattlefieldManager.ManagerInstance.StartingTile ? BattlefieldManager.ManagerInstance.StartingTile.coordinates : "null")+"; DestinationTile: " + (BattlefieldManager.ManagerInstance.DestinationTile ? BattlefieldManager.ManagerInstance.DestinationTile.coordinates : "null"));
        CurrentlySelectedPlayingUnit.CurrentHexTile = BattlefieldManager.ManagerInstance.DestinationTile;

        CurrentlySelectedPlayingUnit.CurrentHexTile.OwningTile.Occupied = true;
        CurrentlySelectedPlayingUnit.CurrentHexTile.ObjectOnHex = CurrentlySelectedPlayingUnit;
        //...reseting starting and destination tiles in order to destroy path...
        BattlefieldManager.ManagerInstance.StartingTile = null;

        Debug.Log("EndCurrentPlayingUnitTurn, should be null,  StartingTile: " + (BattlefieldManager.ManagerInstance.StartingTile ? BattlefieldManager.ManagerInstance.StartingTile.coordinates: "null"));

        BattlefieldManager.ManagerInstance.DestinationTile = null;
        Debug.Log("EndCurrentPlayingUnitTurn(), should be null, DestinationTile: " + (BattlefieldManager.ManagerInstance.DestinationTile ? BattlefieldManager.ManagerInstance.DestinationTile.coordinates : "null"));

        //...this destroys path when the unit reaches destination...
        BattlefieldManager.ManagerInstance.GenerateAndShowPath();

        //CurrentlySelectedPlayingUnit.HideUnitUI();
        CurrentlySelectedPlayingUnit.CurrentState = UnitState.Idle;

        //detarget targeted unit
        if (TargetedUnit != null)
        {
           // TargetedUnit.HideUnitUI();
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
        //owningUnitTransform = OwningUnit.transform;

        //animation setup
        //owningUnitAnimator = OwningUnit.GetComponent<Animator>();

        //SetMovingState(false);

        //speed = ub.speed;
        //rotationSpeed = ub.rotationSpeed;

        //ResetTilesInRange resets tiles which were in movement range of previous unit
        BattlefieldManager.ManagerInstance.ResetTilesInRange();

        BattlefieldManager.ManagerInstance.StartingTile = ub.CurrentHexTile;

        Debug.Log("SetupCurrentlyOwningUnit(UnitBehaviour ub), StartingTile: " + (BattlefieldManager.ManagerInstance.StartingTile ? BattlefieldManager.ManagerInstance.StartingTile.coordinates : "null"));

        BattlefieldManager.ManagerInstance.SelectTilesInRangeSimple(ub.movementRange);
    }

    #endregion turn management
}
