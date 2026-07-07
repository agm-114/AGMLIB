// created by effervescence
using Game;
using Game.Sensors;
using Game.UI;
using Game.EWar;
using Munitions;
using UnityEngine;
using Utility;

public class EWarGuidedDecoy : GuidedDecoy, IEWarHost
{
    Vector3 IEWarHost.Position => base.transform.position;

	ISensorTrackable IEWarHost.Trackable => base._trackable;

	IBoardPieceGroup IEWarHost.LOBGroup => null;

	private void MirrorProcessed()
	{
	}

}
