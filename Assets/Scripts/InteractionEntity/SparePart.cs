public class SparePart : InteractionEntity
{
    public int RocketPartId;
    public bool IsLifted;

    public override void Interact(Unit interactedUnit)
    {
        base.Interact(interactedUnit);
        interactedUnit.PickUpSparePart(this);
    }
}
