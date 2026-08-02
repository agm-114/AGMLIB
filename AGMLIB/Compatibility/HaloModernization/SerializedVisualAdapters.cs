using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Ships;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Utility;
using Utility.Localization;

namespace AGMLIB.Compatibility.HaloModernization;

public class CrewJobLabels : MonoBehaviour
{
    [SerializeField]
    [FormerlySerializedAs("FactionKey")]
    private string _factionKey;

    [SerializeField]
    [FormerlySerializedAs("JobNames")]
    private string[] _jobNames = [];

    [SerializeField]
    [FormerlySerializedAs("CrewRequired")]
    private int[] _crewRequired = [];

    public string FactionKey => _factionKey;

    public IReadOnlyList<string> JobNames => _jobNames;

    public IReadOnlyList<int> CrewRequired => _crewRequired;

    public bool AppliesTo(string factionKey)
    {
        return string.IsNullOrEmpty(_factionKey)
            || string.Equals(_factionKey, factionKey, StringComparison.Ordinal);
    }

    public int Count => Math.Min(_jobNames?.Length ?? 0, _crewRequired?.Length ?? 0);
}

[HarmonyPatch(typeof(Ship), nameof(Ship.EditorFormatCrew))]
internal static class CrewJobLabelsPatch
{
    private static readonly string[] PreferredCustomJobOrder =
    [
        "Superior",
        "Engineer",
        "Menial",
    ];

    private static void Postfix(Ship __instance, ref List<(string, string)> __result)
    {
        if (__instance?.Hull == null || __result == null)
            return;

        CrewOperatedComponent[] components = __instance
            .Hull
            .CollectComponents<CrewOperatedComponent>()
            .ToArray();
        if (
            !components.Any(
                component =>
                    component.GetComponent<CrewJobLabels>() is { } labels
                    && labels.AppliesTo(__instance.Hull.FactionKey)
            )
        )
        {
            return;
        }

        var defaultJobs = new SortedDictionary<string, (int Crew, string Unit)>(
            StringComparer.Ordinal
        );
        var customJobs = new SortedDictionary<string, int>(StringComparer.Ordinal);
        foreach (CrewOperatedComponent component in components)
        {
            CrewJobLabels labels = component.GetComponent<CrewJobLabels>();
            if (labels != null && labels.AppliesTo(__instance.Hull.FactionKey))
            {
                for (int index = 0; index < labels.Count; index++)
                {
                    string jobName = labels.JobNames[index];
                    int crew = labels.CrewRequired[index];
                    if (string.IsNullOrWhiteSpace(jobName) || crew == 0)
                        continue;
                    customJobs[jobName] =
                        customJobs.TryGetValue(jobName, out int existing)
                            ? existing + crew
                            : crew;
                }
                continue;
            }

            component.GetCrewStats(out string job, out int jobCrew, out string unit);
            if (string.IsNullOrWhiteSpace(job) || jobCrew == 0)
                continue;
            if (defaultJobs.TryGetValue(job, out (int Crew, string Unit) existingJob))
                defaultJobs[job] = (existingJob.Crew + jobCrew, unit);
            else
                defaultJobs.Add(job, (jobCrew, unit));
        }

        (string, string)? crewComplement = __result.Count > 0 ? __result[0] : null;
        __result.Clear();
        if (crewComplement.HasValue)
            __result.Add(crewComplement.Value);
        foreach (
            KeyValuePair<string, (int Crew, string Unit)> job in defaultJobs
        )
        {
            __result.Add(
                (
                    "   " + job.Key.MakeLocalizationToken("SHIPSTAT_JOB"),
                    $"{job.Value.Crew} {job.Value.Unit}"
                )
            );
        }
        foreach (string jobName in PreferredCustomJobOrder)
        {
            if (customJobs.TryGetValue(jobName, out int crew))
                __result.Add(("   " + jobName, crew.ToString()));
        }
        foreach (
            KeyValuePair<string, int> job in customJobs.Where(
                job => !PreferredCustomJobOrder.Contains(job.Key)
            )
        )
        {
            __result.Add(("   " + job.Key, job.Value.ToString()));
        }
    }
}

