using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AIComponent;
using UnityEngine;

public class AIAgent : ScriptableObject
{
   public static AIAgent AIAgentInstanceAgent;

   public UnitBehaviour CurrentlyControledUnit;

   public AIAgent()
   {
       AIAgentInstanceAgent = this;
       ScoredActions = new Dictionary<IAction, float>();
   }

   public Dictionary<IAction, float> ScoredActions;

   public List<IAction> GetAvailableActions()
   {
       List<IAction> availableActions = new List<IAction>();
        // ok i'm trying to get unified place where i can collect of the possible actions..
        //so  actions are split into attack actions and move actions..
       availableActions.AddRange(GetAttackActions());

       availableActions.AddRange(GetMoveActions());

       return availableActions;
   }

    private List<IAction> GetMoveActions()
    {
        List<HexBehaviour> HexesInRange = BattlefieldManager.ManagerInstance.GetTilesInRange(
            CurrentlyControledUnit.CurrentHexTile.UnitAnchorWorldPositionVector, CurrentlyControledUnit.movementRange).Where(x=>!x.OwningTile.Occupied && x.OwningTile.Passable).ToList();

        List<IAction> moveActions = new List<IAction>();

        MoveToHexAction iterator;
        foreach (HexBehaviour hex in HexesInRange)
        {
            iterator = new MoveToHexAction(hex);
            moveActions.Add(iterator);
        }

        return moveActions;
    }

    private List<IAction> GetAttackActions()
    {
        List<HexBehaviour> HexesOccupiedbyEnemy = BattlefieldManager.ManagerInstance.GetAllEnemiesInRange();

        List<IAction> attackActions = new List<IAction>();

        AttackUnitOnHexAction iterator;
        foreach (HexBehaviour hex in HexesOccupiedbyEnemy)
        {
            iterator = new AttackUnitOnHexAction(hex);
            attackActions.Add(iterator);
        }

        return attackActions;
    }

    public void ScoreActions()
   {
       List<IAction> availableActions = GetAvailableActions();

       foreach (IAction action in availableActions)
       {
           ScoredActions.Add(action, action.GetScore());
       }
    }

   public void ChooseAction()
   {
       if (ScoredActions.Count>0)
       {
           ScoredActions.Clear();
       }
       
       ScoreActions();

       IAction chosenAction = ScoredActions.OrderByDescending(x => x.Value).First().Key;

       chosenAction.DoAction();
    }

}
