using Modding;
using UnityEngine;

namespace MetaLibVersionLog
{
    public class MetaLibVersionLog : IModEntryPoint
    {
        public void PreLoad()
        {
            Debug.Log("Loading MetaLib...");
        }
        public void PostLoad()
        {
            Debug.Log("Loaded MetaLib v25_f5, included with AGMLIB!.");
        }
    }
}
