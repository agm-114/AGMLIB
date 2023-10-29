using Ships;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Munitions;
using System.Collections;

public class MovementTarget : MonoBehaviour
{
    public static HashSet<MovementTarget> Instances = new ();
    [SerializeField]
    protected float _repairrate = 0;
    [SerializeField]
    protected float _repairdistance = 0;
    [SerializeField]
    protected float _evasivedistance = 10;
    [SerializeField]
    protected float _evasiondelay = 10;
    [SerializeField]
    protected bool _permanant = true;
    [SerializeField]
    protected GameObject _previewprefab;

    [SerializeField]
    public int Priority = 0;
    [HideInInspector]
    public ICraft Craft = null;
    [SerializeField]
    protected List<HullSocket> hullSockets = new();
    public FormationManager FormationManager => gameObject.GetComponentInParent<FormationManager>();
    public List<HullSocket> FilledSockets => hullSockets.Where(hullSocket => hullSocket.Component != null).ToList();
    public List<HullComponent> Components => FilledSockets.ConvertAll(hullSocket => hullSocket.Component);
    protected string _filter = "";
    public bool PermantTarget => _permanant;
    public bool RepairTarget => _repairrate > 0;
    public float RepairRate => _repairrate;
    
    public bool CanRepair => RepairTarget && Vector3.Distance(transform.position, Craft.Rigidbody.position) < _repairdistance;
    [Range(0.0f, 1f)]
    [SerializeField]
    protected float _rtbthreshold = 1;
    public float Rtbthreshold => _rtbthreshold;
    protected GameObject _preview;
    protected Vector3 _offset = Vector3.zero;

    void Awake()
    {
        Instances.Add(this);
    }
    void OnDestroy()
    {
        Instances.Remove(this);
    }
    void FixedUpdate()
    {
        //_previewprefab = GameObject.CreatePrimitive(PrimitiveType.Cube); ;
        Update();
        foreach (HullComponent hullComponent in Components)
        {
            int targethealth = (int)Math.Round(hullComponent.MaxHealth * Craft?.HealthPercentage ?? 0f);
            int currenthealth = (int)hullComponent.CurrentHealth;
            if (targethealth != currenthealth)
            {
                if (hullComponent.CurrentHealth < targethealth)
                    hullComponent.DoHeal(1);
                else if (hullComponent.CurrentHealth > targethealth)
                    hullComponent.DoDamage(1, null, null);
                Common.SetVal(hullComponent, "_minHealthReached", hullComponent.CurrentHealth);
                FormationManager.ShipController?.MarkAsDamaged();
            }
        }
    }
    private void Update()
    {
        foreach (HullSocket hullSocket in FilledSockets)
        {
            if (Craft != null)
            {
                hullSocket.transform.position = Craft.SocketTransform.position;
                hullSocket.transform.rotation = Craft.SocketTransform.rotation;
            }
            else
                hullSocket.transform.position = new Vector3(1000000, 1000000, 1000000);
        }
    }
    public bool Valid(ICraft craft, string filter)
    {
        if (_filter != filter || Craft != null || craft == null)
            return false;
        if (craft is LookaheadMunition missile)
            return FormationManager.ShipController.GetIFF(missile.OwnedBy) == Game.IFF.Mine;
        return true;
    }
    public void EditorVisuals()
    {
        if (_preview == null && _previewprefab != null && FormationManager.EditorShipController != null)
        {
            //_preview = Instantiate(_previewprefab, transform);
        }
    }
}
