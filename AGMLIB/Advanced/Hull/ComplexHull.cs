public class ComplexHull : Hull
{

    private GameObject _sampler => this.Internals().ColliderSamplerPrefab;


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