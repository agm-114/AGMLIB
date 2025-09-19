using QFSW.QC.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public class EffectSpawner : MonoBehaviour
{
    public Collider spawnArea;
    public GameObject prefabToSpawn;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;
    public bool randomizeRotation = false;
    public int randomSeed = 12345;

    private System.Random rng;

    private void Start()
    {
        if(prefabToSpawn == null )
        {
            Debug.LogError("Effect Spawner missing prefab");
        }
        if(spawnArea ==  null)
        {
            Debug.LogError("Effect Spawner missing spawn area");
        }
        rng = new System.Random(randomSeed);
        Debug.LogError("Effect Spawner Coroutine Started " + minSpawnInterval);
        StartCoroutine(SpawnEffectCoroutine());
    }

    private IEnumerator SpawnEffectCoroutine()
    {
        while (true)
        {
            float waitTime = Mathf.Lerp(minSpawnInterval, maxSpawnInterval, (float)rng.NextDouble());
            Debug.LogError("Waiting " + waitTime);
            yield return new WaitForSeconds(waitTime);
            Debug.LogError("Effect Spawner spawning");


            Vector3 spawnPos = GetRandomPointInCollider(spawnArea);
            Quaternion rotation = randomizeRotation ? GetRandomRotation() : Quaternion.identity;

            Poolable poolable = ObjectPooler.Instance.GetNextOrNew(prefabToSpawn, spawnPos, rotation);
            Instantiate(prefabToSpawn, transform.position, rotation);
        }
    }

    private Vector3 GetRandomPointInCollider(Collider col)
    {
        Bounds bounds = col.bounds;
        Vector3 point;
        int safety = 0;

        do
        {
            point = new Vector3(
                Lerp(bounds.min.x, bounds.max.x, (float)rng.NextDouble()),
                Lerp(bounds.min.y, bounds.max.y, (float)rng.NextDouble()),
                Lerp(bounds.min.z, bounds.max.z, (float)rng.NextDouble())
            );

            safety++;
            if (safety > 10) break;

        } while (col.ClosestPoint(point) != point);

        return point;
    }

    private Quaternion GetRandomRotation()
    {
        float x = (float)rng.NextDouble();
        float y = (float)rng.NextDouble();
        float z = (float)rng.NextDouble();
        float w = (float)rng.NextDouble();
        Quaternion q = new Quaternion(x, y, z, w);
        return Quaternion.Normalize(q);
    }

    private float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}