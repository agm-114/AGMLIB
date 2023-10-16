using Game.Orders.Tasks;
using Game.Units;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
public class FormationManager : ShipState
{
    [SerializeField]
    protected bool  _dedicatedformation = true;
    public GameObject movementtargetroot;
    protected Rigidbody rigidbody;
    
    protected List<MovementTarget> _targets;
    protected Vector3 targetpos;
    protected override void Awake()
    {
        targetpos = transform.position;
        base.Awake();
        if(movementtargetroot == null)
            movementtargetroot = gameObject;
        _targets = movementtargetroot.GetComponentsInChildren<MovementTarget>().ToList();
        //dedicatedformation = true;
        rigidbody = ShipController.GetComponent<Rigidbody>();
    }
    protected void FixedUpdate()
    {
        //_targets.Where(target => target.Components.Count > 0).ToList().ForEach(target => target.EditorVisuals());
        if (!_dedicatedformation || ShipController == null || EditorShipController != null)
            return;
        NavigationTask task = Common.GetVal<NavigationTask>(ShipController, "_navOrder");
        if (task is KeepFormationTask keepFormationTask)
        {
            ShipController _guide = Common.GetVal<ShipController>(keepFormationTask, "_guide");
            if (_guide.FormStyle == FormationStyle.Relative)
                targetpos = _guide.transform.position + _guide.transform.TransformDirection(Common.GetVal<Vector3>(keepFormationTask, "_relativePosition"));
            targetpos = _guide.transform.position + Common.GetVal<Vector3>(keepFormationTask, "_relativePosition");
        }
        else if (task is MoveToTask moveToTask && moveToTask.Path.Count > 0)
            targetpos = moveToTask.Path.First();
        else
            targetpos = ShipController?.CurrentNavTarget ?? targetpos;
        movementtargetroot.transform.position = targetpos;
        movementtargetroot.transform.rotation = rigidbody?.gameObject.transform.rotation ?? new();
        List<ICraft> crafts = _targets.Where(target => target.Craft != null).ConvertAll(target => target.Craft);
        List<Rigidbody> rigidbodies = crafts.ConvertAll(target => target.Rigidbody);
        if (rigidbodies.Count() > 0)
        {
            Vector3 centroid = rigidbody.position * rigidbodies.Count() * 10;//shiprigidbody.position;
            Vector3 averagevelocity = rigidbody.velocity * rigidbodies.Count() * 10;//shiprigidbody.position;
            foreach (Rigidbody strikeCraft in rigidbodies)
            {
                averagevelocity += strikeCraft.velocity;
                centroid += strikeCraft.transform.position;
            }
            //UnityEngine.Debug.Log("Centering Value " + centroid + " / " + _rigidbodies.Count());
            averagevelocity /= (rigidbodies.Count() * 11);
            centroid /= (rigidbodies.Count() * 11);
            rigidbody.velocity = averagevelocity;
            rigidbody.MovePosition(centroid);
            float distance = Vector3.Distance(centroid, targetpos);
            foreach(ICraft icraft in crafts)
                icraft.SetLeadFactor(rigidbody.position + (icraft.Target.transform.position - movementtargetroot.transform.position), ShipController.MoveStyle == MovementStyle.Evasive);;
        }
    }
}
