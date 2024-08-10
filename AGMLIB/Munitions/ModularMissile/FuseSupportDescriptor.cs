using Munitions.ModularMissiles.Descriptors;
using Munitions.ModularMissiles;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Munitions.ModularMissiles.Descriptors.Support;
using Munitions.ModularMissiles.Runtime;

public interface IFuse
{
    //public Vector3 FuseDimensions { get; }
}

[CreateAssetMenu(fileName = "New Missile Fuse", menuName = "Nebulous/Missiles/Support/Generic Fuse")]
public class FuseSupportDescriptor : BaseSupportDescriptor, IFuse
{

    public MissileSocketType SocketType = MissileSocketType.Support;
    public Vector3 FuseDimensions = Vector3.one;
    public override MissileSocketType FitsSocketType => SocketType;
    public override void FinalSetup(ModularMissile missile)
    {
        base.FinalSetup(missile);
        missile.SpawnProximityFuze(FuseDimensions);//Fusedimensions * 10 * (AoeRadius / 10) 100 
    }
}