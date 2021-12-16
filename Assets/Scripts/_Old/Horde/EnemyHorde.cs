using UnityEngine;

public class EnemyHorde : MonoBehaviour
{
    private Horde _horde;

    private void Awake()
    {
        _horde = GetComponent<Horde>();
    }

    public void Update()
    {
        //if (_horde.EnemyUnits.Count > 0)
        //    _horde.Attack();

        //else
        //    _horde.StopAttacking();
    }
}
