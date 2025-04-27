using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class ShaderMaterial : MonoComponent
{
    public abstract Material GetMaterial();

    public Material GetLineMaterial(string savekey)
    {
        if (string.IsNullOrEmpty(savekey))
        {
            return null;

        }
        foreach (var hullComponent in BundleManager.Instance.AllComponents)
        {
            Common.Trace(hullComponent.SaveKey);
        }
        HullComponent target = BundleManager.Instance.GetHullComponent(savekey);
        if (target == null)
            Common.Hint($"Failed to load hullcomponent {savekey} {target}");
        LineRenderer lineRenderer = target.gameObject.GetComponentInChildren<LineRenderer>();
        if (lineRenderer == null)
            Common.Hint($"Failed to load line render {savekey} {target}");
        return lineRenderer.material;
    }
}
public abstract class FixedShaderMaterial : ShaderMaterial
{
    public abstract string GetSavekey();

    public override Material GetMaterial()
    {

            Material newmat  = new Material(GetLineMaterial(GetSavekey()));
        int propertyCount = newmat.shader.GetPropertyCount();
        for (int i = 0; i < propertyCount; i++)
        {
            string propertyName = newmat.shader.GetPropertyName(i);
            UnityEngine.Rendering.ShaderPropertyType propertyType = newmat.shader.GetPropertyType(i);
            Common.Trace($"Property {i}: Name = {propertyName}, Type = {propertyType}");
        }
        return newmat;
    }
        

}
public class AuroraMaterial : FixedShaderMaterial
{
    [ColorUsage(true, true)]
    public Color _Color = Color.green;
    public float _PulsesPerSecond = 1;
    [ColorUsage(true, true)]
    public Color _Emission = Color.green;
    [ColorUsage(true, true)] public Color _EmissionColor = Color.green;

    public override string GetSavekey() => "Stock/Mk90 'Aurora' PDT";

    public override Material GetMaterial()
    {
        Material newmat = base.GetMaterial();
        newmat.SetColor("_Color", _Color);
        newmat.SetFloat("_PulsesPerSecond", _PulsesPerSecond);
        newmat.SetColor("_Emission", _Emission);
        newmat.SetColor("_EmissionColor", _EmissionColor);
        return newmat;
    }
}
public class GrazerMaterial : FixedShaderMaterial
{
    public override string GetSavekey() => "Stock/P60 'Grazer' PDT";
}
public class BeamMaterial : FixedShaderMaterial
{
    public override string GetSavekey() => "Stock/Mk610 Beam Turret";
}

public class SaveKeyShaderMaterial: ShaderMaterial
{
    public string SaveKey = "";


    public override Material GetMaterial()
    {
        return GetLineMaterial(SaveKey);
    }


}