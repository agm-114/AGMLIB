public sealed class PaintSchemeMaterialOverride : MonoBehaviour
{
    [Tooltip("Hull class this override applies to.")]
    public string ClassName = "default";

    [Tooltip("Key of the PaintSchemeMaterialTarget to change.")]
    public string TargetKey = "default";

    [Tooltip("Paint mask assigned to the target material.")]
    public Texture2D PaintMask = null!;

    private void Start()
    {
        Hull hull = GetComponentInParent<Hull>();
        if (hull == null)
        {
            Debug.LogError($"[AGMLIB Paint] Override '{TargetKey}' has no parent hull.");
            return;
        }

        if (hull.ClassName != ClassName)
        {
            Destroy(this);
            return;
        }

        PaintSchemeMaterialTarget? matchingTarget = null;
        foreach (PaintSchemeMaterialTarget target in hull.GetComponentsInChildren<PaintSchemeMaterialTarget>(true))
        {
            if (target.Key != TargetKey)
            {
                continue;
            }

            if (matchingTarget != null)
            {
                Debug.LogError($"[AGMLIB Paint] Hull '{ClassName}' has more than one material target with key '{TargetKey}'.");
                return;
            }

            matchingTarget = target;
        }

        if (matchingTarget == null)
        {
            Debug.LogError($"[AGMLIB Paint] Hull '{ClassName}' has no material target with key '{TargetKey}'.");
            return;
        }

        matchingTarget.TryApply(PaintMask);

        Destroy(this);
    }
}
