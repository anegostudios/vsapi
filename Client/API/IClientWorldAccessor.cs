using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

#nullable disable

namespace Vintagestory.API.Client
{
    public delegate ItemStack[] InteractionStacksDelegate(WorldInteraction wi, BlockSelection blockSelection, EntitySelection entitySelection);
    public delegate bool InteractionMatcherDelegate(WorldInteraction wi, BlockSelection blockSelection, EntitySelection entitySelection);

    /// <summary>
    /// The world accessor implemented by the client, offers some extra features only available on the client
    /// </summary>
    public interface IClientWorldAccessor : IWorldAccessor
    {

        ColorMapData GetColorMapData(Block block, int posX, int posY, int posZ);

        /// <summary>
        /// Interface to access the game calendar
        /// </summary>
        new IClientGameCalendar Calendar { get; }

        /// <summary>
        /// Loads the rgb climate and season color map value at given position and multiplies it byte-wise with supplied color
        /// </summary>
        /// <param name="climateColorMap"></param>
        /// <param name="seasonColorMap"></param>
        /// <param name="color"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="flipRb"></param>
        /// <returns></returns>
        int ApplyColorMapOnRgba(string climateColorMap, string seasonColorMap, int color, int posX, int posY, int posZ, bool flipRb = true);
        int ApplyColorMapOnRgba(ColorMap climateColorMap, ColorMap seasonColorMap, int color, int posX, int posY, int posZ, bool flipRb = true);


        /// <summary>
        /// Loads the rgb climate and season color map value for given rain and temp value and multiplies it byte-wise with supplied color
        /// </summary>
        /// <param name="climateColorMap"></param>
        /// <param name="seasonColorMap"></param>
        /// <param name="color"></param>
        /// <param name="rain"></param>
        /// <param name="temp"></param>
        /// <param name="flipRb"></param>
        /// <returns></returns>
        int ApplyColorMapOnRgba(string climateColorMap, string seasonColorMap, int color, int rain, int temp, bool flipRb = true);


        /// <summary>
        /// Whether the player can select liquids
        /// </summary>
        bool ForceLiquidSelectable { get; set;}

        /// <summary>
        /// Whether to spawn ambient particles
        /// </summary>
        bool AmbientParticles { get; set; }

        

        /// <summary>
        /// Returns the player running this client instance
        /// </summary>
        IClientPlayer Player { get; }

        /// <summary>
        /// Loads a sounds without playing it. Use to individually control when to play/stop. Might want to set DisposeOnFinish to false but then you have to dispose it yourself. 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        ILoadedSound LoadSound(SoundParams param);

        /// <summary>
        /// Shakes the camera view by given strength
        /// </summary>
        /// <param name="strengh"></param>
        void AddCameraShake(float strengh);

        void SetCameraShake(float strengh);

        void ReduceCameraShake(float amount);

        /// <summary>
        /// Same effect as when player left-click breaks a block, but will not cause actual breakage of the block
        /// </summary>
        /// <param name="withTool"></param>
        /// <param name="damage"></param>
        /// <param name="blockSelection"></param>
        void IncurBlockDamage(BlockSelection blockSelection, EnumTool? withTool, float damage);

        /// <summary>
        /// Applies the same damage overlay effect on the target as the source has
        /// </summary>
        /// <param name="sourcePos"></param>
        /// <param name="targetPos"></param>
        void CloneBlockDamage(BlockPos sourcePos, BlockPos targetPos);

        /// <summary>
        /// Makes an attempt to attack a particular entity.
        /// </summary>
        /// <param name="sele"></param>
        void TryAttackEntity(EntitySelection sele);

        /// <summary>
        /// The internal cache of all currently loaded entities. Warning: You should not set or remove anything from this dic unless you *really* know what you're doing. Use SpawnEntity/DespawnEntity instead.
        /// </summary>
        Dictionary<long, Entity> LoadedEntities { get; }

        /// <summary>
        /// Gets the MapSizeY on the client, for chunk column enumeration, without this this is surprisingly hard to get...
        /// </summary>
        int MapSizeY { get; }

        Dictionary<int, IMiniDimension> MiniDimensions { get; }

        IMiniDimension GetOrCreateDimension(int dimId, Vec3d pos);
        bool TryGetMiniDimension(Vec3i origin, out IMiniDimension dimension);
        void SetBlocksPreviewDimension(int dimId);

        /// <summary>
        /// Exactly like PlaySoundAt except that it returns the duration of the played sound.  (We don't want to change the method signature of PlaySoundAt for API mod breakage reasons)
        /// </summary>
        int PlaySoundAtAndGetDuration(AssetLocation sound, double x, double y, double z, IPlayer ignorePlayerUid = null, bool randomizePitch = true, float range = 32, float volume = 1f);

        /// <summary>
        /// Does exactly what it says on the tin!
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cz"></param>
        /// <param name="dimension"></param>
        void SetChunkColumnVisible(int cx, int cz, int dimension);
    }
}
