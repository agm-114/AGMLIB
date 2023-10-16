using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum WeaponRole
{
    Unlocked = -1,
    Offensive = 0,
    Defensive = 1
}
public enum LaunchType
{
    Unlocked = -1,
    Hot = 0,
    Cold = 1
}
public enum TargetLost
{
    Unlocked = -1,
    Resume = 0,
    SelfDestruct = 1
}
public enum Terminal
{
    Unlocked = -1,
    None = 0,
    Weave = 1,
    Corkscrew = 2
}
public enum Guidance
{
    Unlocked = -1,
    MinimumAngle = 0,
    FreeApproach = 1
}
public interface IModular
{
    public List<ScriptableObject> Modules { get; }

}

public interface ILimited
{
    public Dictionary<string, int> RestrictedOptions { get; }
    //public Dictionary<string, bool> LockedOptions { get; }
}

