using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// The playstyle that can be selected in the world creation screen
    /// </summary>
    public enum EnumPlayStyle
    {
        WildernessSurvival = 0,
        SurviveAndBuild = 1,
        SurviveAndAutomate = 2,
        CreativeBuilding = 3
    }

   /* public class Playstyle
    {
        public string Code;

        public string[] IncludeMods;

        public string[] ExcludeMods;

        public JsonObject Attributes;
    }*/
}
