using Game.Intel;
using Game;
using Munitions;
using UnityEngine;
using Game.Units;
using Mirror;
using Modding;
using Debug = UnityEngine.Debug;
using HarmonyLib;
using Ships;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;
using UnityEngine.PlayerLoop;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using static Game.WaypointPath;
using Pixelplacement;
using UnityEngine.UI.Extensions;
using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime;
using System.Linq;
using Shapes;
using Bundles;
using Game.Reports;
using Munitions.InstancedDamagers;
using Random = UnityEngine.Random;
using System.Runtime.InteropServices;
using static UI.ModalButtonList;
using System.Diagnostics;
using System.Drawing;
using Effects;
using Game;
using Game.Reports;
using Game.Units;



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

