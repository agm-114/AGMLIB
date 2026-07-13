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
//[HarmonyPatch(typeof(SocketOutlineManager), nameof(SocketOutlineManager.SetSockets))]
class SocketOutlineManagerSetSockets
{
    static void Prefix(SocketOutlineManager __instance, Camera cam)
    {

    }
}

[HarmonyPatch(typeof(SocketOutlineManager), nameof(SocketOutlineManager.DrawShapes))]
class SocketOutlineManagerDrawShapes
{
    static Random random = new Random();
    static SocketGroupDropdown hoveredGroupOwner;
    static SocketOutlineManager hoveredGroupManager;
    static IReadOnlyCollection<HullSocket> hoveredGroupSockets;

    public static void SetHoveredGroup(SocketGroupDropdown owner, SocketOutlineManager manager, IReadOnlyCollection<HullSocket> sockets)
    {
        hoveredGroupOwner = owner;
        hoveredGroupManager = manager;
        hoveredGroupSockets = sockets;
    }

    public static void ClearHoveredGroup(SocketGroupDropdown owner)
    {
        if (!ReferenceEquals(hoveredGroupOwner, owner))
        {
            return;
        }

        hoveredGroupOwner = null;
        hoveredGroupManager = null;
        hoveredGroupSockets = null;
    }

