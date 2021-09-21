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
    static decimal MAX = 1000;
    static decimal MIN = -1000;

    public AIAgent()
   {
       //AIType = AgentType.UtilityAI;
      AIType = AgentType.MinimaxAI;
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
            //StartMinimax();
            StartMinimax2();
        }
    }

    //#region Minimax

    //private void StartMinimax()
    //{
    //    //here i would make first simulated state, so current unit is currentsimunit, actions to take would be chose actions, battlefieldstate will be currently alive units, ScoreWould be zero and then call minimax with that state as an argument
    //    ScoreActions();

    //    int numberOfActionsToConsider = ScoredActions.Count >= 4 ? 4 : ScoredActions.Count;

    //    List<IAction> chosenActions = ScoredActions.OrderByDescending(x=>x.ScoredValue).Take(4).ToList();
    //    //perhaps instead of 0 here, total healthpoint count could be added
    //    Debug.Log("Potential Actions:");
    //    foreach (IAction act in chosenActions)
    //    {
    //        act.Print();
    //    }

    //    SimulatedState simulation = new SimulatedState(chosenActions, GetBattlefieldState(),0,null);
    //    IAction actionToDo = Minimax(0,true,simulation,MIN,MAX);

    //    if (!chosenActions.Contains(actionToDo))
    //    {
    //        actionToDo = chosenActions.OrderByDescending(x => x.SimulatedValue).First();
    //    }

    //    Debug.Log("Chosen Action:");
    //    actionToDo.Print();


    //    actionToDo.DoAction();
    //}

    //private List<SimulatedUnit> GetBattlefieldState()
    //{
    //    List<SimulatedUnit> simList = new List<SimulatedUnit>();

    //    foreach (GameObject unitGameObject in BattlefieldManager.ManagerInstance.CurrentStateOfGame.InstantiatedUnits)
    //    {
    //       UnitBehaviour ub = unitGameObject.GetComponent<UnitBehaviour>();
    //       if (ub.isAlive)
    //       {
    //           simList.Add(new SimulatedUnit(ub,ub.CurrentHexTile,ub.CurrentHealth,ub.Equals(CurrentlyControledUnit)));
    //       }
    //    }

    //    return simList;
    //}

    //// Returns optimal action for
    //// current player 
    //static IAction Minimax(int depth, bool maximizingPlayer,SimulatedState simulationState, int alpha,
    //    int beta)
    //{
    //    List<SimulatedState> nextSimulatedStates = simulationState.GetSimulatedStateAfterAction();
    //    // Terminating condition. i.e
    //    // leaf node is reached
    //    if (depth == 2 || nextSimulatedStates.Count == 0)
    //    {
    //        simulationState.PreviousAction.SimulatedValue = simulationState.actionsToTake
    //            .OrderByDescending(x => x.SimulatedValue).First().SimulatedValue;
    //        return simulationState.PreviousAction;
    //    }


    //    if (maximizingPlayer)
    //    {
    //        IAction best = new EmptySimulatedAction(MIN);

    //        // Recur for left and
    //        // right children
    //        foreach (SimulatedState ss in nextSimulatedStates)
    //        {
    //            IAction minimaxedAction = Minimax(depth + 1, false, ss, alpha, beta);
    //            int val = minimaxedAction.SimulatedValue;
    //            best = best.SimulatedValue <= val ? minimaxedAction : best;/* new EmptySimulatedAction(Math.Max(best.SimulatedValue, val));*/
    //            alpha = Math.Max(alpha, best.SimulatedValue);

    //            // Alpha Beta Pruning
    //            if (beta <= alpha)
    //                break;
    //        }
    //        return best;
    //    }
    //    else
    //    {
    //        IAction best = new EmptySimulatedAction(MAX);

    //        // Recur for left and
    //        // right children
    //        foreach (SimulatedState ss in nextSimulatedStates)
    //        {
    //            IAction minimaxedAction = Minimax(depth + 1,true, ss, alpha, beta);
    //            int val = minimaxedAction.SimulatedValue;
    //            best = best.SimulatedValue < val ? best : minimaxedAction;/*new EmptySimulatedAction(Math.Min(best.SimulatedValue, val));*/
    //            beta = Math.Min(beta, best.SimulatedValue);

    //            // Alpha Beta Pruning
    //            if (beta <= alpha)
    //                break;
    //        }
    //        return best;
    //    }
    //}

    //private void ScoreSimulatedActions()
    //{
    //    throw new NotImplementedException();
    //}

    //#endregion Minimax

    #region Minimax2

    private void StartMinimax2()
    {
        //here i would make first simulated state, so current unit is currentsimunit, actions to take would be chose actions, battlefieldstate will be currently alive units, ScoreWould be zero and then call minimax with that state as an argument
        ScoreActions();

        int numberOfActionsToConsider = ScoredActions.Count >= 3 ? 3 : ScoredActions.Count;

        List<IAction> chosenActions = ScoredActions.OrderByDescending(x => x.ScoredValue).Take(3).ToList();
        //perhaps instead of 0 here, total healthpoint count could be added
        Debug.Log("Potential Actions:");
        foreach (IAction act in chosenActions)
        {
            act.Print();
        }

        string st = String.Empty;

        //SimulatedState simulation = new SimulatedState(chosenActions, GetBattlefieldState2(), 0, null);
        Minimax2(0, true, ActionManager.Instance.CurrentlySelectedPlayingUnit.PlayerId, chosenActions, MIN, MAX, st);

        IAction actionToDo = chosenActions.OrderByDescending(x => x.SimulatedValue).First();
        //if (!chosenActions.Contains(actionToDo))
        //{
        //    actionToDo = chosenActions.OrderByDescending(x => x.SimulatedValue).First();
        //}

        Debug.Log("Possible Actions:");
        foreach (IAction action in chosenActions)
        {
            action.Print();
        }

        Debug.Log("Chosen Action:");
        actionToDo.Print();
        Debug.Log(st);

        BattlefieldManager.ManagerInstance.RevertToOriginalGameState();

        actionToDo.DoAction();
    }
    
    // Updates actions with scores and returns the best
    static void Minimax2(int depth, bool maximizingPlayer, int currentPlayerID, List<IAction> actions, decimal alpha,
        decimal beta, string st)
    {
        //if (BattlefieldManager.ManagerInstance.CurrentStateIndex!=0)
        //{
            //foreach (IAction action in actions)
            //{
            //    BattlefieldManager.ManagerInstance.CreateNewAndChangeCurrentGameStat();
            //    BattlefieldManager.ManagerInstance.CurrentStateOfGame.actionToDoInState = action;
                
            //    //create new action based on action in old game state
            //    IAction newStateAction = CreateActionForNewState(action);
            //    newStateAction.DoAction();
            //}
        //}
       
        // Terminating condition. i.e
        // leaf node is reached
        if (depth == 2)// || nextSimulatedStates.Count == 0)
        {
            decimal currentPlayerHealth =
                BattlefieldManager.ManagerInstance.CurrentStateOfGame.GetHealthScoreOfTheStateForPlayer(
                    currentPlayerID);
            decimal currentEnemyHealth =
                BattlefieldManager.ManagerInstance.CurrentStateOfGame.GetHealthScoreOfTheStateForPlayer(
                    Math.Abs(currentPlayerID - 1));

            foreach (IAction action in actions)
            {
                action.SimulatedValue = action.SimulateScoreForHealth(currentPlayerHealth, currentEnemyHealth);
            }

            //st = "depth " + depth + ":";
            //foreach (IAction ac in actions)
            //{
            //    st += ac.ToString() + "_" + ac.SimulatedValue;
            //}

            return;
        }


        if (maximizingPlayer)
        {
            decimal bestValue = MIN;

            // Recur for left and
            // right children
            foreach (IAction action in actions)
            {
                BattlefieldManager.ManagerInstance.CreateNewAndChangeCurrentGameStat();
                BattlefieldManager.ManagerInstance.CurrentStateOfGame.actionToDoInState = action;

                //create new action based on action in old game state
                IAction newStateAction = CreateActionForNewState(action);
                //newStateAction.DoAction();
                newStateAction.SimulateAction();

                //get current ub
                BattlefieldManager.ManagerInstance.CurrentStateOfGame.GetNextUnit();

                //ActionManager.Instance.CurrentlySelectedPlayingUnit = ub;
                AIAgentInstanceAgent.CurrentlyControledUnit = ActionManager.Instance.CurrentlySelectedPlayingUnit;

                BattlefieldManager.ManagerInstance.ResetTilesInRange();
                BattlefieldManager.ManagerInstance.StartingHexBehaviorTile = ActionManager.Instance.CurrentlySelectedPlayingUnit.CurrentHexTile;
                BattlefieldManager.ManagerInstance.SelectTilesInRangeSimple(ActionManager.Instance.CurrentlySelectedPlayingUnit.movementRange);

                //ovde ide score-ovanje
                List<IAction> availableActions = AIAgentInstanceAgent.GetAvailableActions().OrderByDescending(x => x.ScoredValue).Take(3).ToList();

                Minimax2(depth + 1, false, currentPlayerID, availableActions,alpha, beta, st);

                decimal minimaxedActionValue = availableActions.Max(x => x.SimulatedValue);
                
                bestValue = bestValue <= minimaxedActionValue ? minimaxedActionValue : bestValue;
                alpha = Math.Max(alpha, bestValue);

                action.SimulatedValue = bestValue;
                BattlefieldManager.ManagerInstance.ReturnToPreviousState();
                // Alpha Beta Pruning
                if (beta <= alpha)
                    break;
            }

            //st += "depth " + depth + ":";
            //foreach (IAction ac in actions)
            //{
            //    st += ac.ToString() + "_" + ac.SimulatedValue;
            //}
            return ;
        }
        else
        {
            decimal bestValue = MAX;

            // Recur for left and
            // right children
            foreach (IAction action in actions)
            {
                BattlefieldManager.ManagerInstance.CreateNewAndChangeCurrentGameStat();
                BattlefieldManager.ManagerInstance.CurrentStateOfGame.actionToDoInState = action;

                //create new action based on action in old game state
                IAction newStateAction = CreateActionForNewState(action);
                //newStateAction.DoAction();
                newStateAction.SimulateAction();

                //get current ub
                BattlefieldManager.ManagerInstance.CurrentStateOfGame.GetNextUnit();

                //ActionManager.Instance.CurrentlySelectedPlayingUnit = ub;
                AIAgentInstanceAgent.CurrentlyControledUnit = ActionManager.Instance.CurrentlySelectedPlayingUnit;

                BattlefieldManager.ManagerInstance.ResetTilesInRange();
                BattlefieldManager.ManagerInstance.StartingHexBehaviorTile = ActionManager.Instance.CurrentlySelectedPlayingUnit.CurrentHexTile;
                BattlefieldManager.ManagerInstance.SelectTilesInRangeSimple(ActionManager.Instance.CurrentlySelectedPlayingUnit.movementRange);

                //ovde ide score-ovanje
                List<IAction> availableActions = AIAgentInstanceAgent.GetAvailableActions().OrderByDescending(x => x.ScoredValue).Take(3).ToList();
                Minimax2(depth + 1, true, currentPlayerID, availableActions, alpha, beta, st);

                decimal minimaxedActionValue = availableActions.Min(x => x.SimulatedValue);

                //int val = minimaxedAction.SimulatedValue;
                bestValue = bestValue < minimaxedActionValue ? bestValue : minimaxedActionValue;/*new EmptySimulatedAction(Math.Min(best.SimulatedValue, val));*/
                beta = Math.Min(beta, bestValue);

                action.SimulatedValue = bestValue;
                BattlefieldManager.ManagerInstance.ReturnToPreviousState();
                // Alpha Beta Pruning
                if (beta <= alpha)
                    break;
            }

            //st += "depth " + depth + ":";
            //foreach (IAction ac in actions)
            //{
            //    st += ac.ToString() + "_"+ac.SimulatedValue;
            //}
            return;
        }
    }

    private static IAction CreateActionForNewState(IAction action)
    {
        GameObject ownerGOInCurrentState =
            BattlefieldManager.ManagerInstance.CurrentStateOfGame.InstantiatedUnits.First(x =>
                x.GetComponent<UnitBehaviour>().UniqueUnitId == action.ActionOwner.UniqueUnitId);
        UnitBehaviour ownerUnitBehaviour = ownerGOInCurrentState.GetComponent<UnitBehaviour>();

        HexBehaviour targetHexUnitBehaviour =
            BattlefieldManager.ManagerInstance.GeTileBehaviourFromPoint(action.ChosenTargetHex.OwningTile.Location);

        BattlefieldManager.ManagerInstance.StartingHexBehaviorTile = ownerUnitBehaviour.CurrentHexTile;
        
        if (action.ActionType == ActionType.Attack)
        {
            return  new AttackUnitOnHexAction(ownerUnitBehaviour,targetHexUnitBehaviour);
        }
        else if (action.ActionType == ActionType.Move)
        {
            return new MoveToHexAction(ownerUnitBehaviour,targetHexUnitBehaviour);
        }
        else
        {
            return new EmptySimulatedAction();
        }
    }

    #endregion Minimax2

}
