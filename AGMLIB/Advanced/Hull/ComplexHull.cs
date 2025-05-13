public class ComplexHull : Hull
{

    private GameObject _sampler => Common.GetVal<GameObject>(this, "_colliderSamplerPrefab");


    protected override BaseColliderSampler SpawnColliderSamplerInternal()
    {
        if (_sampler != null)
        {
            if (_sampler.GetComponent<ColliderSampler>() != null)
                return ColliderSampler.GetOrMakeSampler(_sampler);
        }
        return null;
    }
}