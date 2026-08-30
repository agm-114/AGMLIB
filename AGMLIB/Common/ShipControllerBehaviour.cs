using Game.Units;
using UnityEngine;

namespace Lib
{
    public abstract class ShipControllerBehaviour : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Optional explicit ship controller. When empty, the component finds one on the same ship.")]
        private ShipController? _shipController = null;

        protected ShipController? ShipController => ResolveShipController();

        protected ShipController? ResolveShipController()
        {
            if (_shipController != null)
            {
                return _shipController;
            }

            _shipController = GetComponent<ShipController>()
                ?? GetComponentInParent<ShipController>()
                ?? transform.root.GetComponentInChildren<ShipController>(true);
            return _shipController;
        }
    }
}
