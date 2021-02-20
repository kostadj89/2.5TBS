using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        SetupUI();
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
        //if (!UnitMovement.instance.IsMoving)
        //{
        //    if (!currentTile.OwningTile.IsInRange || !currentTile.OwningTile.Passable)
        //    {
        //       // ChangeDestinationToStart();
        //    }
        //    else if (currentTile.OwningTile.Passable && this != BattlefieldManager.ManagerInstance.DestinationTile && this != BattlefieldManager.ManagerInstance.StartingTile)
        //    {
        //       // //we're generating the path each time we hover over potential destination tile
        //        //ChangeDestinationToThis();
        //        //ChangeHexVisual(MouseOverColor, SelectedSprite);
        //    }

        //    BattlefieldManager.ManagerInstance.GenerateAndShowPath();
        //}

        Debug.Log(gameObject.ToString() + " MouseEnter");
    }

    void OnMouseExit()
    {
        ////if (OwningTile.Passable  && this != BattlefieldManager.ManagerInstance.DestinationTile && this != BattlefieldManager.ManagerInstance.StartingTile)
        //if (OwningTile.Passable && OwningTile.IsInRange && this != BattlefieldManager.ManagerInstance.StartingTile && !UnitMovement.instance.IsMoving)
        //{
        //    ChangeHexVisual(Color.gray, OriginalSprite);
        //    //BattlefieldManager.ManagerInstance.GenerateAndShowPath(false);
        //}
        Debug.Log(gameObject.ToString() + " MouseExit");
    }


    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && !UnitMovement.instance.IsMoving && gameObject != BattlefieldManager.ManagerInstance.CurrentlySelectedPlayingUnit)
        {
            /*Ok here we'll take all neighbours which are passable and in range, and move to the closest one*/
            List<Tile> neighbourTiles = currentTile.OwningTile.Neighbours.Where(x => x.IsInRange).ToList();

            if (neighbourTiles!=null && neighbourTiles.Count>0)
            {
                //we color the selected path to real white
                //BattlefieldManager.ManagerInstance.DestinationTile = currentTile;
                currentTile.ChangeHexVisual(Color.red, currentTile.SelectedSprite);

                //need to se how to get min distanced tile, for now this
                Tile minDistanceTile = neighbourTiles.First();

                TileBehaviour targetTileBehaviour =
                    BattlefieldManager.ManagerInstance.GeTileBehaviourFromPoint(minDistanceTile.Location);


                BattlefieldManager.ManagerInstance.DestinationTile = targetTileBehaviour;
                targetTileBehaviour.ChangeHexVisual(Color.white, targetTileBehaviour.SelectedSprite);
                BattlefieldManager.ManagerInstance.GenerateAndShowPath();

                BattlefieldManager.ManagerInstance.TargetedUnit = gameObject.GetComponent<UnitBehaviour>();

                BattlefieldManager.ManagerInstance.TargetedUnit.ShowUnitUI();

                var path = Pathfinder.FindPath(BattlefieldManager.ManagerInstance.StartingTile.OwningTile, BattlefieldManager.ManagerInstance.DestinationTile.OwningTile);
                UnitMovement.instance.StartMoving(path.ToList(), true);
            }
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
