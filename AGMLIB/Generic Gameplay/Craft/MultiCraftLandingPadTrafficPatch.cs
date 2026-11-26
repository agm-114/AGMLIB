using System.Reflection;
using System.Reflection.Emit;

[HarmonyPatch(typeof(CraftCarrierController), nameof(CraftCarrierController.Tick))]
internal static class MultiCraftLandingPadTrafficPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        List<CodeInstruction> code = instructions.ToList();
        MethodInfo launch = AccessTools.Method(typeof(ICraftEntryExitPoint), nameof(ICraftEntryExitPoint.LaunchCraftFromPad));
        int launchIndex = code.FindIndex(instruction => instruction.Calls(launch));
        int startedStore = code.FindIndex(launchIndex + 1, instruction => instruction.IsStloc());
        if (launchIndex < 0 || startedStore < 1 || code[startedStore - 1].opcode != OpCodes.Ldc_I4_1)
            throw new InvalidOperationException("Native craft traffic launch bookkeeping changed.");
        int started = LocalIndex(code[startedStore]);
        int[] gates = Enumerable.Range(0, code.Count).Where(index => code[index].IsLdloc() && LocalIndex(code[index]) == started).ToArray();
        int[] starts = Enumerable.Range(1, code.Count - 1)
            .Where(index => code[index].IsStloc() && LocalIndex(code[index]) == started && code[index - 1].opcode == OpCodes.Ldc_I4_1).ToArray();
        if (gates.Length != 1 || starts.Length != 2)
            throw new InvalidOperationException("Native craft traffic cycle gate changed.");
        FieldInfo reservedPad = CraftCarrierControllerInternals.ReservedPadField;
        IList<LocalVariableInfo> locals = original.GetMethodBody()!.LocalVariables;
        int order = locals.Single(local => local.LocalType == reservedPad.DeclaringType).LocalIndex;
        int pad = locals.Single(local => local.LocalType == typeof(ICraftEntryExitPoint)).LocalIndex;
        LocalBuilder previous = generator.DeclareLocal(typeof(ICraftEntryExitPoint));
        MethodInfo block = AccessTools.Method(typeof(MultiCraftLandingPad), nameof(MultiCraftLandingPad.BlockAdditionalTraffic));
        for (int index = 0; index < code.Count; index++)
        {
            yield return code[index];
            if (index == gates[0])
            {
                yield return new CodeInstruction(OpCodes.Ldloc, previous);
                yield return new CodeInstruction(OpCodes.Ldloc, order);
                yield return new CodeInstruction(OpCodes.Ldfld, reservedPad);
                yield return new CodeInstruction(OpCodes.Call, block);
            }
            if (starts.Contains(index))
            {
                yield return new CodeInstruction(OpCodes.Ldloc, pad);
                yield return new CodeInstruction(OpCodes.Stloc, previous);
            }
        }
    }

    private static int LocalIndex(CodeInstruction instruction)
    {
        short opcode = instruction.opcode.Value;
        if (opcode >= OpCodes.Ldloc_0.Value && opcode <= OpCodes.Ldloc_3.Value)
            return opcode - OpCodes.Ldloc_0.Value;
        if (opcode >= OpCodes.Stloc_0.Value && opcode <= OpCodes.Stloc_3.Value)
            return opcode - OpCodes.Stloc_0.Value;
        return instruction.operand is LocalBuilder local ? local.LocalIndex : Convert.ToInt32(instruction.operand);
    }
}
