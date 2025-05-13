using Mirror;

#pragma warning disable
public class ShipPDTarget : DumbfireRocket, IDamageable
{
    //[HideInInspector]
    //protected override float __maxFlightTime => 99999999;

    //protected override bool _wasSoftkilled => false;  

    public ShipController ShipController;

    [ShowInInspector]
    private string _networkspawnkey = "2c902a31174348c9a34cdfa99c1d4bf4";
    public override Guid SavedNetworkSpawnKey => new(_networkspawnkey);

    protected override void FixedUpdate()
    {
        //base.FixedUpdate();
        if (ShipController != null)
        {
            _body.velocity = ShipController.Velocity;
            Vector3 pos = ShipController.Position;
            //pos.y += 10;
            _body.position = pos;
        }
    }

    new void OnCollisionEnter(Collision collision) => gameObject.GetComponent<Rigidbody>().isKinematic = false;

    HitResult IDamageable.DoDamage(MunitionHitInfo hitInfo, IDamageDealer character, out float damageDone, out bool destroyed)
    {
        destroyed = false;
        //Debug.LogError("Doing " + character.ComponentDamage + " Damage");
        //Debug.LogError("End Stucture " + ShipController.Ship.Hull._structure.CurrentHealth);
        /*
        if (ShipController.Ship.Hull._structure.CurrentHealth > 0)
            ((ISubDamageable)ShipController.Ship.Hull._structure).DoDamage(character.ComponentDamage, character, hitInfo);
        else
        {
            HullPart[] parts = ShipController.gameObject.GetComponentsInChildren<HullPart>();
            try
            {
                parts[UnityEngine.Random.Range(0, parts.Length)].DoDamage(character.ComponentDamage);
            }
            catch (Exception)
            {

            }
        }
        */
        //Debug.LogError("Start Stucture " + ShipController.Ship.Hull._structure.CurrentHealth);
        damageDone = character.ComponentDamage;
        return HitResult.Penetrated;
    }
}