using Game.Units;
using Ships;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class HoldingTargets : ShipState
{
    // Start is called before the first frame update

    [HideInInspector]
    public Craft Craft = null;

    public String filter = "";

}
