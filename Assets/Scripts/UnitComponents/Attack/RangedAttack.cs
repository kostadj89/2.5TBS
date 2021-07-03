using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.UnitComponents.Attack
{
    class RangedAttack : IAttackComponent
    {
        private bool IsEngagedInMelee;
        public UnitBehaviour ParentUnitBehaviour { get; set; }
        public ITakesDamage TargetOfAttack { get; set; }
        public int AttackRange
        {
            get { return 6; }

            set { }

        }

        public float DamageModifier { get; set; }

        public void InitializeComponent(UnitBehaviour unitBehaviour)
        {
            ParentUnitBehaviour = unitBehaviour;
        }

        public List<HexBehaviour> GetAttackableTiles()
        {
            Vector3 curentTileVector = ParentUnitBehaviour.CurrentHexTile.UnitAnchorWorldPositionVector;
            int currAttackRange = AttackRange;
            IsEngagedInMelee = false;

            foreach (HexTile tile in ParentUnitBehaviour.CurrentHexTile.OwningTile.AllNeighbours)
            {
                if (ParentUnitBehaviour.HexContainsAnEnemy(tile.GetHexBehaviour()))
                {
                    currAttackRange = 1;
                    IsEngagedInMelee = true;
                    Debug.Log("Enemy nearby, unit is engaged in melee and will now suffer retaliation strike!");
                    break;
                }
            }

            return BattlefieldManager.ManagerInstance.GetTilesInRange(curentTileVector, currAttackRange);
        }

        public void StartAttack(ITakesDamage target)
        {
            TargetOfAttack = target;
            List<HexBehaviour> attackableTiles = GetAttackableTiles();
            if (attackableTiles.Contains(((UnitBehaviour)TargetOfAttack).CurrentHexTile))
            {
                ParentUnitBehaviour.CurrentState = UnitState.Attacking;
                //we show targeted unit's ui, and damage it
                ((UnitBehaviour)TargetOfAttack).TakeDamage((int)(ParentUnitBehaviour.Damage * CalculateDamageModifiers()));

                //damage attacking unit with relation strike damage
                if (IsEngagedInMelee)
                {
                    ParentUnitBehaviour.TakeDamage((int)(((UnitBehaviour)TargetOfAttack).Damage * 0.5));
                }

                TargetOfAttack = null;

                // if i'm adding flanking it should be here
                //ParentUnitBehaviour.CurrentState = UnitState.Idle;
                ParentUnitBehaviour.SetIdleState();
                //ParentUnitBehaviour.CurrentHexTile.ChangeDestinationToThis();

                ActionManager.Instance.EndCurrentPlayingUnitTurn();
            }
        }

        public bool AttackConditionFufilled(HexBehaviour targetHexBehaviour)
        {
            return GetAttackableTiles().Contains(targetHexBehaviour) && ParentUnitBehaviour.HexContainsAnEnemy(targetHexBehaviour);
        }

        public float CalculateDamageModifiers()
        {
            float tempAttackModifier = 1;

            // if the attacker is engaged in melee we don't take cover reductions in consideration
            if (IsEngagedInMelee)
            {
                tempAttackModifier = 0.5f;
            }
            //... else we'll look all neighbouring fields of the target, and look for the occupied and fields with cover
            else
            {
                //()
                List<HexTile> coversTiles =
                    ((IIsOnHexGrid)TargetOfAttack).CurrentHexTile.OwningTile.AllNeighbours.Where(x =>
                       x.Occupied == true || x.Cover == true).ToList();

                tempAttackModifier = GetCoverModifier(((IIsOnHexGrid)TargetOfAttack).CurrentHexTile.OwningTile, ParentUnitBehaviour.CurrentHexTile.OwningTile, coversTiles);
            }

            return tempAttackModifier;
        }

        //private float GetCoverModifier(Vector3 target, Vector3 rangedAttacker, List<HexTile> coversTiles)
        //{
        //    RaycastHit hit;

        //    Vector3 direction = rangedAttacker - target;

        //    if (Physics.Raycast(target, direction, out hit))
        //    {
        //        Debug.Log("ray just hit the gameobject: " + hit.collider.gameObject.name);
        //        HexBehaviour hitTileHexBehaviour = hit.collider.gameObject.GetComponent<HexBehaviour>();
        //        HexTile hitTile = hitTileHexBehaviour.OwningTile;

        //        if (coversTiles.Contains(hitTile))
        //        {
        //            if (hitTile.Occupied)
        //            {
        //                return 0.5f;
        //            }

        //            if (hitTile.Cover)
        //            {
        //                return 1 - ((BattlefieldSpecialHex)hitTileHexBehaviour.ObjectOnHex).cover;
        //            }
        //        }
        //    }
        //    Debug.DrawRay(target, direction, Color.white);

        //    return 1;
        //}

        private float GetCoverModifier(HexTile target, HexTile rangedAttacker, List<HexTile> coversTiles)
        {
            List<HexTile> relevantCoverTiles = new List<HexTile>();
            float bestCover = 1;

            relevantCoverTiles = coversTiles.Where(x => x.HexTileBetweenTiles(target, rangedAttacker)).ToList();

            if (relevantCoverTiles.Count>0)
            {
                List<float> covers = new List<float>();
                foreach (HexTile hexTile in relevantCoverTiles)
                {
                    if (hexTile.Occupied)
                    {
                       covers.Add(0.3f); 
                    }
                    else
                    {
                        covers.Add(((BattlefieldSpecialHex)hexTile.GetHexBehaviour().ObjectOnHex).cover);
                    }
                }

                bestCover = covers.Max(c => c);
            }


            return bestCover;
        }
    }
}
