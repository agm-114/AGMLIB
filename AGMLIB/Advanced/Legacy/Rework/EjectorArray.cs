//using Game.Ships;

public class EjectorArray : MonoBehaviour
{
    //private int count;
    //public float offset = 1;
    //private float time = 0.0f;
    private int index = 0;

    //public float interpolationPeriod = 1f;
    //public TubeLauncherComponent laucher;
    public List<MissileEjector> linkedejectors;
    public int launcherindex = 0;
    public TubeLauncherComponent tubelauncher;
    public CellLauncherComponent celllauncher;
    public List<MissileEjector> leftejectors;
    public List<MissileEjector> rightejectors;
    // Start is called before the first frame update
    private RotationMonitor rotation;
    private Vector3 dvec;
    private Vector3 lvec;
    void Start()
    {

    }

    RotationMonitor Findmonitor(GameObject searchobject)
    {
        RotationMonitor[] monitors = searchobject.GetComponentsInChildren<RotationMonitor>();
        if (monitors.Length > 0)
            return monitors[0];

        //Debug.Log("No Monitor found");
        return null;
    }

    // Update is called once per frame

    void Update()
    {
        //time += Time.deltaTime;
        //rotation = findmonitor(gorotation);
        //if (ejector = null)
        //    ejector = (MissileEjector)GetPrivateField(launcher, "_ejector");

        launcherindex = tubelauncher != null ? Common.GetVal<int>(tubelauncher, "_currentShot") : index;

        if (tubelauncher == null)
        {
            if (rotation == null)
                rotation = Findmonitor(gameObject.transform.parent.parent.gameObject);
            if (rotation == null)
                return;
            dvec = rotation.angle;
            lvec = gameObject.transform.eulerAngles;
            float dev = (lvec.y - dvec.y) % 360;

            for (index = 0; index < linkedejectors.Count; index++)
            {
                if (dev < 0)
                {
                    //LogError
                    //Debug.LogError("Right Fire Mission Tube: " + index + " Dev:" + dev + " lvec.y: " + lvec.y + " dvec.y " + dvec.y);
                    linkedejectors[index].gameObject.transform.position = rightejectors[index % rightejectors.Capacity].gameObject.transform.position;
                    linkedejectors[index].gameObject.transform.rotation = rightejectors[index % rightejectors.Capacity].gameObject.transform.rotation;
                }
                else
                {
                    //Debug.LogError("Left Fire Mission Tube: " + index + " Dev:" + dev + " lvec.y: " + lvec.y + " dvec.y " + dvec.y);
                    linkedejectors[index].gameObject.transform.position = leftejectors[index % leftejectors.Capacity].gameObject.transform.position;
                    linkedejectors[index].gameObject.transform.rotation = leftejectors[index % leftejectors.Capacity].gameObject.transform.rotation;
                }
            }

            //transform.position = transform.parent.position;
            //transform.position += new Vector3(0, 0, offset * index);
            //ejector = ejectors[index];
            //base.transform;
            //SetPrivateField(laucher, "_ejector", ejectors[index]);
            index = 0;
        }
        else if (index != launcherindex)
        {
            //index = launcherindex;

            if (rotation == null)
                rotation = Findmonitor(gameObject.transform.parent.parent.gameObject);
            dvec = rotation.angle;
            lvec = gameObject.transform.eulerAngles;
            float dev = (lvec.y - dvec.y) % 360;
            if (dev < 0)
            {
                //LogError
                //Debug.LogError("Right Fire Mission Tube: " + index + " Dev:" + dev + " lvec.y: " + lvec.y + " dvec.y " + dvec.y);
                linkedejectors[index % linkedejectors.Count].gameObject.transform.position = rightejectors[index % rightejectors.Capacity].gameObject.transform.position;
                linkedejectors[index % linkedejectors.Count].gameObject.transform.rotation = rightejectors[index % rightejectors.Capacity].gameObject.transform.rotation;
            }
            else
            {
                //Debug.LogError("Left Fire Mission Tube: " + index + " Dev:" + dev + " lvec.y: " + lvec.y + " dvec.y " + dvec.y);
                linkedejectors[index % linkedejectors.Count].gameObject.transform.position = leftejectors[index % leftejectors.Capacity].gameObject.transform.position;
                linkedejectors[index % linkedejectors.Count].gameObject.transform.rotation = leftejectors[index % leftejectors.Capacity].gameObject.transform.rotation;
            }

            //transform.position = transform.parent.position;
            //transform.position += new Vector3(0, 0, offset * index);
            //ejector = ejectors[index];
            //base.transform;
            //SetPrivateField(laucher, "_ejector", ejectors[index]);
            index = launcherindex;
        }
    }
}
/*

`
time = time - interpolationPeriod;

*/