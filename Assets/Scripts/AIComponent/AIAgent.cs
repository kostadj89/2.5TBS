using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AIComponent;
using Assets.Scripts.AIComponent.Actions;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public enum AgentType
{
    UtilityAI,
    MinimaxAI
}

public class AIAgent : ScriptableObject
{

   public static AIAgent AIAgentInstanceAgent;

   public UnitBehaviour CurrentlyControledUnit;

   public AgentType AIType;

    //minimax min and max
    static int MAX = 1000;
    static int MIN = -1000;

    public AIAgent()
   {
       AIType = AgentType.UtilityAI;
       //AIType = AgentType.MinimaxAI;
       AIAgentInstanceAgent = this;
       ScoredActions = new List<IAction>();
   }

   public List<IAction> ScoredActions;

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
            iterator = new MoveToHexAction(CurrentlyControledUnit, hex);
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
            iterator = new AttackUnitOnHexAction(CurrentlyControledUnit,hex);
            attackActions.Add(iterator);
        }

        return attackActions;
    }

    public void ScoreActions()
   {
       List<IAction> availableActions = GetAvailableActions();

       foreach (IAction action in availableActions)
       {
           ScoredActions.Add(action);
       }
    }

    public void ChooseAction()
    {
        if (ScoredActions.Count > 0)
        {
            ScoredActions.Clear();
        }

        if (AIType == AgentType.UtilityAI)
        {
            ScoreActions();

            IAction chosenAction = ScoredActions.OrderByDescending(x => x.ScoredValue).First();

            chosenAction.DoAction();
        }
        else
        {
            StartMinimax();

        }

    }

    #region Minimax

    private void StartMinimax()
    {
        //here i would make first simulated state, so current unit is currentsimunit, actions to take would be chose actions, battlefieldstate will be currently alive units, ScoreWould be zero and then call minimax with that state as an argument
        ScoreActions();

        int numberOfActionsToConsider = ScoredActions.Count >= 4 ? 4 : ScoredActions.Count;

        List<IAction> chosenActions = ScoredActions.OrderByDescending(x=>x.ScoredValue).Take(4).ToList();
        //perhaps instead of 0 here, total healthpoint count could be added
        Debug.Log("Potential Actions:");
        foreach (IAction act in chosenActions)
        {
            act.Print();
        }

        SimulatedState simulation = new SimulatedState(chosenActions, GetBattlefieldState(),0,null);
        IAction actionToDo = Minimax(0,true,simulation,MIN,MAX);

        if (!chosenActions.Contains(actionToDo))
        {
            actionToDo = chosenActions.OrderByDescending(x => x.SimulatedValue).First();
        }

        Debug.Log("Chosen Action:");
        actionToDo.Print();


        actionToDo.DoAction();
    }

    private List<SimulatedUnit> GetBattlefieldState()
    {
        List<SimulatedUnit> simList = new List<SimulatedUnit>();

        foreach (GameObject unitGameObject in BattlefieldManager.ManagerInstance.InstantiatedUnits)
        {
           UnitBehaviour ub = unitGameObject.GetComponent<UnitBehaviour>();
           if (ub.isAlive)
           {
               simList.Add(new SimulatedUnit(ub,ub.CurrentHexTile,ub.CurrentHealth,ub.Equals(CurrentlyControledUnit)));
           }
        }

        return simList;
    }

    // Returns optimal action for
    // current player 
    static IAction Minimax(int depth, bool maximizingPlayer,SimulatedState simulationState, int alpha,
        int beta)
    {
        List<SimulatedState> nextSimulatedStates = simulationState.GetSimulatedStateAfterAction();
        // Terminating condition. i.e
        // leaf node is reached
        if (depth == 2 || nextSimulatedStates.Count == 0)
        {
            simulationState.PreviousAction.SimulatedValue = simulationState.actionsToTake
                .OrderByDescending(x => x.SimulatedValue).First().SimulatedValue;
            return simulationState.PreviousAction;
        }


        if (maximizingPlayer)
        {
            IAction best = new EmptySimulatedAction(MIN);

            // Recur for left and
            // right children
            foreach (SimulatedState ss in nextSimulatedStates)
            {
                IAction minimaxedAction = Minimax(depth + 1, false, ss, alpha, beta);
                int val = minimaxedAction.SimulatedValue;
                best = best.SimulatedValue <= val ? minimaxedAction : best;/* new EmptySimulatedAction(Math.Max(best.SimulatedValue, val));*/
                alpha = Math.Max(alpha, best.SimulatedValue);

                // Alpha Beta Pruning
                if (beta <= alpha)
                    break;
            }
            return best;
        }
        else
        {
            IAction best = new EmptySimulatedAction(MAX);

            // Recur for left and
            // right children
            foreach (SimulatedState ss in nextSimulatedStates)
            {
                IAction minimaxedAction = Minimax(depth + 1,true, ss, alpha, beta);
                int val = minimaxedAction.SimulatedValue;
                best = best.SimulatedValue < val ? best : minimaxedAction;/*new EmptySimulatedAction(Math.Min(best.SimulatedValue, val));*/
                beta = Math.Min(beta, best.SimulatedValue);

                // Alpha Beta Pruning
                if (beta <= alpha)
                    break;
            }
            return best;
        }
    }

    private void ScoreSimulatedActions()
    {
        throw new NotImplementedException();
    }

    #endregion Minimax
    }
