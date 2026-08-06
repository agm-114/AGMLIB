#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Lib.Testing;

[TestingComponentFactory(Order = 100)]
public sealed class DynamicReductionTestingComponentFactory : ITestingComponentFactory
{
    private const string EnabledEnvironmentVariable = "AGMLIB_DYNAMIC_REDUCTION_TEST";
    private const string SourceSaveKey = "Stock/Reinforced Thruster Nozzles";
    private const string TestingSaveKey = "agmlib-testing/Dynamic Reduction 0.9";

    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (!string.Equals(
                Environment.GetEnvironmentVariable(EnabledEnvironmentVariable),
                "1",
                StringComparison.Ordinal))
        {
            return;
        }

        ValidateAccessorBindings();

        context.Create(SourceSaveKey, TestingSaveKey, builder =>
        {
            builder
                .SetDisplayName("[TEST] Dynamic Reduction 0.9")
                .SetCategory("Testing")
                .SetDescription("Opt-in editor fixture for a Power demand multiplier of 0.9.")
                .SetPointCost(0)
                .UnlockForAllFactions();

            Common.SetVal(builder.Component, "_size", new Vector3Int(2, 2, 2));

            SimpleFilter filter = builder.AddToRoot<SimpleFilter>();
            Common.SetVal(filter, "_defaultvalue", true);

            DynamicReduction reduction = builder.AddToRoot<DynamicReduction>();
            reduction.ResourceName = "Power";
            reduction.Multiplier = 0.9f;
            reduction.Filter = filter;
        });
    }

    private static void ValidateAccessorBindings()
    {
        Type[] accessorTypes =
        [
            typeof(ShipInternals),
            typeof(HullPartResourceConnectedInternals),
            typeof(ResourcePoolInternals),
            typeof(ResourceItemInternals)
        ];

        foreach (Type accessorType in accessorTypes)
        {
            Type refsType = accessorType.GetNestedType("Refs", BindingFlags.NonPublic)
                ?? throw new MissingMemberException(accessorType.FullName, "Refs");
            RuntimeHelpers.RunClassConstructor(refsType.TypeHandle);
        }

        Debug.Log("[AGMLIB Test] event=native-accessor-bindings result=success count=4");
    }
}
#endif
