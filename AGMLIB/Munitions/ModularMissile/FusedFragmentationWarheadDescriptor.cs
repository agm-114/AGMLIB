using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime;

[CreateAssetMenu(fileName = "New Missile Warhead", menuName = "Nebulous/Missiles/Warhead/FusedFragmentation")]
public class FusedFragmentationWarheadDescriptor : FragmentationWarheadDescriptor, IFuse
{
    public Vector3 FuseDimensions = Vector3.one;

    public override void FinalSetup(ModularMissile missile)
    {
        missile.AddRuntimeBehaviour<RuntimeMissileWarhead>(this);

        missile.SpawnProximityFuze(FuseDimensions);//Fusedimensions * 10 * (AoeRadius / 10) 100 
    }

}

