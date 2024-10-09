public class FixedCellLauncherComponent : CellLauncherComponent, IFixedWeapon
{
    //public override int Capacity => 10;

    //private BaseCellMissileMagazine _missiles => Common.GetVal<BaseCellMissileMagazine>(this, "_missiles");

    public bool NeedsTightPID => true;

    public Direction FacingDirection => base.Socket.MyHull.MyShip.transform.InverseTransformDirection(base.transform.up).normalized.RemoveTransients().ClosestSide();

    public override ComponentSaveData GetSaveData() => new CellLauncherData();



    /*
    public override void LoadSaveData(ComponentSaveData data)
    {
        base.LoadSaveData(new CellLauncherData());
    }
    */

    /*
    protected override BaseCellMissileMagazine MakeInternalMagazine()
    {
        return new MagazineCellMissileMagazine(this, _allCells);
    }
    */

    /*
    List<BulkMagazineComponent> BulkMagazineComponents => this.transform.parent.parent.GetComponentsInChildren<BulkMagazineComponent>().ToList();
    protected override void Start()
    {
        //Slurp();
        base.Start();

    }
    public void Slurp()
    {
        //((MagazineCellMissileMagazine)_missiles).Slurp();

        foreach (BulkMagazineComponent mag in BulkMagazineComponents.Where(a => a?.Magazines != null))
        {
            foreach (IMagazine submag in mag.Magazines)
            {
                if (this.IsAmmoCompatible(submag.AmmoType))
                {
                    //Debug.LogError("Slurp " + submag.AmmoType.MunitionName);
                    //IMagazineProvider _provider = (this as IConfigurableMagazineLoadout).GetMagazineProvider<IMagazineProvider>();
                    
                    //_provider.AddToMagazine(submag.AmmoType, 1);
                    //this.Group as 
                    //this.add
                    //this.add
                    //_missiles.AddToMagazineInternal(submag);
                    //this.magaiz.AddToMagazine(submag.AmmoType, (uint)submag.Quantity);

                    //mag.RemoveAllFromMagazine(submag.AmmoType);
                }
                //mag.RemoveAllFromMagazine(submag.AmmoType);
            }

        }
    }
    */
}
