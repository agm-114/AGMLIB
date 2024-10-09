using static Utility.GameColors;

public class SimpleDiscount : MonoBehaviour
{
    public float Cost = 0.5f;
    public virtual float Multiplier(float amount) => Cost;
    public static HullSocket CostingSocket = null;
    public static HullSocket UISocket = null;

    public static float ListCalc(HullComponent component, int existingCount)
    {
        HullSocket currentsocket = Common.GetVal<HullSocket>(component, "_socket") ?? CostingSocket ?? UISocket ;
        //Debug.LogError("Discount Calc for " + existingCount + "th " + component.ComponentName);
        List<SimpleDiscount> discounts = component?.GetComponents<SimpleDiscount>()?.ToList() ?? new();
        if (currentsocket != null)
            discounts.AddRange(currentsocket?.GetComponents<SimpleDiscount>()?.ToList() ?? new());

        //Debug.LogError("Discount Size Preappend " + discounts.Count);
        //discounts = discounts.Append(new SimpleDiscount()).ToList();

        if (discounts.Count == 0) { return 1; }
        //Debug.LogError(discounts.Count + " Discounts provided " + discounts.ConvertAll(x => x.Multiplier(existingCount)).Aggregate(1f, (a, x) => a * x));
        return discounts.ConvertAll(x => x.Multiplier(existingCount)).Aggregate(1f, (a, x) => a * x);
    } 

}

public class Discount : SimpleDiscount
{

    public int Modulo = 2;
    public int Offset = -1;
    

    public override float Multiplier(float amount)
    {
        if (Offset < 0)
            Offset = Modulo + Offset;
        return amount % Modulo == Offset ? Cost : 1;
    }
    /*
    public float TotalMultiplier(float amount)
    {
        if (amount == 0)
            return amount;
        
        int discountedcomps = (int)(amount / Modulo);
        return ((discountedcomps * Cost) + (amount - discountedcomps)) / amount;
    }*/

}



[HarmonyPatch(typeof(HullComponent), nameof(HullComponent.GetPointCost))]
class HullComponentGetPointCost
{
    static void Prefix(HullComponent __instance, int existingCount, HullComponentCostCalculator saveInfo = null)
    {
        Postfix();
        if (saveInfo == null)
            return;
        SimpleDiscount.CostingSocket = Common.GetVal<HullSocket>(saveInfo, "_socket");
    }


    static void Postfix()
    {

        SimpleDiscount.CostingSocket = null;

    }
}


[HarmonyPatch(typeof(HullComponent), "GetBasePointCost")]
class HullComponentGetBasePointCost
{
    static void Postfix(HullComponent __instance, int existingCount, ref int __result)
    {

        //Debug.LogError("Start cost " + __result);
        //List<float> test = discounts.ConvertAll<float>(x => x.Multiplier(existingCount));
        //foreach(float val in test)
        //{
        //    //Debug.LogError("Discount " + val);
        //}


        float discount = Discount.ListCalc(__instance, existingCount);

        if (discount == 1) { return; }
        //Debug.LogError("nonzero");
        __result = (int)((__result) * Discount.ListCalc(__instance, existingCount));
        //Debug.LogError("End Cost " + __result);

    }
}




[HarmonyPatch(typeof(HullComponent), nameof(HullComponent.GetFormattedSubtitle))]
class HullComponentGetFormattedSubtitle
{
    static void Postfix(HullComponent __instance, int currentlyInstalled, ref string __result)
    {
        float discount = Discount.ListCalc(__instance, currentlyInstalled);
        if (discount == 1) { return; }
        if(discount < 1)
        __result += $" [<color={GetTextColor(ColorName.Green)}>{1 - discount:P0}</color> OFF]";
        if (discount > 1)
            __result += $" [<color={GetTextColor(ColorName.Red)}>{1 - discount:P0}</color> Extra]";
        //Debug.LogError("End Cost " + __result);

    }
}
