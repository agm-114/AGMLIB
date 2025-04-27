using UnityEngine;



public class ResizingLineBeamMuzzleEffects : LineBeamMuzzleEffects
{
    public ShaderMaterial LineMaterial = null;
    public bool PulseAudio = false;

    protected override void Awake()
    {
        if (LineMaterial != null)
        {
            _beam.material = LineMaterial.GetMaterial();
        }
        if (_beam.useWorldSpace)
            Common.Trace("Worldspace beams may cause issues");

        base.Awake();
    }

    protected override void HandleBeamLengthChanged()
    {
        base.HandleBeamLengthChanged();
        if (_beam == null || _beam.positionCount <= 2)
        {
            return;
        }

        int pointCount = _beam.positionCount;
        Vector3 origin = Vector3.zero;
        Vector3 direction = Vector3.forward;

        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1); // normalized distance along the line
            Vector3 point = origin + direction * _beamLength * t;
            _beam.SetPosition(i, point);
        }
    }

    public override void StopEffect()
    {
        //RaycastMuzzle

        AudioSource holder = _fireSoundEffect;
        if (PulseAudio && (_fireSoundEffect?.isPlaying ?? false))
            _fireSoundEffect = null;
        base.StopEffect();
        _fireSoundEffect = holder ?? _fireSoundEffect;
    }

}
public class CurvedResizingLineBeamMuzzleEffects : ResizingLineBeamMuzzleEffects
{
    public float curveHeight = 1.0f; // Height of the curve for the arc



    protected override void HandleBeamLengthChanged()
    {
        base.HandleBeamLengthChanged();

        if (_beam == null || _beam.positionCount < 2 )
        {
            Debug.LogWarning("LineRenderer is not properly set up or target is null.");
            return;
        }

        int pointCount = _beam.positionCount;

        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.forward * _beamLength;

        // Create a control point to curve the beam.
        Vector3 mid = (start + end) / 2;
        Vector3 curveDirection = Vector3.Cross((end - start).normalized, Vector3.up).normalized;
        Vector3 control = mid + curveDirection * curveHeight;

        // Use a quadratic Bézier curve to interpolate positions
        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);
            Vector3 point = Mathf.Pow(1 - t, 2) * start +
                            2 * (1 - t) * t * control +
                            Mathf.Pow(t, 2) * end;
            _beam.SetPosition(i, point);
        }
    }
}
