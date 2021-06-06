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
    public Sprite ReachableLookingHex;
    public Sprite OcuppiedLookingHex;

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

    public void ChangeHexVisualToDeselected()
    {
        ChangeHexVisual(Color.white, this.OwningTile.Hazadours ? HazardousLookingHex : NormalLookingHex);
    }

    public void ChangeHexVisualToHazardous()
    {
        ChangeHexVisual(Color.yellow, HazardousLookingHex);
    }

    public void ChangeHexVisualToOccupied()
    {
        ChangeHexVisual(Color.red, OcuppiedLookingHex);
    }

    public void ChangeVisualToReachable()
    {
        ChangeHexVisual(Color.white, ReachableLookingHex);
    }

    public void ChangeVisualToNormal()
    {
        ChangeHexVisual(Color.white, NormalLookingHex);
    }

    private void ChangeHexVisual(Color color, Sprite sprite)
    {
        TileBehaviourSpriteRenderer.material.color = color;
        TileBehaviourSpriteRenderer.sprite = sprite;
    }

    private void ChangeHexVisual(Color color)
    {
        TileBehaviourSpriteRenderer.material.color = color;
    }


    //The corresponding OnMouseOver function is called while the mouse stays over the object and OnMouseExit is called when it moves away.
    void OnMouseEnter()
    {
        if (!ActionManager.Instance.IsMoving)
        {
            if ((OwningTile.IsInRange && OwningTile.Passable && this != BattlefieldManager.ManagerInstance.DestinationTile && this != BattlefieldManager.ManagerInstance.StartingTile)||(OwningTile.Occupied && OwningTile.ReachableNeighbours.Count()>0))
            {
                ChangeDestinationToThis();
            }
            //we're generating the path each time we hover over potential destination tile
            else //if (!OwningTile.IsInRange || !OwningTile.Passable)
            {
                ChangeDestinationToStart();
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
                ChangeHexVisualToHazardous();
            }
            else
            {
                ChangeVisualToReachable();
            }
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && !ActionManager.Instance.IsMoving && ((OwningTile.Passable && OwningTile.IsInRange) || (OwningTile.Occupied && OwningTile.ReachableNeighbours.Count() > 0)))
        {
            ActionManager.Instance.StartUnitActionOnHex(this);
        }
    }

    public void ChangeDestinationToThis()
    {
        // we change the actual destination tile to the current one
        BattlefieldManager.ManagerInstance.DestinationTile = this;
        Debug.Log("ChangeDestinationToThis(), DestinationTile: " + (BattlefieldManager.ManagerInstance.DestinationTile ? BattlefieldManager.ManagerInstance.DestinationTile.coordinates : "null"));
        ChangeHexVisual(MouseOverColor, SelectedLookingHex);
    }

    //when we're over inacessable tile
    private void ChangeDestinationToStart()
    {
        BattlefieldManager.ManagerInstance.DestinationTile = BattlefieldManager.ManagerInstance.StartingTile;
        Debug.Log("ChangeDestinationToStart(), DestinationTile: " + (BattlefieldManager.ManagerInstance.DestinationTile ? BattlefieldManager.ManagerInstance.DestinationTile.coordinates : "null"));

    }

    private void StartingTileChanged(HexBehaviour origin)
    {
        if (this == origin)
        {
            BattlefieldManager.ManagerInstance.StartingTile = null;
            Debug.Log("StartingTileChanged(HexBehaviour origin), should be null, StartingTile: " + (BattlefieldManager.ManagerInstance.StartingTile ? BattlefieldManager.ManagerInstance.StartingTile.coordinates : "null"));
            TileBehaviourSpriteRenderer.sprite = NormalLookingHex;
            return;
        }

        BattlefieldManager.ManagerInstance.StartingTile = this;
        Debug.Log("StartingTileChanged(HexBehaviour origin), StartingTile: " + (BattlefieldManager.ManagerInstance.StartingTile ? BattlefieldManager.ManagerInstance.StartingTile.coordinates : "null"));
        ChangeVisualToSelected();
    }

}

public interface IIsOnHexGrid
{
    HexBehaviour CurrentHexTile { get; set; }
}
