using System.Collections.Generic;
using UnityEngine;

public class UIHints : MonoBehaviour
{
    private Horde _playerHorde = null;

    public GameObject AttackHint;

    public GameObject InteractionEntityHint;
    private WorldHint _worldHint;

    public Vector2 WorldHintOffset;

    private void Awake()
    {
        _worldHint = InteractionEntityHint.AddComponent<WorldHint>();

        _worldHint.Offset = WorldHintOffset;
    }

    private void Start()
    {
        _playerHorde = PlayerHorde.Horde;
    }

    private void Update()
    {
        if (_playerHorde.EnemyUnits.Count > 0)
            ShowAttackHint();

        else
            HideAttackHint();

        if (_playerHorde.InteractionEntities.Count > 0)
        {
            RefreshClosestInteractionEntity();
            ShowWorldHint();
        }

        else
        {
            HideWorldHint();
        }
    }

    public void ShowAttackHint()
    {
        if (AttackHint.activeInHierarchy)
            return;

        AttackHint.SetActive(true);
    }

    public void HideAttackHint()
    {
        if (!AttackHint.activeInHierarchy)
            return;

        AttackHint.SetActive(false);
    }

    public void ShowWorldHint()
    {
        if (InteractionEntityHint.activeInHierarchy)
            return;

        InteractionEntityHint.SetActive(true);
    }

    public void HideWorldHint()
    {
        if (!InteractionEntityHint.activeInHierarchy)
            return;

        InteractionEntityHint.SetActive(false);
    }

    public void RefreshClosestInteractionEntity()
    {
        if (!(_playerHorde.InteractionEntities.Count > 0))
            return;

        var closest = _playerHorde.InteractionEntities[0];
        
        if (closest == null)
        {
            HideWorldHint();
            return;
        }
        
        var playerPosition = (Vector2)_playerHorde.transform.position;

        float minDistance = Vector2.Distance(playerPosition, closest.transform.position);

        _playerHorde.InteractionEntities.ForEach(ent => 
        {
            float newDistance = Vector2.Distance(playerPosition, ent.transform.position);

            if (newDistance < minDistance)
            {
                minDistance = newDistance;
                closest = ent;
            }
        });

        _worldHint.AttachedEntity = closest;
    }
}

public class WorldHint : MonoBehaviour
{
    public InteractionEntity AttachedEntity;
    private RectTransform _transform;
    private Camera _camera;
    public Vector2 Offset;

    private void Start()
    {
        _camera = CameraMover.PlayerCamera;
        _transform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (AttachedEntity != null)
            _transform.anchoredPosition = (Vector2)_camera.WorldToScreenPoint(AttachedEntity.GetIteractPoint()) + Offset;

        else
            gameObject.SetActive(false);
    }
}