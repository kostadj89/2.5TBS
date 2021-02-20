using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public Tile OwningTile;

    //public Material OpaqueMaterial;
    //public Material DefaultMaterial;

    public SpriteRenderer TileBehaviourSpriteRenderer;

    public Sprite OriginalSprite;
    public Sprite SelectedSprite;

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
        ChangeHexVisual(Color.white, SelectedSprite);
    }

    public void ChangeHexVisual(Color color, Sprite sprite)
    {
        TileBehaviourSpriteRenderer.material.color = color;
        TileBehaviourSpriteRenderer.sprite = sprite;
    }

    //The corresponding OnMouseOver function is called while the mouse stays over the object and OnMouseExit is called when it moves away.
    void OnMouseEnter()
    {
        if (!UnitMovement.instance.IsMoving)
        {
            if (!OwningTile.IsInRange || !OwningTile.Passable)
            {
                ChangeDestinationToStart();
            }
            else if (OwningTile.Passable && this != BattlefieldManager.ManagerInstance.DestinationTile && this != BattlefieldManager.ManagerInstance.StartingTile)
            {
                //we're generating the path each time we hover over potential destination tile
                ChangeDestinationToThis();
                //ChangeHexVisual(MouseOverColor, SelectedSprite);
            }

            BattlefieldManager.ManagerInstance.GenerateAndShowPath();
        }
    }

    void OnMouseExit()
    {
        //if (OwningTile.Passable  && this != BattlefieldManager.ManagerInstance.DestinationTile && this != BattlefieldManager.ManagerInstance.StartingTile)
        if (OwningTile.Passable && OwningTile.IsInRange && this != BattlefieldManager.ManagerInstance.StartingTile && !UnitMovement.instance.IsMoving)
        {
            ChangeHexVisual(Color.gray, OriginalSprite);
            //BattlefieldManager.ManagerInstance.GenerateAndShowPath(false);
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && !UnitMovement.instance.IsMoving && OwningTile.Passable && OwningTile.IsInRange)
        {
            //we color the selected path to real white
            ChangeHexVisual(Color.white, SelectedSprite);

            BattlefieldManager.ManagerInstance.GenerateAndShowPath();

            var path = Pathfinder.FindPath(BattlefieldManager.ManagerInstance.StartingTile.OwningTile, BattlefieldManager.ManagerInstance.DestinationTile.OwningTile);
            UnitMovement.instance.StartMoving(path.ToList());
        }
    }

    private void ChangeDestinationToThis()
    {
        //TileBehaviour destination = BattlefieldManager.ManagerInstance.DestinationTile;

        //if (this == destination)
        //{
        //    BattlefieldManager.ManagerInstance.DestinationTile = null;
        //    TileBehaviourSpriteRenderer.color = Color.white;
        //    return;
        //}

        // we change the actual destination tile to the current one
        BattlefieldManager.ManagerInstance.DestinationTile = this;
        ChangeHexVisual(MouseOverColor, SelectedSprite);
    }

    //when we're over inacessable tile
    private void ChangeDestinationToStart()
    {
        BattlefieldManager.ManagerInstance.DestinationTile = BattlefieldManager.ManagerInstance.StartingTile;
    }

    private void StartingTileChanged(TileBehaviour origin)
    {
        if (this == origin)
        {
            BattlefieldManager.ManagerInstance.StartingTile = null;
            TileBehaviourSpriteRenderer.sprite = OriginalSprite;
            return;
        }

        BattlefieldManager.ManagerInstance.StartingTile = this;
        ChangeHexVisual(Color.white,SelectedSprite);
    }
}
