using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UnitComponents.Attack;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.AIComponent
{
    public class SimulatedState
    {
        //SimulatedUnit CurrentluPlayingSimUnit;
        public List<IAction> actionsToTake;
        public List<SimulatedUnit> BattlefieldState;
        public int Score;

        public IAction PreviousAction;

        //public SimulatedState(SimulatedUnit su, List<IAction> selectedAction, List<SimulatedUnit> bfs, int score)
        //{
        //    CurrentluPlayingSimUnit = su;
        //    actionsToTake = selectedAction;
        //    bfs = BattlefieldState;
        //    Score = score;
        //}

        public SimulatedState(List<IAction> selectedAction, List<SimulatedUnit> bfs, int score, IAction previousAction)
        {
            actionsToTake = selectedAction;
            BattlefieldState = bfs;
            Score = score;
            PreviousAction = previousAction;
        }

        //needs to get a list of new states, so for each action it should return new score, new potential actions and new
        public List<SimulatedState> GetSimulatedStateAfterAction()
        {
            //creates copy of the simulated units
            //List<SimulatedUnit> newSimulatedUnits = new List<SimulatedUnit>(BattlefieldState);
            List<SimulatedState> newSimulatedStates = new List<SimulatedState>();
            int newScore = Score;
            SimulatedUnit currentSimulatedUnit = BattlefieldState.First(x => x.currentlyPlaying);

            int indexOfCurrentPlayingUnit = BattlefieldState.IndexOf(currentSimulatedUnit);
            //simulate each action and generate score, and change simulated units accordingly
            foreach (IAction action in actionsToTake)
            {
                //create copy of previous state, then change according to action
                List<SimulatedUnit> newSimulatedUnits = new List<SimulatedUnit>(BattlefieldState);

                switch (action.ActionType)
                {
                    //I should maybe create on action simulate method, with currSimulatedunit, and battlefieldState list as parametres 
                    case ActionType.Attack:
                        //not checking real battlefield but the simulation
                        UnitBehaviour targetOfAttack = ((SimulatedUnit)BattlefieldState.First(x => x.SimulatedHexBehaviour.Equals(action.ChosenTargetHex))).UnitBehaviour;
                        SimulatedUnit simulatedTarget =
                            BattlefieldState.First(x => x.UnitBehaviour.Equals(targetOfAttack));

                        //create copy of the battlefieldstate, sim unit list, find curr and target and change their simulatedHealth
                        

                        int currDamage = 0 ;
                        int retaliationDamage = 0;

                        if (currentSimulatedUnit.SimulatedHexBehaviour.OwningTile.AllNeighbours.Contains(action.ChosenTargetHex.OwningTile))
                        {
                            currDamage = currentSimulatedUnit.UnitBehaviour.Damage;
                            retaliationDamage = (targetOfAttack.Damage / 2);
                            //also checking if, in simulation, target would perish. if yes bonus point
                            newScore += currDamage - retaliationDamage +
                                        (simulatedTarget.SimulatedHealth - currDamage <= 0 ? 1 : 0) +
                                        (currentSimulatedUnit.SimulatedHealth - retaliationDamage <= 0 ? -1 : 0);
                        }
                        else if (currentSimulatedUnit.UnitBehaviour.AttackType == AttackType.Melee)
                        {
                            currDamage = currentSimulatedUnit.UnitBehaviour.Damage;
                            retaliationDamage = (targetOfAttack.Damage / 2);
                            //also checking if, in simulation, target would perish. if yes bonus point
                            newScore += currDamage - retaliationDamage +
                                        (simulatedTarget.SimulatedHealth - currDamage <= 0 ? 1 : 0) +
                                        (currentSimulatedUnit.SimulatedHealth - retaliationDamage <= 0 ? -1 : 0);
                            
                            //update changed hex, but i need to check if they were not in the neighbouring tiles
                            Path<HexTile> pathAttack = Pathfinder.FindPath(currentSimulatedUnit.SimulatedHexBehaviour.OwningTile, action.ChosenTargetHex.OwningTile);

                            //Debug.Log(currentSimulatedUnit.SimulatedHexBehaviour.OwningTile.ToString()+" path to" +action.ChosenTargetHex.OwningTile.ToString());

                            if (currentSimulatedUnit.SimulatedHexBehaviour.OwningTile == action.ChosenTargetHex.OwningTile || pathAttack ==null)
                            {
                                string pathNull = "Path is null: " + (pathAttack == null ? "Yes" : "No");
                                Debug.Log(pathNull+" for action: --------------------");
                                action.Print();
                                Debug.Log(" --------------------");

                                continue;
                            }

                            //if (expr)
                            //{
                                
                            //}
                            

                            //+ "\n pathAttack.PreviousSteps is null" + pathAttack.PreviousSteps == null ? "Yes" : "No"));
                           // Debug.Log("Path.LastSteps is null: " + pathAttack.LastStep == null ? "Yes" : "No");//
                           // Debug.Log("Path.PreviousSteps is null: " + pathAttack.PreviousSteps == null ? "Yes" : "No");//

                            newSimulatedUnits[newSimulatedUnits.IndexOf(currentSimulatedUnit)].SimulatedHexBehaviour = pathAttack.PreviousSteps.LastStep.GetHexBehaviour();
                            
                        }
                        else if (currentSimulatedUnit.UnitBehaviour.AttackType == AttackType.Ranged)
                        {
                            RangedAttack ra = (RangedAttack) currentSimulatedUnit.UnitBehaviour.AttackComponent;
                            //simulate for ranged ignore cover for now
                            currDamage = (int)(ra.CalculateDamageBonuses(currentSimulatedUnit.SimulatedHexBehaviour) * ra.CalculateDamageModifiers(action.ChosenTargetHex,currentSimulatedUnit.SimulatedHexBehaviour));

                            newScore += currDamage +
                                        (simulatedTarget.SimulatedHealth - currDamage <= 0 ? 1 : 0);
                            
                        }

                        newSimulatedUnits[newSimulatedUnits.IndexOf(currentSimulatedUnit)].SimulatedHealth =
                            currentSimulatedUnit.SimulatedHealth - retaliationDamage;

                        newSimulatedUnits[newSimulatedUnits.IndexOf(simulatedTarget)].SimulatedHealth =
                            simulatedTarget.SimulatedHealth - currDamage;

                        //adding new states 
                        //but first determine who is next playing unit

                        //newSimulatedStates.Add(new SimulatedState());
                        break;
                    case ActionType.Move:
                        //just change current unit simulated hex
                        //Path<HexTile> pathMove = Pathfinder.FindPath(currentSimulatedUnit.SimulatedHexBehaviour.OwningTile, action.ChosenTargetHex.OwningTile);
                        //Debug.Log(""+ newSimulatedUnits[indexOfCurrentPlayingUnit].ToString());
                        //Debug.Log("" + currentSimulatedUnit.SimulatedHexBehaviour.OwningTile.ToString()+" vs "+ action.ChosenTargetHex.OwningTile.ToString());
                        //Debug.Log("" + pathMove == null ? "path is null" : "path ok");
                        //Debug.Log("" + pathMove.LastStep ==null?"last step null": pathMove.LastStep.ToString());
                        newSimulatedUnits[indexOfCurrentPlayingUnit].SimulatedHexBehaviour = action.ChosenTargetHex;//pathMove.LastStep.GetHexBehaviour();

                        newScore += 1 + (int)math.ceil(action.GetScore());
                        break;
                }

                action.SimulatedValue = newScore;

                //newSimulatedUnits = newSimulatedUnits.Where(x => x.SimulatedHealth > 0).ToList();
                int countOfLiving = newSimulatedUnits.Where(x => x.SimulatedHealth > 0).ToList().Count;

                if (countOfLiving<=1)
                {
                    continue;
                }

                //get next currPlayingUnit
                SetNextPlayingUnit(indexOfCurrentPlayingUnit, newSimulatedUnits);

                //get actions for next currPlayingUnit
               List<IAction> newActions = GetActionsForNextUnit(newSimulatedUnits).OrderByDescending(x=>x.SimulatedConsidValue).Take(4).ToList();

               newSimulatedStates.Add(new SimulatedState(newActions, newSimulatedUnits, newScore,action));
            }

            return newSimulatedStates;
        }

        private List<IAction> GetActionsForNextUnit(List<SimulatedUnit> newSimulatedUnits)
        {
            List<IAction> newActions = new List<IAction>();
            SimulatedUnit currPlaying = newSimulatedUnits.First(x => x.currentlyPlaying);
            List<HexBehaviour> enemyHexes = newSimulatedUnits
                .Where(x => x.UnitBehaviour.PlayerId != currPlaying.UnitBehaviour.PlayerId)
                .Select(x => x.SimulatedHexBehaviour).ToList();

            newActions.AddRange(GetMoveActions(enemyHexes, currPlaying));
            newActions.AddRange(GetAttackActions(newSimulatedUnits,enemyHexes, currPlaying));

            return newActions;
        }

        private List<IAction> GetMoveActions(List<HexBehaviour> enemyHexes,SimulatedUnit su)
        {
            List<HexBehaviour> HexesInRange = BattlefieldManager.ManagerInstance.GetTilesInRange(
                su.SimulatedHexBehaviour.UnitAnchorWorldPositionVector, su.UnitBehaviour.movementRange).Where(x => x.OwningTile.Passable && !x.Equals(su.SimulatedHexBehaviour)).ToList();

            List<HexBehaviour> freeHexesInRange = HexesInRange.Except(enemyHexes).ToList();

            List<IAction> moveActions = new List<IAction>();

            MoveToHexAction iterator;
            foreach (HexBehaviour hex in freeHexesInRange)
            {
                iterator = new MoveToHexAction(su.UnitBehaviour, hex);
                iterator.SimulatedConsidValue = iterator.Simulate(su, new SimulatedUnit(null, hex, 0, false));
                moveActions.Add(iterator);
            }

            return moveActions;
        }

        private List<IAction> GetAttackActions(List<SimulatedUnit> newSimulatedUnits, List<HexBehaviour> enemyHexes, SimulatedUnit su)
        {
            List<HexBehaviour> HexesInAttackRange = BattlefieldManager.ManagerInstance.GetTilesInRange(su.SimulatedHexBehaviour.UnitAnchorWorldPositionVector,su.UnitBehaviour.attackRange);
            List<HexBehaviour> validHexesForAttack = HexesInAttackRange.Intersect(enemyHexes).ToList();
            List<IAction> attackActions = new List<IAction>();

            AttackUnitOnHexAction iterator;
            foreach (HexBehaviour hex in validHexesForAttack)
            {
                iterator = new AttackUnitOnHexAction(su.UnitBehaviour, hex);
                SimulatedUnit enemy = newSimulatedUnits.Find(x => x.SimulatedHexBehaviour == hex);
                iterator.SimulatedConsidValue = iterator.Simulate(su, enemy);
                attackActions.Add(iterator);
            }

            return attackActions;
        }

        private void SetNextPlayingUnit(int indexOfCurrentPlayingUnit, List<SimulatedUnit> newSimulatedUnits)
        {
            newSimulatedUnits[indexOfCurrentPlayingUnit].currentlyPlaying = false;
            bool foundNextUnit = false;
            int iterator = indexOfCurrentPlayingUnit;
            while (!foundNextUnit)
            {
                if (iterator + 1 >= newSimulatedUnits.Count)
                    iterator = 0;
                else
                    iterator++;

                if (newSimulatedUnits[iterator].SimulatedHealth>0)
                {
                    newSimulatedUnits[iterator].currentlyPlaying = true;
                    foundNextUnit = true;
                }
            }
        }
    }
}