public class RotationFollower : MonoBehaviour
{
    [SerializeField]
    [FormerlySerializedAs("target")]
    private Transform _target;

    private void LateUpdate()
    {
        if (_target != null)
            transform.rotation = _target.rotation;
    }
}

public class VisualEffectStateFollower : MonoBehaviour
{
    [SerializeField]
    [FormerlySerializedAs("masterVFX")]
    private VisualEffect _master;

    [SerializeField]
    [FormerlySerializedAs("ownVFX")]
    private VisualEffect _follower;

    private bool _wasPlaying;

    private void OnEnable()
    {
        Refresh(force: true);
    }

    private void Update()
    {
        Refresh(force: false);
    }

    private void Refresh(bool force)
    {
        if (_master == null || _follower == null)
            return;

        bool playing = _master.aliveParticleCount > 0;
        if (!force && playing == _wasPlaying)
            return;

        _wasPlaying = playing;
        if (playing)
            _follower.Play();
        else
            _follower.Stop();
    }
}

public class SocketComponentScaler : MonoBehaviour
{
    [SerializeField]
    private float _scale = 1f;

    [SerializeField]
    [FormerlySerializedAs("_equipmentList")]
    private string[] _componentSaveKeys = [];

    private HashSet<string> _componentSet;

    public float GetScale(string componentSaveKey)
    {
        _componentSet ??= new HashSet<string>(
            _componentSaveKeys.Where(key => !string.IsNullOrWhiteSpace(key)),
            StringComparer.Ordinal
        );
        return _componentSet.Contains(componentSaveKey) ? _scale : 1f;
    }
}

[HarmonyPatch(typeof(HullSocket), nameof(HullSocket.SetComponent))]
internal static class SocketComponentScalerPatch
{
    private static void Postfix(HullSocket __instance, HullComponent __result)
    {
        if (__instance == null || __result == null || __instance.Type != HullSocketType.Surface)
            return;

        SocketComponentScaler scaler =
            __instance.GetComponent<SocketComponentScaler>()
            ?? __instance.transform.parent?.GetComponent<SocketComponentScaler>();
        if (scaler == null)
            return;

        float scale = scaler.GetScale(__result.SaveKey);
        if (Mathf.Approximately(scale, 1f))
            return;

        Transform installed = __result.transform;
        Transform parent = installed.parent;
        installed.SetParent(null, worldPositionStays: true);
        installed.localScale *= scale;
        installed.SetParent(parent, worldPositionStays: true);
    }
}

public class IndexedComponentHullPaint : ComponentHullPaint
{
    [SerializeField]
    [Min(0)]
    [FormerlySerializedAs("MaterialIndex")]
    private int _materialIndex;

    public override void SetColors(Color baseColor, Color stripeColor)
    {
        LODGroupSharedMaterial materials = GetComponent<LODGroupSharedMaterial>();
        if (materials == null)
            return;

        materials[_materialIndex].SetColor("_BasePaintTint", baseColor);
        materials[_materialIndex].SetColor("_StripePaintTint", stripeColor);
    }
}

public class EmissiveComponentHullPaint : ComponentHullPaint
{
    [SerializeField]
    private MeshRenderer _renderer;

    [SerializeField]
    private string _primaryProperty = "_Emission_Color";

    [SerializeField]
    private string _secondaryProperty = "_Emission_Color2";

    public override void SetColors(Color baseColor, Color stripeColor)
    {
        if (_renderer == null)
            return;

        Material material = _renderer.material;
        material.SetColor(_primaryProperty, RetintPreservingValue(material.GetColor(_primaryProperty), baseColor));
        material.SetColor(
            _secondaryProperty,
            RetintPreservingValue(material.GetColor(_secondaryProperty), stripeColor)
        );
    }

    private static Color RetintPreservingValue(Color source, Color tint)
    {
        Color.RGBToHSV(source, out _, out _, out float value);
        Color.RGBToHSV(tint, out float hue, out float saturation, out _);
        return Color.HSVToRGB(hue, saturation, value, hdr: true);
    }
}

public class FunctionalBarrelGlow : MonoBehaviour
{
    [SerializeField]
    private VisualEffect _visualEffect;

