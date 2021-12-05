using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractionEntity : MonoBehaviour
{
    public UnityEvent OnInteract;
    public float DistanceToInteract;
    public Vector2 InteractPoint;
    public bool DestroyAfterInteract;
    
    public virtual void Interact(Unit interactedUnit)
    {
        OnInteract?.Invoke();

        if (DestroyAfterInteract)
            StartCoroutine(DestroyAfterUse());
    }

    private IEnumerator DestroyAfterUse()
    {
        yield return new WaitForEndOfFrame();
        Destroy(this);
    }

    public Vector2 GetIteractPoint() => (Vector2)transform.position + InteractPoint;
}