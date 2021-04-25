using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexBehaviour : MonoBehaviour
{
    //reference to HexTile object, which is used in pathfinding
    public HexTile OwningTile;

    //reference to object contained on this hex
    public IIsOnHexGrid ObjectOnHex;

    public SpriteRenderer TileBehaviourSpriteRenderer;

    public Sprite NormalLookingHex;
    public Sprite SelectedLookingHex;
    public Sprite HazardousLookingHex;

    Color MouseOverColor = new Color(255f/255f, 255f / 255f, 255f / 255f, 127f/255f);

    //public bool isInRange = true;

    //testing
    public string coordinates;
    // used for setting a unit to it's position
    public Vector3 UnitAnchorWorldPositionVector;

    void Awake()
    {
        TileBehaviourSpriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    public void ChangeVisualToSelected()
    {
        ChangeHexVisual(Color.white, SelectedLookingHex);
    }

    public void ChangeHexVisual(Color color, Sprite sprite)
    {
        TileBehaviourSpriteRenderer.material.color = color;
        TileBehaviourSpriteRenderer.sprite = sprite;
    }

    public void ChangeHexVisual(Color color)
    {
        TileBehaviourSpriteRenderer.material.color = color;
    }

    public void ChangeHexVisualToDeselected()
    {
        TileBehaviourSpriteRenderer.material.color = Color.white;
        TileBehaviourSpriteRenderer.sprite = this.OwningTile.Hazadours ? HazardousLookingHex : NormalLookingHex;
    }

    //The corresponding OnMouseOver function is called while the mouse stays over the object and OnMouseExit is called when it moves away.
    void OnMouseEnter()
    {
        if (!ActionManager.Instance.IsMoving)
        {
            if (!OwningTile.IsInRange || !OwningTile.Passable)
            {
                ChangeDestinationToStart();
            }
            //we're generating the path each time we hover over potential destination tile
            else if (OwningTile.Passable && this != BattlefieldManager.ManagerInstance.DestinationTile && this != BattlefieldManager.ManagerInstance.StartingTile)
            {
                if (OwningTile.Occupied)
                {
                    
                }
                
                ChangeDestinationToThis();
            }

            BattlefieldManager.ManagerInstance.GenerateAndShowPath();
        }
    }

    void OnMouseExit()
    {
        //if (OwningTile.Passable  && this != BattlefieldManager.ManagerInstance.DestinationTile && this != BattlefieldManager.ManagerInstance.StartingTile)
        if (OwningTile.Passable && OwningTile.IsInRange && this != BattlefieldManager.ManagerInstance.StartingTile && !ActionManager.Instance.IsMoving)
        {
            if (this.OwningTile.Hazadours)
            {
                ChangeHexVisual(Color.gray,HazardousLookingHex);
            }
            else
            {
                ChangeHexVisual(Color.gray, NormalLookingHex);
            }
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && !ActionManager.Instance.IsMoving && OwningTile.Passable && OwningTile.IsInRange)
        {
            ActionManager.Instance.StartUnitActionOnHex(this);
        }
    }

    public void ChangeDestinationToThis()
    {
        //HexBehaviour destination = BattlefieldManager.ManagerInstance.DestinationTile;

        //if (this == destination)
        //{
        //    BattlefieldManager.ManagerInstance.DestinationTile = null;
        //    TileBehaviourSpriteRenderer.color = Color.white;
        //    return;
        //}

        // we change the actual destination tile to the current one
        BattlefieldManager.ManagerInstance.DestinationTile = this;
        ChangeHexVisual(MouseOverColor, SelectedLookingHex);
    }

    //when we're over inacessable tile
    private void ChangeDestinationToStart()
    {
        BattlefieldManager.ManagerInstance.DestinationTile = BattlefieldManager.ManagerInstance.StartingTile;
    }

    private void StartingTileChanged(HexBehaviour origin)
    {
        if (this == origin)
        {
            BattlefieldManager.ManagerInstance.StartingTile = null;
            TileBehaviourSpriteRenderer.sprite = NormalLookingHex;
            return;
        }

        BattlefieldManager.ManagerInstance.StartingTile = this;
        ChangeVisualToSelected();
    }
}

public interface IIsOnHexGrid
{
    HexBehaviour CurrentHexTile { get; set; }
}
