using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitAction
{
    Move,
    Wait,
    Attack,
    Special
}

public class UnitBehaviour : MonoBehaviour
{
    //Movement, speed will probably be replaced with tilesPerTurn
    public float speed = 0.0025f;
    public float rotationSpeed = 0.004f;
    //when does the unit takes turn
    public int initiative;
    //number of tiles unit can cover in single move action
    public int movementRange = 3;

    public TileBehaviour currentTile;
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
    protected UnitAction[] availableActions = {UnitAction.Move, UnitAction.Wait, UnitAction.Attack};

    //player
    public int PlayerId;

    private UnitUI unitUI;

    // Start is called before the first frame update
    void Start()
    {
        unitUI = gameObject.GetComponentInChildren<UnitUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
