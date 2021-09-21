using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Scripting;

namespace Assets.Scripts
{
    public class StateOfGame : MonoBehaviour
    {
        #region Fields
        //Tile and Tile behaviour
        public HexTile SelectedTile = null;
        public HexBehaviour StartingHexBehaviorTile = null;
        public HexBehaviour DestinationTile = null;

        ////List of units to be placed on the board, not instantiated
        //public GameObject[] UnitsToPlace;
        ////List Of BattlefieldObjects
        //public GameObject BattlefieldObjectToPlace;
        public int currentPlayerId;
        public List<GameObject> InstantiatedUnits;

        public List<GameObject> InstantiatedBattlefieldObjects;

        //Board
        public Dictionary<Point, HexBehaviour> Board;

        //for minimax
        public UnitBehaviour previouslyControlledUnit;
        public int? indexOfPreviousState = null;
        public int IndexOfState = 0;
        public IAction actionToDoInState = null;
        #endregion Fields

        #region Methods

        public StateOfGame CreateCopyOfTheState(IAction action = null)
        {
            StateOfGame newStateOfGame = new StateOfGame();

            newStateOfGame.Board = new Dictionary<Point, HexBehaviour>();//new Dictionary<Point, HexBehaviour>(this.Board);

            newStateOfGame.indexOfPreviousState = this.IndexOfState;
            newStateOfGame.IndexOfState = this.IndexOfState + 1;

            //from old board make copy of a board
            foreach (Point p in Board.Keys)
            {
                GameObject gameObjectHex = Instantiate(Board[p].gameObject);

                HexBehaviour hb = gameObjectHex.GetComponent<HexBehaviour>();
                hb.StateIndex = newStateOfGame.IndexOfState;//indexOfPreviousState==null? 0: indexOfPreviousState.Value+ 1;

                hb.OwningTile = CreateNewHexTile(Board[p].OwningTile);
                //gameObjectHex.tag = "ToDelete";
                gameObjectHex.SetActive(false);

                newStateOfGame.Board.Add(p, hb);
            }

            /*should redo the neighborugs, and set them to copy tiles*/

            newStateOfGame.InstantiatedBattlefieldObjects = new List<GameObject>(this.InstantiatedBattlefieldObjects);
            newStateOfGame.InstantiatedUnits = new List<GameObject>();//List<GameObject>(this.InstantiatedUnits);

            UnitBehaviour actionManagerCurrentUnit = ActionManager.Instance.CurrentlySelectedPlayingUnit;
            foreach (GameObject g in this.InstantiatedUnits)
            {
                GameObject gameObject = (GameObject)Instantiate(g);
                gameObject.SetActive(false);
                gameObject.tag = "UnitTempCopy";

                UnitBehaviour old = g.GetComponent<UnitBehaviour>();
                string coords = old.CurrentHexTile.coordinates;

                int indexOfOpenBracket = 0;
                int indexOfComma = coords.IndexOf(",");
                int indexOfClosedBracket = coords.IndexOf(")");
                int coordX = Int32.Parse(coords.Substring(indexOfOpenBracket+1, indexOfComma-1));
                int coordY = Int32.Parse(coords.Substring(indexOfComma+1, indexOfClosedBracket - indexOfComma-1));

                Point point = new Point(coordX,coordY);       


                //unitBehaviour
                UnitBehaviour newUnitBehaviour = gameObject.GetComponent<UnitBehaviour>();
                
                newUnitBehaviour.InitializeAttackComponent();
                newUnitBehaviour.InitializeMovementComponent();

                newUnitBehaviour.CurrentHexTile = newStateOfGame.Board[point];
                newStateOfGame.Board[point].ObjectOnHex = newUnitBehaviour;

                newUnitBehaviour.currentStateIndex = newStateOfGame.IndexOfState;//indexOfPreviousState == null ? 0 : indexOfPreviousState.Value + 1;

                //newUnitBehaviour.animator = old.GetComponent<Animator>();
                //newUnitBehaviour.unitUI = old.GetComponent<UnitUI>();

                if (actionManagerCurrentUnit.UniqueUnitId == newUnitBehaviour.UniqueUnitId)
                {
                    newStateOfGame.previouslyControlledUnit = actionManagerCurrentUnit;
                    ActionManager.Instance.CurrentlySelectedPlayingUnit = newUnitBehaviour;
                    AIAgent.AIAgentInstanceAgent.CurrentlyControledUnit = newUnitBehaviour;
                }

                newStateOfGame.InstantiatedUnits.Add(gameObject);
            }

            foreach (HexBehaviour behaviour in newStateOfGame.Board.Values)
                behaviour.OwningTile.FindNeighbours(newStateOfGame.Board, new Vector2(10, 10));
            /*-----I should recheck unit on fields and reset them----*/
            //foreach (GameObject unit in newStateOfGame.InstantiatedUnits)
            //{
            //    UnitBehaviour ub = unit.GetComponent<UnitBehaviour>();
            //    Point newPoint = ub.CurrentHexTile.coordinates
            //}

            

            if (action!=null)
            {
                actionToDoInState = action;
            }

            return newStateOfGame;
        }

