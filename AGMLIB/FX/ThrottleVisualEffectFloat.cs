using System;
using UnityEngine;
using UnityEngine.VFX;
using Utility;

namespace Lib.FX
{
    [DisallowMultipleComponent]
    public class ThrottleVisualEffectFloat : ShipControllerBehaviour
    {
        [SerializeField]
        private List<VisualEffect> _effects = [];

        [SerializeField]
        private string _propertyName = "max_throttle";

        public float OneThird = 0.33f;
        public float TwoThirds = 0.66f;
        public float Full = 1f;
        public float Flank = 1.5f;
        public float FlightQuarters = 1f;

        private float CurrentValue => ShipController?.Throttle switch
        {
            MovementSpeed.OneThird => OneThird,
            MovementSpeed.TwoThirds => TwoThirds,
            MovementSpeed.Full => Full,
            MovementSpeed.Flank => Flank,
            MovementSpeed.FlightQuarters => FlightQuarters,
            _ => Full
        };

        private float _lastValue = float.MinValue;

        private void Update() => ApplyCurrentValue();

        private void ApplyCurrentValue()
        {
            if (ShipController == null || _lastValue.Equals(CurrentValue))
                return;

            _effects.ForEach(effect => effect.SetFloat(_propertyName, CurrentValue));
            _lastValue = CurrentValue;
        }

    }
}
