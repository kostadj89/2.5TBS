using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.UnitComponents.Attack;
using Assets.Scripts.UnitComponents.Movement;
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

public enum MovementType
{
    Walking,
    Flying,
    Teleporting
}

public enum AttackType
{
    Melee,
    Ranged
}

public class UnitBehaviour : MonoBehaviour, IIsOnHexGrid, ITakesDamage
{
    #region Fields

    //ui
    private UnitUI unitUI;
    //current hex
    private HexBehaviour currentHexTile;
    //animator
    private Animator animator;

    //Movement, speed will probably be replaced with tilesPerTurn
    public float speed = 0.0025f;
    public float rotationSpeed = 0.004f;
   
    //number of tiles unit can cover in single move action
    public int movementRange = 3;

    
    //Attributes
    public int MaxHealth;
    public int CurrentHealth;
    public int Damage;
    public bool isAlive = true;
    //when does the unit takes turn
    public int Initiative;

    //attack
    public int attackRange = 1;
    List<UnitBehaviour> enemiesInRange = new List<UnitBehaviour>();
    public bool hasAttacked;

    //actions
    protected UnitAction[] availableActions = { UnitAction.Move, UnitAction.Wait, UnitAction.Attack };
    //unitState
    public UnitState CurrentState;

    //player
    public int PlayerId;

    //movement type
    public MovementType MovementType;
    //Attack type
    public AttackType AttackType;
    #endregion

    #region Properties

    public HexBehaviour CurrentHexTile
    {
        get { return currentHexTile; }
        set { currentHexTile = value; }
    }

    public IMovementComponent MovementComponent { get; set; }
    public IAttackComponent AttackComponent { get; set; }

    #endregion

    #region Overrides

    // Start is called before the first frame update
    void Start()
    {
        unitUI = gameObject.GetComponentInChildren<UnitUI>();
        SetupUI();
        CurrentState = UnitState.Idle;

        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Type GetObjectType()
    {
        return typeof(UnitBehaviour);
    }

    #endregion

    #region Methods

    public bool HexContainsAnEnemy(HexBehaviour targetHexBehaviour)
    {
        if (targetHexBehaviour.ObjectOnHex!=null && targetHexBehaviour.ObjectOnHex.GetObjectType() == typeof(UnitBehaviour))
        {
            return this.PlayerId != ((UnitBehaviour) targetHexBehaviour.ObjectOnHex).PlayerId;
        }
        else
        {
            return false;
        }
    }

    public bool HexContainsAnAlly(HexBehaviour targetHexBehaviour)
    {
        if (targetHexBehaviour.ObjectOnHex.GetObjectType() == typeof(UnitBehaviour))
        {
            return this.PlayerId == ((UnitBehaviour)targetHexBehaviour.ObjectOnHex).PlayerId;
        }
        else
        {
            return false;
        }
    }


    public void GetEnemies()
    {
        enemiesInRange.Clear();

        foreach (UnitBehaviour unit in FindObjectsOfType<UnitBehaviour>())
        {

        }
    }

    public void SetMovingState()
    {
        CurrentState = UnitState.Moving;
        animator.Play("walking");
    }

    public void SetIdleState()
    {
        CurrentState = UnitState.Idle;
        animator.Play("Idle");
    }

    public void InitializeMovementComponent()
    {
        switch (MovementType)
        {
                
            case MovementType.Flying:
                break;
            case MovementType.Teleporting:
                break;
            case MovementType.Walking:
                MovementComponent = new GroundMovement();
                break;

        }

        MovementComponent.InitializeComponent(this);
    }

    public void InitializeAttackComponent()
    {
        switch (AttackType)
        {

            case AttackType.Ranged:
                AttackComponent = new RangedAttack();
                break;
            case AttackType.Melee:
                AttackComponent = new MeleeAttack();
                break;

        }

        AttackComponent.InitializeComponent(this);
    }

    #endregion

    #region Events

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
        if (Input.GetMouseButtonUp(0) && !ActionManager.Instance.IsMoving && PlayerId != ActionManager.Instance.CurrentlySelectedPlayingUnit.GetComponent<UnitBehaviour>().PlayerId && CurrentHexTile.OwningTile.ReachableNeighbours.Count() > 0)
        {
           //currentHexTile.ChangeDestinationToThis();
            Debug.Log("UnitBehaviour OnMouseOver()(), DestinationTile: " + (BattlefieldManager.ManagerInstance.DestinationTile ? BattlefieldManager.ManagerInstance.DestinationTile.coordinates : "null"));
            ActionManager.Instance.StartUnitActionOnHex(CurrentHexTile);
        }

        Debug.Log(gameObject.ToString() + " MouseOver");
    }

    #endregion

    #region Unit UI

    private void SetupUI()
    {
        unitUI.SetUIMaxHealth(MaxHealth);
    }

    private void UpdateHealthUI()
    {
        unitUI.SetUIHealth(CurrentHealth);
    }

    public void ShowUnitUI()
    {
        if (unitUI == null)
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

    #endregion

}
