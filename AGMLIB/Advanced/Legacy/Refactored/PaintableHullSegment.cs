public class PaintableHullSegment : HullSegment
{
    public string key;
    private Hull Hull;
    private Material OrignalMaterial;
    //private const string _paintMaskParam = "_PaintMask";
    //private Material _bakedMaterial;
    [HideInInspector]
    public Texture2D paintScheme;
    // Start is called before the first frame update
    void Awake()
    {
        Hull = base.GetComponentInParent<Hull>();
        GetBaseMat();
        foreach (PaintScheme painter in Hull.GetComponentsInChildren<PaintScheme>())
        {
            //Debug.LogError("Painter " + painter.gameObject.name + " Key: " + key);
            //if (painter.key == key)
            //{
            //paintScheme = painter.replacementtexture;
            UpdatePaintScheme();
            Destroy(paintScheme);
            //}

        }
    }
    void GetBaseMat() => OrignalMaterial = Common.GetVal<Material>(this, "_originalMaterial");

    void UpdatePaintScheme() => OrignalMaterial.SetTexture(_paintMaskParam, paintScheme);

    public void Quickbakepaint()
    {
        foreach (MeshRenderer meshRenderer in base.gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material.SetTexture(_paintMaskParam, paintScheme);
        }
        UpdatePaintScheme();
    }

    // Update is called once per frame
}
