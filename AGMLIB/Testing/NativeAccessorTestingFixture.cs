#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Lib.Testing;

[TestingComponentFactory(Order = 147)]
public class NativeAccessorTestingFactory : ITestingComponentFactory
{
    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (Environment.GetEnvironmentVariable("AGMLIB_NATIVE_ACCESSOR_TEST") == "1")
            new GameObject("Native Accessor Tests").AddComponent<NativeAccessorTests>();
    }
}

internal class NativeAccessorTests : MonoBehaviour
{
    private IEnumerator Start()
    {
        float deadline = Time.realtimeSinceStartup + 120;
        while (!BundleManager.IsInitialized)
        {
            if (Time.realtimeSinceStartup >= deadline)
                throw new InvalidOperationException("Native accessor bundle loading timed out.");
            yield return null;
        }

        Type[] accessors = typeof(NativeInternalsExtensions).Assembly.GetTypes()
            .Where(type => type.Name.EndsWith("Internals", StringComparison.Ordinal) && !type.ContainsGenericParameters)
            .Append(typeof(SettingsAmmoLoadoutInternals<IMagazineProvider>))
            .Distinct().OrderBy(type => type.Name).ToArray();
        int bindings = 0;
        int failures = 0;
        foreach (Type accessor in accessors)
        {
            Type refs = accessor.GetNestedType("Refs", BindingFlags.NonPublic)
                ?? throw new MissingMemberException(accessor.FullName, "Refs");
            if (refs.ContainsGenericParameters)
                refs = refs.MakeGenericType(accessor.GetGenericArguments());
            try
            {
                RuntimeHelpers.RunClassConstructor(refs.TypeHandle);
                bindings += refs.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Length;
            }
            catch (Exception exception)
            {
                failures++;
                Debug.LogError($"[AGMLIB Test] event=native-accessor-binding type={accessor.Name} result=failure error={exception.GetBaseException()}");
            }
        }
        try { CheckPrivateData(); }
        catch (Exception exception)
        {
            failures++;
            Debug.LogError($"[AGMLIB Test] event=native-private-fields result=failure error={exception}");
        }
        Debug.Log($"[AGMLIB Test] event=native-accessors result={(failures == 0 ? "success" : "failure")} types={accessors.Length} bindings={bindings} failures={failures}");
        if (Environment.GetEnvironmentVariable("AGMLIB_DYNAMIC_STUN_TEST") != "1")
            Application.Quit();
    }

    private static void CheckPrivateData()
    {
        Type bulletType = AccessTools.Inner(typeof(BallisticRaycastMuzzle), "RaycastBullet");
        object bullet = FormatterServices.GetUninitializedObject(bulletType);
        AccessTools.Field(bulletType, "Lifetime").SetValue(bullet, 5f);
        AccessTools.Field(bulletType, "Position").SetValue(bullet, Vector3.one);
        Require(BallisticRaycastMuzzleInternals.GetBulletLifetime(bullet) == 5f, "bullet-lifetime");
        Require(BallisticRaycastMuzzleInternals.GetBulletPosition(bullet) == Vector3.one, "bullet-position");
        Require(BallisticRaycastMuzzleInternals.GetBulletLifetime(null!) == 0, "null-default");

        Type debuffType = AccessTools.Inner(typeof(HullComponent), "DebuffInstance");
        object debuff = FormatterServices.GetUninitializedObject(debuffType);
        AccessTools.Field(debuffType, "InstanceID").SetValue(debuff, 73);
        Require(HullComponentInternals.GetDebuffInstanceId(debuff) == 73, "debuff-instance-id");
        Require(HullComponentInternals.GetInstanceDebuff(debuff) == null, "debuff-reference");

        object order = FormatterServices.GetUninitializedObject(AccessTools.Inner(typeof(CraftCarrierController), "TrafficOrder"));
        object originalType = CraftCarrierControllerInternals.GetOrderType(order);
        CraftCarrierControllerInternals.SetOrderType(order, Enum.ToObject(originalType.GetType(), -1));
        Require(Convert.ToInt32(CraftCarrierControllerInternals.GetOrderType(order)) == -1, "launch-suspension");
        CraftCarrierControllerInternals.SetOrderType(order, originalType);
        Require(CraftCarrierControllerInternals.GetOrderType(order).Equals(originalType), "launch-restoration");
        Require(CraftCarrierControllerInternals.GetOrderType(null!) == null, "null-private-enum");
        Debug.Log("[AGMLIB Test] event=native-private-fields result=success checks=8");
    }

    private static void Require(bool condition, string check)
    {
        if (!condition)
            throw new InvalidOperationException($"Native private field check failed: {check}");
    }
}
#endif
