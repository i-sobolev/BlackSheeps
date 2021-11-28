using System.Collections.Generic;
using UnityEngine;

public class RocketPart : MonoBehaviour
{
    public List<GameObject> MainParts;
    public List<GameObject> AdditiveParts;

    public RocketPart RecuiredPart;

    public bool IsEnabled { private set; get; }

    private void Start()
    {
        IsEnabled = false;
        MainParts.ForEach(obj => obj.SetActive(false));
        AdditiveParts.ForEach(obj => obj.SetActive(false));
    }

    public void EnablePart()
    {
        IsEnabled = true;
        MainParts.ForEach(obj => obj.SetActive(true));

        CheckRecuirePart();
    }

    public void CheckRecuirePart()
    {
        if (RecuiredPart == null)
            return;

        if (RecuiredPart.IsEnabled)
            AdditiveParts.ForEach(obj => obj.SetActive(true));
    }
}
