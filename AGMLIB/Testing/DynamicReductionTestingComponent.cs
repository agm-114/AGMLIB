#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using System;
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
}
#endif
