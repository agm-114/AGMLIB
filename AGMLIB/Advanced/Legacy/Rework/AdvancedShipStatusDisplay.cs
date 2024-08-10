using Game.UI;
using Game.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedShipStatusDisplay : ShipStatusDisplay
{
    // Start is called before the first frame update
    public ShipController GetShip() => _ship;//This class has no reason to exist
}
