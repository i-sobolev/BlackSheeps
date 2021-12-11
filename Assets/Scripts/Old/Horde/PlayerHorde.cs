using UnityEngine;

public class PlayerHorde : MonoBehaviour
{
    public static Horde Horde { private set; get; }

    private void Awake()
    {
        Horde = GetComponent<Horde>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Horde.Attack();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            Horde.StopAttacking();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Horde.Interact();
        }
    }
}