    static bool Prefix(SocketOutlineManager __instance, Camera cam,
        Camera ____camera,
        Color ____emptyColor, Color ____filledColor, Color ____selectedColor, Color ____arcHardLimit, Color ____arcSoftLimit,
        float ____arcThickness, float ____arcRadiusBuffer, float ____arcDashScale, float ____arcDashSpacing, float ____arcLift,
        bool ____visible, HullSocket[] ____sockets, HullSocket ____hoveredSocket, HullSocket ____selectedSocket

    )
    {
        //Common.LogPatch();

        //Debug.LogError("Help me I am trapped in a scripting factory ");
        Camera _camera = ____camera;

        Color _emptyColor = ____emptyColor;

        Color _filledColor = ____filledColor;
        Color _selectedColor = ____selectedColor;
        float _arcThickness = ____arcThickness;
        float _arcRadiusBuffer = ____arcRadiusBuffer;

        Color _arcHardLimit = ____arcHardLimit;
        Color _arcSoftLimit = ____arcSoftLimit;

        float _arcDashScale = ____arcDashScale;

        float _arcDashSpacing = ____arcDashSpacing;

        float _arcLift = ____arcLift;

        bool _visible = ____visible;

        HullSocket[] _sockets = ____sockets;

        HullSocket _hoveredSocket = ____hoveredSocket;

        HullSocket _selectedSocket = ____selectedSocket;

        if (!_visible || _sockets == null)
            return false;
        foreach (HullSocket socket in _sockets.OrderByDescending((x) => Vector3.Distance(x.transform.position, _camera.transform.position)))
        {
            using (Draw.Command(cam, CustomPassInjectionPoint.AfterPostProcess))
            {

                Draw.SizeSpace = ThicknessSpace.Meters;
                Draw.ThicknessSpace = ThicknessSpace.Meters;
                Draw.RadiusSpace = ThicknessSpace.Meters;
                Draw.ZTest = CompareFunction.Always;
                Draw.Matrix = socket.transform.localToWorldMatrix;
                List<Color> numbers = new List<Color> { Color.green, Color.blue, Color.white, Color.black, Color.yellow, Color.cyan, Color.magenta };


                //Draw.BackgroundColor = numbers[random.Next(numbers.Count)];
                bool groupHoverActive = __instance == hoveredGroupManager && hoveredGroupSockets != null;
                bool groupHovered = groupHoverActive && hoveredGroupSockets.Contains(socket);
                bool singleSocketHovered = !groupHoverActive && socket == _hoveredSocket;
                Draw.Color = socket == _selectedSocket || groupHovered || singleSocketHovered
                        ? _selectedColor
                        : socket.Component != null ? _filledColor : _emptyColor;

                //Draw.BackgroundColor = numbers[random.Next(numbers.Count)];

                //Testing

                //End testing


                SocketEditorUISettings socketFilters = socket.GetComponent<SocketEditorUISettings>() ?? new SocketEditorUISettings();
                Draw.Radius = socketFilters.Radius;
                switch (socketFilters.Shape)
                {
                    case RenderShape.Default:
                        Draw.Cuboid(Vector3.zero, Quaternion.identity, (Vector3)socket.Size.Dimensions * 0.25f);
                        break;
                    case RenderShape.Sphere:
                        Draw.Sphere(Vector3.zero);
                        break;
                    case RenderShape.Torus:
                        Draw.Torus(Vector3.zero, socketFilters.Radius, socketFilters.Thickness);
                        break;
                    case RenderShape.Cone:
                        Draw.Cone(Vector3.zero, socketFilters.Radius, socketFilters.Thickness);
                        break;
                    case RenderShape.Invisible:
                        break;
                    default:
                        break;
                }
                if (socketFilters.DrawColor != Color.clear)
                {
                    Color GetDebugColor(HullSocket socket)
                    {
                        return socket.Type switch
                        {
                            HullSocketType.Surface => Color.red,
                            HullSocketType.Compartment => Color.yellow,
                            HullSocketType.Module => Color.cyan,
                            _ => Color.white,
                        };
                    }
                    Color newcolor = GetDebugColor(socket);
                    newcolor.a = Draw.Color.a;
                    Draw.Color = newcolor;
                }


            }
        }
        for (int i = 0; i < _sockets.Length; i++)
        {
            HullSocket socket = _sockets[i];
            if (socket.Type != 0)
                continue;
            if (socket.Component != null && socket.Component.UseTraversalArcs)
            {
                void DrawArc(Quaternion rot, float angleStart, float angleEnd, bool circle = false)
                {
                    using (Draw.Command(cam, CustomPassInjectionPoint.AfterPostProcess))
                    {
                        Draw.LineGeometry = LineGeometry.Volumetric3D;
                        Draw.ThicknessSpace = ThicknessSpace.Noots;
                        Draw.Thickness = _arcThickness;
                        Draw.UseDashes = true;
                        Draw.DashSpace = DashSpace.Meters;
                        Draw.DashSize = _arcDashScale;
                        Draw.DashSpacing = _arcDashSpacing;
                        Draw.LineEndCaps = LineEndCap.Round;
                        Draw.Matrix = socket.transform.localToWorldMatrix;
                        Vector3Int socketSize = socket.Size.Dimensions;
                        float arcRadius = Mathf.Max(socketSize.x, socketSize.z) * 0.25f * _arcRadiusBuffer;
                        Vector3 attach = socket.AttachPoint / 10f * _arcLift;
                        Draw.Color = _arcSoftLimit;
                        if (circle)
                        {
                            Draw.Ring(attach, Vector3.up, arcRadius, _arcThickness);
                            return;
                        }

                        Draw.Color = (socket?.TraverseLimits?.LimitFiringOnly ?? false) ? _arcSoftLimit : _arcHardLimit;

                        Draw.Arc(attach, rot * Quaternion.LookRotation(Vector3.up), arcRadius, angleStart * ((float)Math.PI / 180f), angleEnd * ((float)Math.PI / 180f));
                    }
                }
                //x = 90 makes elevation
                if (socket.TraverseLimits is not TraversalLimits _rearLimits)
                {
                    DrawArc(Quaternion.identity, 0, 0, true);
                    continue;
                }
                if (socket.gameObject.transform.GetComponent<CustomTraversalLimits>()?.PublicForwardLimits is not TraversalLimits _forwardLimits || socket.gameObject.transform.GetComponent<CustomTraversalLimits>().Ignore)
                {
                    DrawArc(Quaternion.Euler(0f, -90f, 0f), 0f - _rearLimits.LeftAngle, _rearLimits.RightAngle);
                    continue;
                }
                //Debug.LogError("Rear arc");
                //Debug.LogError(_rearLimits.LeftAngle + " " + _rearLimits.RightAngle);
                //Debug.LogError("Front arc");
                //Debug.LogError(_forwardLimits.LeftAngle + " " + _forwardLimits.RightAngle);
                //Draw.Arc(attach, Quaternion.Euler(0f, 0f, 0f) * Quaternion.LookRotation(Vector3.up), arcRadius, (0f - socket2.TraverseLimits.Value.LeftAngle) * ((float)Math.PI / 180f), (socket2.TraverseLimits.Value.RightAngle - 90f) * ((float)Math.PI / 180f));

                //Draw.Arc(attach, Quaternion.Euler(0f, 0f, 0f) * Quaternion.LookRotation(Vector3.up), arcRadius, (90f - _forwardLimits.RightAngle) * ((float)Math.PI / 180f), (socket2.TraverseLimits.Value.RightAngle - 90f) * ((float)Math.PI / 180f));
                DrawArc(Quaternion.Euler(0f, 0f, 0f), -90f + _forwardLimits.RightAngle, socket.TraverseLimits.Value.RightAngle - 90f);
                DrawArc(Quaternion.Euler(0f, 0f, 180f), -90f + _forwardLimits.LeftAngle, socket.TraverseLimits.Value.LeftAngle - 90f);

            }
        }
        return false;
    }
}

