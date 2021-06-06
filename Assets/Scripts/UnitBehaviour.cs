using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public enum UnitAction
{
    Move,
    Wait,
    Attack,
    Special
}

public enum UnitState
{
    Idle,
    Moving,
    Attacking,
    CastingSpell,
    Flanking
}

public class UnitBehaviour : MonoBehaviour, IIsOnHexGrid, ITakesDamage
{
    //Movement, speed will probably be replaced with tilesPerTurn
    public float speed = 0.0025f;
    public float rotationSpeed = 0.004f;
    //when does the unit takes turn
    public int initiative;
    //number of tiles unit can cover in single move action
    public int movementRange = 3;

    private HexBehaviour currentHexTile;
    //Attributes
    public int MaxHealth;
    public int CurrentHealth;
    public int Damage;
    public bool isAlive = true;

    //attack
    public int attackRange = 1;
    List<UnitBehaviour> enemiesInRange = new List<UnitBehaviour>();
    public bool hasAttacked;

    //actions
    protected UnitAction[] availableActions = {UnitAction.Move, UnitAction.Wait, UnitAction.Attack };
    //unitState
    public UnitState CurrentState;

    //player
    public int PlayerId;

    private UnitUI unitUI;

    public HexBehaviour CurrentHexTile
    {
        get { return currentHexTile; }
        set { currentHexTile = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        unitUI = gameObject.GetComponentInChildren<UnitUI>();
        SetupUI();
        CurrentState = UnitState.Idle;
    }

private void SetupUI()
    {
        unitUI.SetUIMaxHealth(MaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    //The corresponding OnMouseOver function is called while the mouse stays over the object and OnMouseExit is called when it moves away.
    void OnMouseEnter()
    {
        Debug.Log(gameObject.ToString() + " MouseEnter");
    }

    void OnMouseExit()
    {
        Debug.Log(gameObject.ToString() + " MouseExit");
    }


    //if we click on unit we get his tile as possible destination field instead
    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && !ActionManager.Instance.IsMoving && PlayerId != ActionManager.Instance.CurrentlySelectedPlayingUnit.GetComponent<UnitBehaviour>().PlayerId)
        {
            BattlefieldManager.ManagerInstance.DestinationTile = currentHexTile;
            Debug.Log("UnitBehaviour OnMouseOver()(), DestinationTile: " + (BattlefieldManager.ManagerInstance.DestinationTile ? BattlefieldManager.ManagerInstance.DestinationTile.coordinates : "null"));
            ActionManager.Instance.StartUnitActionOnHex(currentHexTile);
        }

        Debug.Log(gameObject.ToString() + " MouseOver");
    }

    public void GetEnemies()
    {
        enemiesInRange.Clear();

        foreach (UnitBehaviour unit in FindObjectsOfType<UnitBehaviour>())
        {
            
        }
    }

    public void ShowUnitUI()
    {
        if (unitUI==null) 
        {
            unitUI = gameObject.GetComponentInChildren<UnitUI>();
        }

        unitUI.EnableUnitUI();
    }

    public void HideUnitUI()
    {
        unitUI.DisableUnitUI();
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        unitUI.SetUIHealth(CurrentHealth);
    }
}
