using UnityEngine;

namespace BlackSheeps
{
    public class UnitOld : MonoBehaviour
    {
        [SerializeField] private UnitType _unitType;

        private InteractionEntity _currentInteractionEntity = null;
        private UnitOld _currentEnemy = null;

        [SerializeField] private int _health = 500;
        [SerializeField] private int _hitDamage = 1;

        public UnitType UnitType => _unitType;
        public UnitBehaviour CurrentBehaviour { get; private set; }
        public SparePart LiftedSparePart { private set; get; }
        public Horde LinkedHorde { get; set; }
        public bool IsAttacking { private set; get; }

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
            CurrentBehaviour = UnitBehaviour.Passive;
        }

        private void Update()
        {
            BehaviourUpdate();
        }

        public bool IsAlive() => _health > 0;

        public void ApplyDamage(short Damage)
        {
            _health -= Damage;

            if (_health <= 0)
                Die();
        }

        private void Die()
        {
            LinkedHorde.DeleteFriendUnit(this);

            _currentEnemy = null;
            CurrentBehaviour = UnitBehaviour.Dead;

            DropSparePart();
        }

        public void Interact(InteractionEntity interactionEntity)
        {
            if (!(interactionEntity is Rocket))
                DropSparePart();

            CurrentBehaviour = UnitBehaviour.Interact;
            _currentInteractionEntity = interactionEntity;
        }

        public void PickUpSparePart(SparePart sparePart)
        {
            CurrentBehaviour = UnitBehaviour.Carries;
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
            CurrentBehaviour = UnitBehaviour.Passive;
        }

        private void BehaviourUpdate()
        {
            switch (CurrentBehaviour)
            {
                case UnitBehaviour.Passive:

                    //if (MoveTarget != null)
                    //{
                    //    _agent.stoppingDistance = _passiveStoppingDistance;
                    //    _agent.SetDestination(MoveTarget.position);
                    //}

                    break;

                case UnitBehaviour.Attack:

                    //_agent.stoppingDistance = _defaultStoppingDistance;

                    Vector3 argetPosition = _currentEnemy.transform.position;
                    //_agent.SetDestination(argetPosition);

                    //if (CheckDistance(argetPosition, 1.5f))
                    //{
                    //    if (_currentEnemy.IsAlive())
                    //    {
                    //        IsAttacking = true;
                    //        _currentEnemy.ApplyDamage(HitDamage);
                    //    }

                    //    else
                    //    {
                    //        IsAttacking = false;
                    //        _currentEnemy = null;
                    //        CurrentBehaviour = Behaviour.Passive;
                    //    }
                    //}

                    break;

                case UnitBehaviour.Interact:

                    //_agent.stoppingDistance = _defaultStoppingDistance;

                    Vector3 entityPosition = _currentInteractionEntity.GetIteractPoint();
                    //_agent.SetDestination(entityPosition);

                    if (_currentInteractionEntity is SparePart)
                    {
                        var sparePart = _currentInteractionEntity as SparePart;

                        if (sparePart.IsLifted)
                        {
                            _currentInteractionEntity = null;
                            CurrentBehaviour = UnitBehaviour.Passive;
                        }
                    }

                    //if (CheckDistance(entityPosition, _currentInteractionEntity.DistanceToInteract))
                    //{
                    //    _currentInteractionEntity.Interact(this);
                    //    LinkedHorde.InteractionEntities.Remove(_currentInteractionEntity);

                    //    if (!(_currentInteractionEntity is SparePart))
                    //        CurrentBehaviour = Behaviour.Passive;

                    //    _currentInteractionEntity = null;
                    //}

                    break;

                case UnitBehaviour.Carries:

                    //if (MoveTarget != null)
                    //{
                    //    _agent.stoppingDistance = _passiveStoppingDistance;
                    //    _agent.SetDestination(MoveTarget.position);
                    //}

                    break;
            }
        }

        public void Attack(UnitOld enemy)
        {
            DropSparePart();

            _currentEnemy = enemy;
            CurrentBehaviour = UnitBehaviour.Attack;
        }

        public void StopAttacking()
        {
            if (CurrentBehaviour == UnitBehaviour.Attack)
            {
                IsAttacking = false;
                _currentEnemy = null;
                CurrentBehaviour = UnitBehaviour.Passive;
            }
        }
    }
}