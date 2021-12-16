using BlackSheeps;
using NavMeshGrid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Horde : GridArea
{
    public UnitType UnitsType;

    private List<Unit> _friendUnits = new List<Unit>();
    private List<Unit> _enemyUnits = new List<Unit>();
    private List<InteractionEntity> _interactionEntities = new List<InteractionEntity>();

    public override void OnAgentEnterArea(NavMeshGridAgent agent)
    {
        base.OnAgentEnterArea(agent);
        
        if (agent is Unit unit)
        {
            if (!_friendUnits.Contains(unit))
                AddNewUnit(unit);
            //if (unit.UnitType == UnitsType)
            //{
            //    AddNewUnit(unit);
            //}

            //else
            //    _enemyUnits.Add(unit);
        }

        //if (collision.GetComponent<InteractionEntity>())
        //{
        //    var interactionEntityInTrigger = collision.GetComponent<InteractionEntity>();

        //    if (!_interactionEntities.Contains(interactionEntityInTrigger))
        //        _interactionEntities.Add(interactionEntityInTrigger);
        //}
    }

    public override void OnAgentExitArea(NavMeshGridAgent agent)
    {
        base.OnAgentExitArea(agent);

        //if (collision.GetComponent<InteractionEntity>())
        //    _interactionEntities.Remove(collision.GetComponent<InteractionEntity>());

        //if (agent is Unit unit)
        //{
        //    //if (unitInTrigger.UnitType != UnitsType)
        //    //    _enemyUnits.Remove(unitInTrigger);
        //}
    }

    public void AddNewUnit(Unit newUnit)
    {
        //if (newUnit.LinkedHorde != null)
        //    return;

        _friendUnits.Add(newUnit);

        PositionChanged += () =>
        {
            newUnit.MoveTo(NodesInArea.First(node =>
            {
                return node.AgentOnNode == null && !_friendUnits.Exists(unit => unit.TargetNode == node);
            }));
        };

        //newUnit.LinkedHorde = this;
    }

    public void Attack()
    {
        //if (_enemyUnits.Count <= 0 || _friendUnits.Count <= 0)
        //    return;

        //foreach (var friendUnit in _friendUnits)
        //{
        //    if (friendUnit.CurrentBehaviour == UnitBehaviour.Passive)
        //    {
        //        UnitOld closestEnemyUnit = null;
        //        float minDistance = Mathf.Infinity;

        //        foreach (var enemyUnit in _enemyUnits)
        //        {
        //            if (enemyUnit.CurrentBehaviour != UnitBehaviour.Dead)
        //            {
        //                if (friendUnit.CurrentBehaviour == UnitBehaviour.Passive)
        //                {
        //                    float newDistance = Vector2.Distance(friendUnit.transform.position, enemyUnit.transform.position);

        //                    if (newDistance < minDistance)
        //                    {
        //                        minDistance = newDistance;
        //                        closestEnemyUnit = enemyUnit;
        //                    }
        //                }
        //            }
        //        }

        //        if (closestEnemyUnit != null || closestEnemyUnit.CurrentBehaviour != UnitBehaviour.Dead)
        //            friendUnit.Attack(closestEnemyUnit);
        //    }
        //}
    }

    public void StopAttacking()
    {
        //if (_friendUnits.Count > 0)
        //{
        //    foreach (var unit in _friendUnits)
        //        unit.StopAttacking();
        //}
    }

    public void Interact()
    {
        ChooseFriendUnitToInteract();
    }

    public void ChooseFriendUnitToInteract()
    {
        if (_interactionEntities.Count == 0 || _friendUnits.Count == 0)
            return;

        InteractionEntity closestInteractionEntity = _interactionEntities[0];
        float minDistance = Vector2.Distance(transform.position, closestInteractionEntity.transform.position);

        foreach (var entity in _interactionEntities)
        {
            if (entity == null)
            {
                _interactionEntities.Remove(entity);
            }

            else
            {
                float newDistance = Vector2.Distance(transform.position, entity.transform.position);

                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    closestInteractionEntity = entity;
                }
            }
        }

        if (closestInteractionEntity is Rocket)
        {
            foreach (var unit in _friendUnits)
            {
                //if (unit.CurrentBehaviour == UnitBehaviour.Carries)
                //{
                //    unit.Interact(closestInteractionEntity);
                //    return;
                //}
            }

            return;
        }

        UnitOld closestUnit = null;
        minDistance = Mathf.Infinity;

        foreach (var unit in _friendUnits)
        {
            //if (unit.CurrentBehaviour == UnitBehaviour.Passive)
            //{
            //    float newDistance = Vector2.Distance(unit.transform.position, closestInteractionEntity.transform.position);

            //    if (newDistance < minDistance)
            //    {
            //        minDistance = newDistance;
            //        closestUnit = unit;
            //    }
            //}
        }

        if (closestInteractionEntity != null && closestUnit != null)
            closestUnit.Interact(closestInteractionEntity);
    }

    public void DeleteFriendUnit(UnitOld unit)
    {
        //_friendUnits.Remove(unit);
    }
}
