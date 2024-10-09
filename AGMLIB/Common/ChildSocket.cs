using Object = UnityEngine.Object;

public class ChildSocket : MonoBehaviour
{
    public List<string> Components = new();
    private readonly List<GameObject> _children = new();
    private HullSocket _hullsocket;
    void Start()
    {
        _hullsocket = gameObject.GetComponentInParent<HullSocket>();
        if(_hullsocket == null)
            _hullsocket = gameObject.GetComponent<HullSocket>();

        foreach (HullComponent componentPrefab in BundleManager.Instance.AllComponents)
        {
            GameObject socketObject = Object.Instantiate(new GameObject(), _hullsocket.transform);
            HullSocket newsocket = socketObject.AddComponent<HullSocket>();
            Common.SetVal(newsocket, "_key", ShortGuid.NewGuid().ToString());
            Common.SetVal(gameObject.GetComponentInParent<BaseHull>(), "_socketLookup", null);
            //socketObject.AddComponent<SocketFilters>().Whitelisteverything = true;
            newsocket.SetComponent(componentPrefab);

            //GameObject gameObject = Object.Instantiate(componentPrefab.gameObject, _hullsocket.transform);
            //gameObject.name = componentPrefab.name;
            //HullComponent _component = gameObject.GetComponent<HullComponent>();
            //_component.SetSocket(newsocket);
            //gameObject.transform.SetParent(newsocket.transform);
            //gameObject.transform.localPosition = newsocket.AttachPoint / 10f;
            //_children.Add(.gameObject);
            _children.Add(socketObject);

        }

        /*
        foreach (string componentname in Components)
        {
            HullComponent componentPrefab = BundleManager.Instance.GetHullComponent(componentname);
        }
        */
    }

    void FixedUpdate()
    {
    }

    void OnDestroy()
    {
        foreach(GameObject gameObject in _children)
        {
            Destroy(gameObject);
        }
    }
}

