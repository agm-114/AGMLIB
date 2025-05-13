[HarmonyPatch(typeof(BoxSignature), "Awake")]
class BoxSignatureAwake
{

    static void Prefix(BoxSignatureAwake __instance)
    {
        BoxCollider _collider = Common.GetVal<BoxCollider>(__instance, "_collider");
        if (_collider == null)
        {
            return;
        }
        Bounds _sigSize = new Bounds(Vector3.zero, _collider.bounds.size);
        Common.SetVal(__instance, "_sigSize", _sigSize);
        _collider.enabled = false;
    }





}