using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    public UnitTypes.UnitType UnitType;

    private NavMeshAgent _agent;
    public Transform MoveTarget;

    public Horde LinkedHorde { get; set; }

    public enum Behaviour { Passive, Attack, Interact, Carries, Dead }
    public Behaviour CurrentBehaviour;

    private InteractionEntity _currentInteractionEntity = null;
    private Unit _currentEnemy = null;
    public SparePart LiftedSparePart { private set; get; }

    [SerializeField] private short Health = 500;
    [SerializeField] private short HitDamage = 1;
    public bool IsAttacking { private set; get; }

    private float _passiveStoppingDistance;
    private float _defaultStoppingDistance;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_currentInteractionEntity != null)
            Gizmos.DrawLine(transform.position, _currentInteractionEntity.GetIteractPoint());

        if (_currentEnemy != null)
            Gizmos.DrawLine(transform.position, _currentEnemy.transform.position);
    }
#endif

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _agent.SetDestination(transform.position);

        _defaultStoppingDistance = _agent.stoppingDistance;
        _passiveStoppingDistance = _agent.stoppingDistance;

        CurrentBehaviour = Behaviour.Passive;
    }

    private void Update()
    {
        BehaviourUpdate();
    }

    public bool IsAlive() => Health > 0;

    public void SetPassiveStoppingDistance(float newDistance) => _passiveStoppingDistance = newDistance;

    public void ApplyDamage(short Damage)
    {
        Health -= Damage;
        
        if (Health <= 0)
            Die();
    }

    private void Die()
    {
        SetMainComponentsEnableState(false);

        LinkedHorde.DeleteFriendUnit(this);

        _currentEnemy = null;
        CurrentBehaviour = Behaviour.Dead;

        DropSparePart();
    }

    public void SetMoveTarget(Transform target) => MoveTarget = target;
    public void SetMoveTarget(Vector3 target) => _agent.SetDestination(target);

    public void Interact(InteractionEntity interactionEntity)
    {
        if (!(interactionEntity is Rocket))
            DropSparePart();

        CurrentBehaviour = Behaviour.Interact;
        _currentInteractionEntity = interactionEntity;
    }

    public void PickUpSparePart(SparePart sparePart)
    {
        CurrentBehaviour = Behaviour.Carries;
        LiftedSparePart = sparePart;
        sparePart.IsLifted = true;

        sparePart.transform.position = new Vector2(10000, 10000);
    }

    public void DropSparePart()
    {
        if (LiftedSparePart == null)
            return;

        LiftedSparePart.IsLifted = false;
        LiftedSparePart.transform.position = transform.position;
        LiftedSparePart = null;
    }

    public void UseSparePart()
    {
        Destroy(LiftedSparePart);
        LiftedSparePart = null;
        CurrentBehaviour = Behaviour.Passive;
    }

    private void BehaviourUpdate()
    {
        switch(CurrentBehaviour)
        {
            case Behaviour.Passive:

                if (MoveTarget != null)
                {
                    _agent.stoppingDistance = _passiveStoppingDistance;
                    _agent.SetDestination(MoveTarget.position);
                }

                break;

            case Behaviour.Attack:

                _agent.stoppingDistance = _defaultStoppingDistance;

                Vector3 argetPosition = _currentEnemy.transform.position;
                _agent.SetDestination(argetPosition);

                if (CheckDistance(argetPosition, 1.5f))
                {
                    if (_currentEnemy.IsAlive())
                    {
                        IsAttacking = true;
                        _currentEnemy.ApplyDamage(HitDamage);
                    }

                    else
                    {
                        IsAttacking = false;
                        _currentEnemy = null;
                        CurrentBehaviour = Behaviour.Passive;
                    }
                }

                break;

            case Behaviour.Interact:

                _agent.stoppingDistance = _defaultStoppingDistance;

                Vector3 entityPosition = _currentInteractionEntity.GetIteractPoint();
                _agent.SetDestination(entityPosition);

                if (_currentInteractionEntity is SparePart)
                {
                    var sparePart = _currentInteractionEntity as SparePart;
                    
                    if (sparePart.IsLifted)
                    {
                        _currentInteractionEntity = null;
                        CurrentBehaviour = Behaviour.Passive;
                    }
                }

                if (CheckDistance(entityPosition, _currentInteractionEntity.DistanceToInteract))
                {
                    _currentInteractionEntity.Interact(this);
                    LinkedHorde.InteractionEntities.Remove(_currentInteractionEntity);

                    if (!(_currentInteractionEntity is SparePart))
                        CurrentBehaviour = Behaviour.Passive;
                    
                    _currentInteractionEntity = null;
                }

                break;

            case Behaviour.Carries:

                if (MoveTarget != null)
                {
                    _agent.stoppingDistance = _passiveStoppingDistance;
                    _agent.SetDestination(MoveTarget.position);
                }

                break;
        }
    }

    public void Attack(Unit enemy)
    {
        DropSparePart();

        _currentEnemy = enemy;
        CurrentBehaviour = Behaviour.Attack;
    }

    public void StopAttacking()
    {
        if (CurrentBehaviour == Behaviour.Attack)
        {
            IsAttacking = false;
            _currentEnemy = null;
            CurrentBehaviour = Behaviour.Passive;
        }
    }

    public void SetMainComponentsEnableState(bool isEnabled)
    {
        _agent.enabled = isEnabled;
        GetComponent<CircleCollider2D>().enabled = isEnabled;
    }

    private bool CheckDistance(Vector3 target, float minDistance) => Vector3.Distance(transform.position, target) <= minDistance;
}