[CreateAssetMenu(fileName = "New Faction", menuName = "Nebulous/Game Systems/SalvageRules")]
public class SalvageRules : ScriptableObject
{
    public AnimationCurve FriendlySalvageCurve = new AnimationCurve(new Keyframe(0f, 0.5f), new Keyframe(1f, 1f));
    public AnimationCurve HostileSalvageCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

}

[CreateAssetMenu(fileName = "New Faction", menuName = "Nebulous/Game Systems/InventoryRules")]
public class InventoryRules : ScriptableObject
{
    public int DefaultAmount = 10;
}