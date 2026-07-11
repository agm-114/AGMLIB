using System.Collections;
using System.Reflection;

[CreateAssetMenu(fileName = "New Area Debuff Profile", menuName = "Nebulous/Modules/Area Effects/Debuff Profile")]
public class AreaDebuffProfileModule : ScriptableObject, IOnDamageableImpact, IOnGenericImpact
{
    [SerializeField]
    protected float _effectRadius = 0f;

    [SerializeField]
    protected float _durationSeconds = 0f;

    [SerializeField]
    protected List<ComponentDebuff> _normalDebuffs = new();

    [SerializeField]
    protected List<ComponentDebuff> _timeLimitedDebuffs = new();

    [SerializeField]
    protected List<ComponentDebuff> _forcedDebuffs = new();

    public float EffectRadius => _effectRadius;
    public float DurationSeconds => _durationSeconds;
    public IList<ComponentDebuff> NormalDebuffs => _normalDebuffs;
    public IList<ComponentDebuff> TimeLimitedDebuffs => _timeLimitedDebuffs;
    public IList<ComponentDebuff> ForcedDebuffs => _forcedDebuffs;

    public void OnDamageableImpact(LightweightKineticShell shell, LightweightKineticMunitionContainer attachedTo, IDamageable hitObject, MunitionHitInfo hitInfo, bool trigger, HitResult hitRes, float damageDone, bool targetDestroyed)
    {
        Apply(hitInfo);
    }

    public void OnGenericImpact(LightweightKineticShell shell, LightweightKineticMunitionContainer attachedTo, MunitionHitInfo hitInfo, bool trigger, HitResult hitRes)
    {
        Apply(hitInfo);
    }

    public void Apply(MunitionHitInfo hitInfo)
    {
        if (hitInfo == null || !HasAnyDebuffs())
        {
            return;
        }

        MethodInfo addDebuff = typeof(HullComponent).GetMethod("AddDebuffToComponent", Common.FunFlags);
        if (addDebuff == null)
        {
            return;
        }

        foreach (HullComponent component in AreaEffectRuntime.ComponentsInRadius(hitInfo.Point, EffectRadius))
        {
            MunitionHitInfo componentHitInfo = MakeLocalHitInfo(hitInfo, component);
            foreach (ComponentDebuff debuff in ValidDebuffs(NormalDebuffs))
            {
                addDebuff.Invoke(component, new object[] { debuff, componentHitInfo, true });
            }

            foreach (ComponentDebuff debuff in ValidDebuffs(ForcedDebuffs))
            {
                addDebuff.Invoke(component, new object[] { debuff, componentHitInfo, false });
            }

            foreach (ComponentDebuff debuff in ValidDebuffs(TimeLimitedDebuffs))
            {
                HashSet<int> existingInstances = ActiveDebuffInstanceIds(component, debuff);
                addDebuff.Invoke(component, new object[] { debuff, componentHitInfo, true });
                foreach (int instanceId in ActiveDebuffInstanceIds(component, debuff).Where(instanceId => !existingInstances.Contains(instanceId)))
                {
                    component.gameObject.GetOrAddComponent<TimedAreaDebuffRemoval>()
                        .Schedule(component, instanceId, DurationSeconds);
                }
            }
            componentHitInfo?.Dispose();
        }
    }

    private bool HasAnyDebuffs()
    {
        return ValidDebuffs(NormalDebuffs).Any()
            || ValidDebuffs(TimeLimitedDebuffs).Any()
            || ValidDebuffs(ForcedDebuffs).Any();
    }

    private static IEnumerable<ComponentDebuff> ValidDebuffs(IEnumerable<ComponentDebuff> debuffs)
    {
        return debuffs?.Where(debuff => debuff != null) ?? Enumerable.Empty<ComponentDebuff>();
    }

    private static HashSet<int> ActiveDebuffInstanceIds(HullComponent component, ComponentDebuff debuff)
    {
        HashSet<int> instanceIds = new();
        object activeDebuffs = Common.GetVal<object>(component, "_activeDebuffs");
        if (activeDebuffs is not IEnumerable debuffInstances)
        {
            return instanceIds;
        }

        foreach (object instance in debuffInstances)
        {
            ComponentDebuff activeDebuff = Common.GetVal<ComponentDebuff>(instance, "Debuff");
            if (activeDebuff == debuff)
            {
                instanceIds.Add(Common.GetVal<int>(instance, "InstanceID"));
            }
        }

        return instanceIds;
    }

    private static MunitionHitInfo MakeLocalHitInfo(MunitionHitInfo source, HullComponent component)
    {
        MunitionHitInfo local = MunitionHitInfo.Take();
        local.HitObject = component.gameObject;
        local.HitCollider = source.HitCollider;
        local.Point = source.Point;
        local.LocalPoint = component.transform.InverseTransformPoint(source.Point);
        local.Normal = source.Normal;
        local.HitNormal = source.HitNormal;
        local.LocalNormal = component.transform.InverseTransformDirection(source.Normal);
        local.LocalHitNormal = component.transform.InverseTransformDirection(source.HitNormal);
        local.HitUV = source.HitUV;
        return local;
    }
}

public class TimedAreaDebuffRemoval : MonoBehaviour
{
    public void Schedule(HullComponent component, int instanceId, float durationSeconds)
    {
        if (component != null && durationSeconds > 0f)
        {
            StartCoroutine(RemoveAfterDelay(component, instanceId, durationSeconds));
        }
    }

    private IEnumerator RemoveAfterDelay(HullComponent component, int instanceId, float durationSeconds)
    {
        yield return new WaitForSeconds(durationSeconds);

        if (component == null)
        {
            yield break;
        }

        object rpcProvider = Common.GetVal<object>(component, "_componentRpcProvider");
        MethodInfo removeDebuff = rpcProvider?.GetType().GetMethod("RemoveDebuff", Common.FunFlags);
        if (removeDebuff == null)
        {
            yield break;
        }

        removeDebuff.Invoke(rpcProvider, new object[] { component, instanceId });
    }
}
