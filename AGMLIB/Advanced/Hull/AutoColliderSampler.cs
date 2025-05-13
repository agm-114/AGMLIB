using Object = UnityEngine.Object;

public class AutoColliderSampler : BaseColliderSampler
{
    private static Dictionary<GameObject, AutoColliderSampler> _samplers = new Dictionary<GameObject, AutoColliderSampler>();

    protected MeshCollider[] _colliders;

    protected override MeshCollider[] _allColliders => _colliders;

    public static AutoColliderSampler GetOrMakeSampler(GameObject prefab)
    {
        AutoColliderSampler value = null;
        if (_samplers.TryGetValue(prefab, out value))
        {
            return value;
        }

        GameObject gameObject = Object.Instantiate(prefab);
        gameObject.transform.position = Vector3.zero;
        value = gameObject.GetComponent<AutoColliderSampler>();
        value._colliders = gameObject.GetComponentsInChildren<MeshCollider>();
        _samplers.Add(prefab, value);
        return value;
    }

    private void Awake()
    {
        Build();
    }

    private void OnDestroy()
    {
        foreach (KeyValuePair<GameObject, AutoColliderSampler> sampler in _samplers)
        {
            if (sampler.Value == this)
            {
                _samplers.Remove(sampler.Key);
                break;
            }
        }
    }

    public Dictionary<Mesh, Mesh> MeshMap = new Dictionary<Mesh, Mesh>();
    public bool FuzzySample = false;

    public new Vector2? SampleUV(Mesh colliderSharedMesh, Vector3 point, Vector3 normal)
    {
        Vector2? trybase = base.SampleUV(colliderSharedMesh, point, normal);
        if (trybase.HasValue)
            return trybase;
        if (MeshMap.ContainsKey(colliderSharedMesh))
            return base.SampleUV(MeshMap[colliderSharedMesh], point, normal);
        else
            return base.SampleUV(MeshMap[colliderSharedMesh], point, normal);
        base.gameObject.SetActive(value: true);
        Ray ray = new Ray(point + normal * _rayBackoff, -normal);
        if (FuzzySample && Physics.Raycast(ray, out var hitInfo, _rayBackoff * 2f))
        {
            base.gameObject.SetActive(value: false);
            return hitInfo.textureCoord;
        }

        base.gameObject.SetActive(value: false);
        return null;
    }

}