using BlackSheeps;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class Rocket : InteractionEntity
{
    public List<RocketPart> Parts;

    public UnityEvent OnRocketCollected;

    public override void Interact(UnitOld interactedUnit)
    {
        base.Interact(interactedUnit);

        Parts[interactedUnit.LiftedSparePart.RocketPartId].EnablePart();
        Parts.ForEach(part => part.CheckRecuirePart());

        interactedUnit.UseSparePart();

        if (Parts.TrueForAll(obj => obj.IsEnabled))
        {
            Debug.Log("RocketCollected!");
            OnRocketCollected?.Invoke();
        }
    }
}