    [SerializeField]
    private Light _lightSource;

    [SerializeField]
    private AnimationCurve _lightIntensityCurve = new(
        new Keyframe(0f, 10f),
        new Keyframe(1f, 1f)
    );

    private HullPart _hullPart;
    private float _baseLightIntensity;
    private float _curveDuration;
    private float _curveTime;

    private void Awake()
    {
        _hullPart = GetComponentInParent<HullPart>();
        if (_hullPart != null)
            _hullPart.OnIsFunctionalChanged += HandleFunctionalityChanged;

        if (_lightSource != null)
            _baseLightIntensity = _lightSource.intensity;
        if (_lightIntensityCurve.length > 0)
            _curveDuration = _lightIntensityCurve.keys[_lightIntensityCurve.length - 1].time;
        _curveTime = _curveDuration;
        RefreshFunctionality();
        RefreshLight();
    }

    private void OnDestroy()
    {
        if (_hullPart != null)
            _hullPart.OnIsFunctionalChanged -= HandleFunctionalityChanged;
        _hullPart = null;
    }

    private void Update()
    {
        if (_curveTime >= _curveDuration)
            return;

        _curveTime = Mathf.Min(_curveTime + Time.deltaTime, _curveDuration);
        RefreshLight();
    }

    public void FireInstant()
    {
        _curveTime = 0f;
        RefreshLight();
    }

    private void HandleFunctionalityChanged(HullPart part)
    {
        RefreshFunctionality();
        RefreshLight();
    }

    private void RefreshFunctionality()
    {
        if (_visualEffect == null)
            return;
        if (_hullPart == null || _hullPart.IsFunctional)
            _visualEffect.Play();
        else
            _visualEffect.Stop();
    }

    private void RefreshLight()
    {
        if (_lightSource == null)
            return;

        bool functional = _hullPart == null || _hullPart.IsFunctional;
        _lightSource.intensity = functional
            ? _baseLightIntensity * _lightIntensityCurve.Evaluate(_curveTime)
            : 0f;
    }
}

public class SocketCapController : MonoBehaviour
{
    [SerializeField]
    [FormerlySerializedAs("socket")]
    private HullSocket _socket;

    [SerializeField]
    [FormerlySerializedAs("cap")]
    private GameObject _cap;

    [SerializeField]
    [FormerlySerializedAs("ComponentName")]
    private string _keepCapForComponentSaveKey;

    private void Awake()
    {
        if (_socket == null)
            _socket = GetComponent<HullSocket>();
        if (_socket == null)
            return;

        _socket.OnInstalledComponentChanged += HandleInstalledComponentChanged;
        HandleInstalledComponentChanged(_socket.Component);
    }

    private void OnDestroy()
    {
        if (_socket != null)
            _socket.OnInstalledComponentChanged -= HandleInstalledComponentChanged;
        _socket = null;
    }

    private void HandleInstalledComponentChanged(HullComponent component)
    {
        if (_cap == null)
            return;

        bool keepForSelectedComponent =
            component != null
            && !string.IsNullOrWhiteSpace(_keepCapForComponentSaveKey)
            && string.Equals(
                component.SaveKey,
                _keepCapForComponentSaveKey,
                StringComparison.Ordinal
            );
        _cap.SetActive(component == null || keepForSelectedComponent);
    }
}

public class TurretBaseVisibilityMarker : MonoBehaviour
{
}

public class TurretBaseVisibilityAdapter : MonoBehaviour
{
    [SerializeField]
    private HullComponent _component;

    [SerializeField]
    private GameObject[] _baseObjects = [];

    private HullSocket _lastSocket;

    private void OnEnable()
    {
        _lastSocket = null;
    }

    private void LateUpdate()
    {
        if (_component == null)
            _component = GetComponentInParent<HullComponent>();

        HullSocket socket = _component?.Socket as HullSocket;
        if (socket == _lastSocket)
            return;

        _lastSocket = socket;
        bool showBase =
            socket == null || socket.GetComponent<TurretBaseVisibilityMarker>() == null;
        foreach (GameObject baseObject in _baseObjects)
            baseObject?.SetActive(showBase);
    }
}
