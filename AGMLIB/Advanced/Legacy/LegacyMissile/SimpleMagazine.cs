
// Nebulous, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Munitions.MissileImpactWarhead
using Random = UnityEngine.Random;

public class SimpleMagazine : MonoBehaviour, IMagazine
{
    //[SerializeField]

    //public Dictionary<LightweightMunitionBase, int> _weightedammo;

    [SerializeField]
    protected List<LightweightMunitionBase> _ammotypes = new(1);
    [SerializeField]
    protected List<float> _weights = new(1);

    public int QuantityNotReserved => throw new NotImplementedException();

    public int QuantityAvailableNotReserved => throw new NotImplementedException();

    public string HeldName => throw new NotImplementedException();

    public int Quantity => throw new NotImplementedException();

    public int QuantityAvailable => throw new NotImplementedException();

    public int PeakQuantity => throw new NotImplementedException();

    public float PercentageAvailable => throw new NotImplementedException();

    public bool IsReinforced => throw new NotImplementedException();

    IMunition IMagazine.AmmoType
    {

        get
        {
            float randomNumber = Random.Range(0f, _weights.Sum());

            for (int i = 0; i < _ammotypes.Count; i++)
                if (randomNumber < _weights[i])
                    return _ammotypes[i];
                else
                    randomNumber -= _weights[i];

            return BundleManager.Instance.GetMunition("Stock/120mm HE Shell");
        }
    }

    event QuantityChanged IQuantityHolder.OnQuantityChanged
    {
        add => throw new NotImplementedException();

        remove => throw new NotImplementedException();
    }

    public int Deposit(uint quantity) => throw new NotImplementedException();
    public int Reserve(uint quantity) => throw new NotImplementedException();
    public int UnReserve(uint quantity) => throw new NotImplementedException();
    public int UpperQuantityBound() => throw new NotImplementedException();

    int IMagazine.Withdraw(uint quantity) => (int)quantity;
}
