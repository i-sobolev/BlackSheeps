using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject UnitPrefab;

    public Vector2 StartSpawnPoint;
    public Vector2 EndSpawnPoint;

    public int NumberOfUnits;

    public float SpawnDelay = 1;
    public float AnimationDuration = 1;

    private void Awake()
    {
        StartSpawnPoint += (Vector2)transform.position;
        EndSpawnPoint += (Vector2)transform.position;
    }

    public void SpawnUnits()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        if (NumberOfUnits > 0)
        {
            NumberOfUnits--;

            var newUnit = Instantiate(UnitPrefab, StartSpawnPoint, Quaternion.identity);
            StartCoroutine(Animation(newUnit));

            yield return new WaitForSeconds(SpawnDelay);

            StartCoroutine(Spawn());
        }
    }

    private IEnumerator Animation(GameObject newUnit)
    {
        var sprite = newUnit.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0);

        var unitComponent = newUnit.GetComponent<Unit>();
        unitComponent.SetMoveTarget(EndSpawnPoint);

        float lerp = 0;

        while (lerp < 1)
        {
            lerp += Time.deltaTime / AnimationDuration;

            float curvedLerp = lerp.EaseInOutQuad();

            newUnit.transform.position = Vector2.Lerp(StartSpawnPoint, EndSpawnPoint, curvedLerp);
            sprite.color = new Color(1, 1, 1, curvedLerp);

            yield return null;
        }

        sprite.color = Color.white;
        newUnit.transform.position = EndSpawnPoint;

        //unitComponent.SetMainComponentsEnableState(true);

        yield return null;
    }
}