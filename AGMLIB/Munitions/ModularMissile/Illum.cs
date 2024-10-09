using Munitions.ModularMissiles.Descriptors.Support;

[HarmonyPatch(typeof(JammerSupportDescriptor), nameof(JammerSupportDescriptor.SpawnJammingEffect))]
class JammerSupportDescriptorSpawnJammingEffect : MonoBehaviour
{
    static void Prefix(JammerSupportDescriptor __instance, ActiveEWarEffect ____effectPrefab)
    {
        ActiveEWarEffect _effectPrefab = ____effectPrefab;
        if (_effectPrefab != null)
            return;

        JammerSupportDescriptor source = BundleManager.Instance.AllMissileComponents.Where(desc => desc is JammerSupportDescriptor jammer).Select(desc => (JammerSupportDescriptor)desc).First();
        
        _effectPrefab = Common.GetVal<ActiveEWarEffect>(source, "_effectPrefab");
        SignatureType _sigType = Common.GetVal<SignatureType>(_effectPrefab, "_sigType");

        if (__instance is ModularJammerSupportDescriptor jammer)
        {
            //Debug.LogError("Setting Up Illums " + _sigType);
            
            IReadOnlyCollection<HullComponent> hullComponents = BundleManager.Instance.AllComponents;

   

            HullComponent goodewar = hullComponents.FirstOrDefault(x => x.SaveKey == jammer.PrefabName);
            //Debug.LogError("Found Target " + goodewar.SaveKey);
            RezFollowingMuzzle goodmuzzel = goodewar.gameObject.GetComponentInChildren<RezFollowingMuzzle>();
            GameObject prefab = Common.GetVal<GameObject>(goodmuzzel, "_followingPrefab");
            if (prefab == null)
                return;
            SensorIlluminator illum = prefab.GetComponent<SensorIlluminator>();

            _effectPrefab = illum;
            /*
            _effectPrefab = NetworkObjectPooler.Instance.GetNextOrNew<ActiveEWarEffect>(_effectPrefab.gameObject, Vector3.one * 10000, Quaternion.identity);
            SensorIlluminator illum = _effectPrefab.gameObject.AddComponent<SensorIlluminator>();

            MeshFilter _areaDisplay         = Common.SetVal(illum, "_areaDisplay", Common.GetVal<MeshFilter>(_effectPrefab, "_areaDisplay"));
            MeshRenderer _areaRenderer      = Common.SetVal(illum, "_areaDisplay", Common.GetVal<MeshRenderer>(_effectPrefab, "_areaRenderer"));
            Mesh _omnidirectionalSphereMesh = Common.SetVal(illum, "_areaDisplay", Common.GetVal<Mesh>(_effectPrefab, "_omnidirectionalSphereMesh"));
            DestroyImmediate(_effectPrefab);
            _effectPrefab = illum;
            */
        }

        Common.SetVal(__instance, "_sigType", SignatureType.Radar);
        Common.SetVal(__instance, "_effectPrefab", _effectPrefab);
    }

}
 