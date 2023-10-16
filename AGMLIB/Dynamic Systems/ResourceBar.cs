using Game.UI;
using Game.Units;
using Ships;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


public class ResourceBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Gradient colors;
    public Image target;
    private AdvancedShipStatusDisplay display;
    public float clamp = 1;
    //public ShipStatusDisplayPart fallback;

    [HideInInspector]
    public ResourceComponent ResourceComponent;
    [HideInInspector]
    public GameObject shipGO;
    RectTransform rectTransform;
    private float _glow;
    [HideInInspector]
    public ShipController ShipController;
    private int _searchtimer = 100;
    private FriendlyShipItem _shipBar;
    private TooltipAttribute _tooltip;

    void Start()
    {
        
        try
        {
            rectTransform = transform as RectTransform;
            if (target == null)
                target = gameObject.GetComponent<Image>();
            if (display == null)
                display = gameObject.GetComponentInParent<AdvancedShipStatusDisplay>();
            colors.mode = GradientMode.Blend;

            if (display.GetShip() == null || display.GetShip().gameObject == null)
            {
                return;
            }
            shipGO = display.GetShip().gameObject;

            if (ResourceComponent == null)
                ResourceComponent = shipGO.GetComponent<ResourceComponent>();
            if (ResourceComponent == null)
                ResourceComponent = display.GetShip().gameObject.GetComponentInChildren<ResourceComponent>();
            if (ResourceComponent == null)
                ResourceComponent = display.GetShip().gameObject.GetComponentInParent<ResourceComponent>();
        }
        catch (System.Exception)
        {

        }
        _tooltip = gameObject.GetComponentInChildren<TooltipAttribute>();
        
        if (_tooltip != null)
            Debug.LogError("TOOLTIP FOUND");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _searchtimer--;
        if (display == null && _shipBar == null && _searchtimer <= 0 && ShipController != null)
        {
            //Debug.LogError("Searching");

            FriendlyShipItem[] friendlyShipItems = FindObjectsOfType<FriendlyShipItem>();

            foreach (FriendlyShipItem bar in friendlyShipItems)
            {
                CrewStatusIcon crewicon = bar.GetComponentInChildren<CrewStatusIcon>();
                    
                if (crewicon != null && bar.Ship == ShipController)
                {
                    _shipBar = bar;
                    rectTransform.root.SetParent((crewicon.transform as RectTransform).parent);
                }

                //    ShipController _ship;
            }
            if (friendlyShipItems.Length > 0 && _shipBar == null)
                Destroy(this);
            _searchtimer = 1000;    
        }

        if (display == null || display.isActiveAndEnabled)
        {
            //Debug.LogError("Calculationg Glow");
            if (ResourceComponent != null && ResourceComponent.isActiveAndEnabled)
                _glow = ResourceComponent.fillpercentage;
            //Debug.LogError("Glow is " + glow);
            target.color = colors.Evaluate(_glow);
            //Debug.LogError("Color is " + colors.Evaluate(glow));
            rectTransform.anchorMax = new Vector2(Mathf.Min(clamp, _glow), 1);
            rectTransform.offsetMax.Set(0, 0);
            rectTransform.offsetMin.Set(0, 0);
            //Debug.LogError("Transform is " + Mathf.Min(1, glow));
        }

    }

    //Debug.LogError("No parents");
    //CrewStatusIcon[] crewicons = FindObjectsOfType<CrewStatusIcon>();
    //ShipGlanceBar[] glanceBars = FindObjectsOfType<ShipGlanceBar>();
    //Debug.LogError(rectTransform.parent.parent.name);
    //Debug.LogError(rectTransform.parent.name);
    //UnityEngine.Component[] components = (crewicons[0].transform as RectTransform).parent.GetComponents(typeof(UnityEngine.Component));
    //foreach (UnityEngine.Component component in components)
    //{
    //    Debug.LogError(component.ToString());
    //}
    //Debug.LogError(rectTransform.parent.parent.name);
    //Debug.LogError("Found " + crewicons.Length + " icons");
    //Debug.LogError("Found " + glanceBars.Length + " bars");
    //Debug.LogError("Found " + friendlyShipItems.Length + " friendlyships");
    //foreach (ShipGlanceBar bar in glanceBars)
    //{
    //    CrewStatusIcon crewicon = bar.GetComponentInChildren<CrewStatusIcon>();
    //    if (crewicon != null && (ShipController)GetPrivateField(bar, "_ship") == ShipController)
    //        rectTransform.parent.parent.SetParent((crewicon.transform as RectTransform).parent);
    //}

    public static object GetPrivateField(object instance, string fieldName)
    {
        static object GetPrivateFieldInternal(object instance, string fieldName, System.Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(instance);
            }
            else if (type.BaseType != null)
            {
                return GetPrivateFieldInternal(instance, fieldName, type.BaseType);
            }
            else
            {
                return null;
            }
        }

        return GetPrivateFieldInternal(instance, fieldName, instance.GetType());
    }
}
