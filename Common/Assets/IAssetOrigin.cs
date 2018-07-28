using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public interface IAssetOrigin
    {
        string OriginPath { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        void LoadAsset(IAsset asset);

        bool TryLoadAsset(IAsset asset);

        /// <summary>
        /// Returns all assets of the given category which can be found in this origin 
        /// </summary>
        /// <param name="Category"></param>
        /// <returns></returns>
        List<IAsset> GetAssets(AssetCategory Category, bool shouldLoad = true);


        /// <summary>
        /// Returns all assets of the given base location path which can be found in this origin 
        /// </summary>
        /// <param name="baseLocation"></param>
        /// <returns></returns>
        List<IAsset> GetAssets(AssetLocation baseLocation, bool shouldLoad = true);


        /// <summary>
        /// Resource packs are not allowed to affect gameplay
        /// </summary>
        /// <returns></returns>
        bool IsAllowedToAffectGameplay();
        
    }
}