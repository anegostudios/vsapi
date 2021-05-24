using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public class TextureDictionary : Dictionary<string, CompositeTexture>
    {
        internal bool alreadyBaked = false;

        public TextureDictionary() : base()
        {
        }

        public TextureDictionary(int initialCapacity) : base(initialCapacity)
        {
        }

        /// <summary>
        /// Called by the texture atlas manager when building up the block atlas. Has to add all of the blocks textures
        /// </summary>
        public virtual void BakeAndCollect(IAssetManager manager, ITextureLocationDictionary mainTextureDict, AssetLocation sourceCode, string sourceMessage)
        {
            if (alreadyBaked) return;

            foreach (var ct in Values)
            {
                ct.Bake(manager);

                if (ct.Baked.BakedVariants != null)
                {
                    BakedCompositeTexture[] bct = ct.Baked.BakedVariants;
                    for (int i = 0; i < bct.Length; i++)
                    {
                        if (!mainTextureDict.ContainsKey(bct[i].BakedName))
                            mainTextureDict.AddTextureLocation_Checked(new AssetLocationAndSource(bct[i].BakedName, sourceMessage, sourceCode, i + 1));
                    }
                    continue;
                }

                if (!mainTextureDict.ContainsKey(ct.Baked.BakedName))
                    mainTextureDict.AddTextureLocation_Checked(new AssetLocationAndSource(ct.Baked.BakedName, sourceMessage, sourceCode));
            }

            alreadyBaked = true;
        }
    }
}
