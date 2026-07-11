public static class AreaEffectRuntime
{
    public static IEnumerable<HullComponent> ComponentsInRadius(Vector3 position, float radius)
    {
        if (radius <= 0f)
        {
            return Physics.OverlapSphere(position, 0.1f, 2048, QueryTriggerInteraction.Collide)
                .Select(hit => hit.GetComponentInParent<HullComponent>())
                .Where(component => component != null)
                .Distinct();
        }

        return MunitionsHelpers.SphereOverlapComponentsUnlimited(position, radius)
            .OfType<HullComponent>()
            .Distinct();
    }

    public static ShipController HitShip(MunitionHitInfo hitInfo)
    {
        return hitInfo?.HitObject?.transform?.root?.GetComponent<ShipController>();
    }
}
