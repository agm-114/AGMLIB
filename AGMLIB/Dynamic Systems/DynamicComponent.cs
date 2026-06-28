public class DynamicComponent : ActiveSettings
{
    public MonoBehaviour MonoBehaviour;

    protected override void FixedUpdate()
    {

        base.FixedUpdate();
        /*
        if (!active)
        {
            if (DisableWeapons)
                Common.SetVal(ShipController, "_weaponsControl", WeaponsControlStatus.Hold);
        }
        */
        if (active != MonoBehaviour.isActiveAndEnabled)
        {
            MonoBehaviour.enabled = active;
            if (MonoBehaviour is BaseSignature sig)
            {
                Common.RunFunc(sig, "FireSignatureChangedEvent", []);
                //sig.OnSignatureChanged.Invoke();
            }
        }


    }
    //public enum ForceMode { Start, Impulse, Acceleration, Force, VelocityChange };
}