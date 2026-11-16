using Modding;
using UnityEngine;

namespace NebulousCli;

public class EntryPoint : IModEntryPoint
{
    private static BridgeDriver? _driver;

    public void PreLoad() { }

    public void PostLoad()
    {
        if (_driver != null || Application.isBatchMode)
            return;

        var host = new GameObject("Nebulous Game Bridge");
        try
        {
            var driver = host.AddComponent<BridgeDriver>();
            driver.StartSession();
            UnityEngine.Object.DontDestroyOnLoad(host);
            _driver = driver;
        }
        catch
        {
            UnityEngine.Object.Destroy(host);
            throw;
        }
    }
}
