public class CraftWeaponSettings : MonoBehaviour
{
    [Tooltip("If true, this weapon will not fire unless there is a magazine capable of feeding it ammo.")]
    [SerializeField]
    private bool _requireExternalAmmoFeed = false;
}