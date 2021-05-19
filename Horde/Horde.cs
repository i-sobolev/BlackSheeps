using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Horde : MonoBehaviour
{
    public UnitTypes.UnitType UnitsType;

    public List<Unit> FriendUnits;
    public List<Unit> EnemyUnits;
    public List<InteractionEntity> InteractionEntities;
    public AnimationCurve StoppingDistanceCurve;
    private int NumberOfFriendUnits = 0;

    public AnimationCurve AreaSizeCurve;
    private CircleCollider2D _hordeArea;
    public bool isAreaSizeConstant;

    public UnityEvent OnHordeDie;
    public UnityEvent OnUnitDie;
    public UnityEvent OnUnitJoined;

    private void Awake()
    {
        _hordeArea = GetComponent<CircleCollider2D>();
        SetAreaSize();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
    private void SetAreaSize()
    {
        if (isAreaSizeConstant)
            return;

        _hordeArea.radius = AreaSizeCurve.Evaluate(NumberOfFriendUnits);
    }
    private void Update()
    {
        FriendUnits.ForEach(unit => unit.SetMoveTarget(transform)); // SHIT FIX IT PLS C MON MAAAAN LOL KEKM SHPEKM
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Unit>())
        {
            var unitInTrigger = collision.GetComponent<Unit>();

            if (unitInTrigger.UnitType == UnitsType)
            {
                AddNewUnit(unitInTrigger);
            }

            else
                EnemyUnits.Add(unitInTrigger);
        }

        if (collision.GetComponent<InteractionEntity>())
        {
            var interactionEntityInTrigger = collision.GetComponent<InteractionEntity>();

            if (!InteractionEntities.Contains(interactionEntityInTrigger))
                InteractionEntities.Add(interactionEntityInTrigger);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<InteractionEntity>())
            InteractionEntities.Remove(collision.GetComponent<InteractionEntity>());

        if (collision.GetComponent<Unit>())
        {
            var unitInTrigger = collision.GetComponent<Unit>();

            if (unitInTrigger.UnitType != UnitsType)
                EnemyUnits.Remove(unitInTrigger);
        }
    }

    public void AddNewUnit(Unit newUnit)
    {
        if (newUnit.LinkedHorde != null)
            return;

        FriendUnits.Add(newUnit);
        NumberOfFriendUnits++;

        FriendUnits.ForEach(unit => unit.SetPassiveStoppingDistance(StoppingDistanceCurve.Evaluate(NumberOfFriendUnits)));
        newUnit.LinkedHorde = this;

        OnUnitJoined?.Invoke();

        SetAreaSize();
    }

    public void Attack()
    {
        if (EnemyUnits.Count <= 0 || FriendUnits.Count <= 0)
            return;

        foreach (var friendUnit in FriendUnits)
        {
            if (friendUnit.CurrentBehaviour == Unit.Behaviour.Passive)
            {
                //var closestEnemyUnit = EnemyUnits[0];
                //float minDistance = Vector2.Distance(friendUnit.transform.position, closestEnemyUnit.transform.position);

                Unit closestEnemyUnit = null;
                float minDistance = Mathf.Infinity;

                foreach (var enemyUnit in EnemyUnits)
                {
                    if (enemyUnit.CurrentBehaviour != Unit.Behaviour.Dead)
                    {
                        if (friendUnit.CurrentBehaviour == Unit.Behaviour.Passive)
                        {
                            float newDistance = Vector2.Distance(friendUnit.transform.position, enemyUnit.transform.position);

                            if (newDistance < minDistance)
                            {
                                minDistance = newDistance;
                                closestEnemyUnit = enemyUnit;
                            }
                        }
                    }
                }

                //if (closestEnemyUnit != null || closestEnemyUnit.CurrentBehaviour != Unit.Behaviour.Dead)
                if (closestEnemyUnit != null || closestEnemyUnit.CurrentBehaviour != Unit.Behaviour.Dead)
                    friendUnit.Attack(closestEnemyUnit);
            }
        }
    }

    public void StopAttacking()
    {
        if (FriendUnits.Count > 0)
        {
            foreach (var unit in FriendUnits)
                unit.StopAttacking();
        }
    }

    public void Interact()
    {
        ChooseFriendUnitToInteract();
    }

    public void ChooseFriendUnitToInteract()
    {
        if (InteractionEntities.Count == 0 || FriendUnits.Count == 0)
            return;

        InteractionEntity closestInteractionEntity = InteractionEntities[0];
        float minDistance = Vector2.Distance(transform.position, closestInteractionEntity.transform.position);

        foreach (var entity in InteractionEntities)
        {
            if (entity == null)
            {
                InteractionEntities.Remove(entity);
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
            foreach (var unit in FriendUnits)
            {
                if (unit.CurrentBehaviour == Unit.Behaviour.Carries)
                {
                    unit.Interact(closestInteractionEntity);
                    return;
                }
            }

            return;
        }

        //Unit closestUnit = FriendUnits[0];
        //minDistance = Vector2.Distance(closestUnit.transform.position, closestInteractionEntity.transform.position);

        Unit closestUnit = null;
        minDistance = Mathf.Infinity;

        foreach (var unit in FriendUnits)
        {
            if (unit.CurrentBehaviour == Unit.Behaviour.Passive)
            {
                float newDistance = Vector2.Distance(unit.transform.position, closestInteractionEntity.transform.position);

                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    closestUnit = unit;
                }
            }
        }

        if (closestInteractionEntity != null && closestUnit != null)
            closestUnit.Interact(closestInteractionEntity);
    }

    public void DeleteFriendUnit(Unit unit)
    {
        FriendUnits.Remove(unit);
        NumberOfFriendUnits--;

        OnUnitDie?.Invoke();

        if (NumberOfFriendUnits <= 0)
            OnHordeDie?.Invoke();
    }
}
