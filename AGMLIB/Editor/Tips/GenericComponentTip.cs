using FleetEditor.Tips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Editor.Tips
{
        
    public class GenericComponentTip : ShipDesignTip
    {
        protected override DesignWarning GetTipInternal(Ship ship, ParameterizedDesignTip.Param[] parameters)
        {
            if (parameters == null)
            {
                return null;
            }
            List<string> socketKeys = (from x in parameters
                                       where x.ParamName == "SocketKey"
                                       select x.StrParam).ToList();
            List<string> weaponKeys = (from x in parameters
                                       where x.ParamName == "PartKey"
                                       select x.StrParam).ToList();
            if (ship.Hull.AllSockets.Where((HullSocket x) => socketKeys.Count == 0 || socketKeys.Contains(x.Key)).Any((HullSocket x) => x.Component != null && weaponKeys.Contains(x.Component.SaveKey)))
            {
                return ShipDesignWarning.Tip(parameters.FirstOrDefault((ParameterizedDesignTip.Param x) => x.ParamName == "Title").StrParam, parameters.FirstOrDefault((ParameterizedDesignTip.Param x) => x.ParamName == "Message").StrParam, ship);
            }
            return null;
        }
    }
}
