using UnityEngine;

public class UnitAnimatorController : MonoBehaviour
{
    public UnitTypes.UnitType UnitType;

    private Unit _unit;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    

    private float Speed;
    private Vector2 _lastPosition;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _unit = GetComponent<Unit>();
        _animator = GetComponent<Animator>();
    
        _lastPosition = transform.position;
    }

    private void Update()
    {
        if (UnitType == UnitTypes.UnitType.Sheep)
        {
            if (_unit.LinkedHorde == null)
            {
                _animator.Play("NoHordeIdle");
                return;
            }
            
            _animator.SetBool("IsCarringBox", _unit.LiftedSparePart != null);
        }

        Speed = Vector2.Distance(transform.position, _lastPosition);
        float xDirection = _lastPosition.x - transform.position.x;
        
        _animator.SetFloat("Speed", Speed);
        _animator.SetBool("IsAttacking", _unit.IsAttacking);
        _animator.SetBool("IsAlive", _unit.IsAlive());

        float flipDelta = 0.05f;
        if (xDirection > flipDelta)
            _spriteRenderer.flipX = true;

        if (xDirection < -flipDelta)
            _spriteRenderer.flipX = false;
    }

    private void FixedUpdate()
    {
        _lastPosition = transform.position;
    }
}
