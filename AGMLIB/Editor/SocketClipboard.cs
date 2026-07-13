using FleetEditor;
using Lib;
using Lib.Dynamic_Systems.Area;
using Munitions.ModularMissiles;
using Shapes;
using SmallCraft;
using Steamworks.Ugc;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.EventSystems;
using static Utility.GameColors;

//using static UnityEditorInternal.ReorderableList;#dll
using Random = System.Random;
/*
    <Reference Include="Unity.RenderPipelines.HighDefinition.Runtime">
      <HintPath>libs\Unity.RenderPipelines.HighDefinition.Runtime.dll</HintPath>
    </Reference>
 */
[HarmonyPatch(typeof(FleetEditor.Clipboard.SocketClipboardItem), MethodType.Constructor, new Type[] { typeof(HullSocket) })]
class SocketClipboardItemConstructorPatch
{
    static void Postfix(FleetEditor.Clipboard.SocketClipboardItem __instance, HullSocket socket)
    {
        SocketEditorChildSettings childSettings = socket.GetComponent<SocketEditorChildSettings>();
        if (childSettings == null || childSettings.VisualChildren.Count == 0)
        {
            return;
        }

        List<CopiedSocketChild> children = childSettings.VisualChildren
            .Select((child, index) => new { child, index })
            .Where(entry => entry.child != null)
            .Select(entry => new CopiedSocketChild(entry.index, new FleetEditor.Clipboard.SocketClipboardItem(entry.child)))
            .ToList();

        if (children.Count > 0)
        {
            SocketClipboardChildren.Items.Add(__instance, children);
        }
    }
}

[HarmonyPatch(typeof(FleetEditor.Clipboard.SocketClipboardItem), nameof(FleetEditor.Clipboard.SocketClipboardItem.Apply))]
class SocketClipboardItemApplyPatch
{
    static void Postfix(FleetEditor.Clipboard.SocketClipboardItem __instance, FleetCompositionSubmodeController editor, object toTarget)
    {
        if (toTarget is not HullSocket targetSocket ||
            !SocketClipboardChildren.Items.TryGetValue(__instance, out List<CopiedSocketChild> copiedChildren))
        {
            return;
        }

        SocketEditorChildSettings targetSettings = targetSocket.GetComponent<SocketEditorChildSettings>();
        if (targetSettings == null)
        {
            return;
        }

        foreach (CopiedSocketChild copiedChild in copiedChildren)
        {
            if (copiedChild.Index >= targetSettings.VisualChildren.Count)
            {
                continue;
            }

            HullSocket targetChild = targetSettings.VisualChildren[copiedChild.Index];
            if (targetChild == null || !copiedChild.ClipboardItem.ValidTarget(targetChild))
            {
                continue;
            }

            copiedChild.ClipboardItem.Apply(editor, targetChild, out _);
        }
    }
}

class CopiedSocketChild
{
    public int Index { get; }
    public FleetEditor.Clipboard.SocketClipboardItem ClipboardItem { get; }

    public CopiedSocketChild(int index, FleetEditor.Clipboard.SocketClipboardItem clipboardItem)
    {
        Index = index;
        ClipboardItem = clipboardItem;
    }
}

static class SocketClipboardChildren
{
    public static readonly ConditionalWeakTable<FleetEditor.Clipboard.SocketClipboardItem, List<CopiedSocketChild>> Items = new();
}