        private HexTile CreateNewHexTile(HexTile owningTile)
        {
            HexTile newHexTile = new HexTile(owningTile.X,owningTile.Y);
            //newHexTile.AllNeighbours = owningTile.AllNeighbours;
            newHexTile.Cover = owningTile.Cover;
            newHexTile.Hazadours = owningTile.Hazadours;
            newHexTile.HighGround = owningTile.HighGround;
            newHexTile.IsInRange = owningTile.IsInRange;
            newHexTile.Occupied = owningTile.Occupied;
            newHexTile.Passable = owningTile.Passable;
            newHexTile.Location = owningTile.Location;


            return newHexTile;
        }

        public decimal GetHealthScoreOfTheStateForPlayer(int playerId)
        {
            return this.InstantiatedUnits.Where(x => x.GetComponent<UnitBehaviour>().PlayerId == playerId)
                .Sum(x => x.GetComponent<UnitBehaviour>().CurrentHealth);
        }

        internal void Clear()
        {
            foreach (Point key in this.Board.Keys)
            {
                Destroy(this.Board[key].gameObject);
            }
            this.Board.Clear();

            //foreach (GameObject gameObject in this.InstantiatedBattlefieldObjects)
            //{
            //    GameObject.Destroy(gameObject);
            //}
            //foreach (GameObject battlefieldObject in InstantiatedBattlefieldObjects)
            //{
            //    Destroy(battlefieldObject);
                
            //}

            this.InstantiatedBattlefieldObjects.Clear();
            //foreach (GameObject gameObject in this.InstantiatedUnits)
            //{
            //    GameObject.Destroy(gameObject);
            //}

            foreach (GameObject unit in InstantiatedUnits)
            {
                Destroy(unit);
            }

            this.InstantiatedUnits.Clear();
        }

        internal void GetNextUnit()
        {
            UnitBehaviour prevUnitBehaviour = ActionManager.Instance.CurrentlySelectedPlayingUnit;
            int newPlayerId = Math.Abs(prevUnitBehaviour.PlayerId - 1);

            int indexOfPrevUnit = InstantiatedUnits.IndexOf(prevUnitBehaviour.gameObject);
            int i = indexOfPrevUnit == InstantiatedUnits.Count - 1 ? 0 : indexOfPrevUnit + 1;

            UnitBehaviour ubIterator;
            while (true)
            {
                ubIterator = InstantiatedUnits[i].GetComponent<UnitBehaviour>();

                if (ubIterator.PlayerId == newPlayerId && ubIterator.isAlive)
                {
                    ActionManager.Instance.CurrentlySelectedPlayingUnit = ubIterator;
                    return;
                }

                if (i == InstantiatedUnits.Count -1)
                {
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            //int i = InstantiatedUnits.IndexOf(InstantiatedUnits.First(x =>
            //    x.GetComponent<UnitBehaviour>() == prevUnitBehaviour));
            //for(;i<InstantiatedUnits.Count;i++)
            //{
            //    if(InstantiatedUnits[i].GetComponent<UnitBehaviour>().)
            //}

        }

        #endregion Methods
    }
}
