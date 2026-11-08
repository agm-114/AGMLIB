namespace Lib.Diagnostics;

internal enum RayDamageOutcome { Miss, NoDamage, Damage }

// Snapshots deliberately contain no live missiles, colliders, hit-info leases, or damage dealers.
[Serializable]
internal class SalvoTrace(int id, string launcher)
{
    public int SalvoId = id;
    public string Launcher = launcher;
    public List<MissileTrace> Missiles = [];
    public bool Finished => Missiles.Count > 0 && Missiles.All(missile => missile.Status != ActiveMissileSalvo.MissileStatus.InFlight);
    public float Damage => Missiles.Sum(missile => missile.Rays.Sum(ray => ray.ReportedDamage));
    public string Title => $"Salvo {SalvoId} / {Launcher}";
}

[Serializable]
internal class MissileTrace(uint id, string name)
{
    internal const int MaxPoints = 1024;
    internal const int MaxRays = 256;
    public uint MissileId = id;
    public string Name = name;
    public ActiveMissileSalvo.MissileStatus Status;
    public List<FlightPoint> Path = [];
    public List<BeamRayTrace> Rays = [];
    public int DroppedRays;
    public int PathDecimations;
    [NonSerialized] private float _sampleInterval = 0.1f;

    public void Sample(float time, Vector3 position, bool force = false)
    {
        if (Path.Count > 0 && Path[Path.Count - 1].Time == time && Path[Path.Count - 1].Position == position)
            return;
        if (!force && Path.Count > 0 && time - Path[Path.Count - 1].Time < _sampleInterval)
            return;
        if (Path.Count == MaxPoints)
        {
            // Keep the launch point and downsample the older path before appending the latest point.
            for (int i = 1; i < MaxPoints / 2; i++)
                Path[i] = Path[i * 2];
            Path.RemoveRange(MaxPoints / 2, MaxPoints / 2);
            _sampleInterval *= 2f;
            PathDecimations++;
        }
        Path.Add(new FlightPoint(time, position));
    }
}

[Serializable]
internal class FlightPoint(float time, Vector3 position)
{
    public float Time = time;
    public Vector3 Position = position;
}

[Serializable]
internal class BeamRayTrace
{
    internal const int MaxSteps = 128;
    public float Time;
    public uint MissileId;
    public int RayIndex;
    public Vector3 Origin;
    public Vector3 End;
    public string Kind = "Beam ray";
    public string Target = "Miss";
    public string Collider = "";
    public string Damager = "Not created (no damageable hit)";
    public string Settings = "";
    public float Range;
    public float SocketSize;
    public float SizeDamage;
    public float RangeDamageMultiplier;
    public float SizePenetration;
    public float RangePenetrationMultiplier;
    public float ComponentDamage;
    public float ArmorPenetration;
    public HitResult Result;
    public float ReportedDamage;
    public bool Hit;
    public bool Destroyed;
    public bool Completed;
    public bool HasTargetBounds;
    public Bounds TargetBounds;
    public int DroppedSteps;
    public List<DamageTraceStep> Steps = [];
    public RayDamageOutcome Outcome => ReportedDamage > 0 ? RayDamageOutcome.Damage : Hit ? RayDamageOutcome.NoDamage : RayDamageOutcome.Miss;

    public void AddStep(DamageTraceStep step)
    {
        if (Steps.Count < MaxSteps)
            Steps.Add(step);
        else
            DroppedSteps++;
    }

    public string Describe() =>
        $"Missile {MissileId} / ray {RayIndex}\n" +
        $"{Kind}: {Target} / {Result}{(Completed ? "" : " (incomplete)")}\n" +
        $"Range {Range * 10f:N1} m; curve input {Range:N3} world units; socket weight {SocketSize:N3}\n" +
        $"Component curve: {SizeDamage:N3} x {RangeDamageMultiplier:N4} = {SizeDamage * RangeDamageMultiplier:N3} HP\n" +
        $"Penetration curve: {SizePenetration:N3} x {RangePenetrationMultiplier:N4} = {SizePenetration * RangePenetrationMultiplier:N3} cm\n" +
        $"Dealer characteristics: {ComponentDamage:N3} HP / {ArmorPenetration:N3} cm\n" +
        $"Reported damage: {ReportedDamage:N3} HP; target destroyed: {Destroyed}\n" +
        $"Collider: {Collider}\nDamager: {Damager}\n{Settings}";
}

[Serializable]
internal class DamageTraceStep(string description)
{
    public string Description = description;
    public bool HasSegment;
    public Vector3 Start;
    public Vector3 End;
    public int Intersections;
    public bool DamageEvaluated;
    public float ReportedDamage;
    public HitResult Result;

    public RayDamageOutcome Outcome => ReportedDamage > 0 ? RayDamageOutcome.Damage : Intersections > 0 ? RayDamageOutcome.NoDamage : RayDamageOutcome.Miss;

    public string Describe() => !HasSegment ? Description : Description +
        $"\nReported internal-ray damage: {ReportedDamage:N3} HP; " +
        (DamageEvaluated ? $"result {Result}." : Intersections == 0 ? "miss." : "no component damage applied.");
}

internal static class SalvoTraceGeometry
{
    public static float DistanceToSegment(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 delta = end - start;
        float along = delta.sqrMagnitude > 0f ? Mathf.Clamp01(Vector2.Dot(point - start, delta) / delta.sqrMagnitude) : 0f;
        return Vector2.Distance(point, start + along * delta);
    }
}
