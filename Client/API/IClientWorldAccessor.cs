using Newtonsoft.Json;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public delegate ItemStack[] InteractionStacksDelegate(WorldInteraction wi, BlockSelection blockSelection, EntitySelection entitySelection);
    public delegate bool InteractionMatcherDelegate(WorldInteraction wi, BlockSelection blockSelection, EntitySelection entitySelection);


    [JsonObject(MemberSerialization.OptIn)]
    public class WorldInteraction
    {
        /// <summary>
        /// Left or Right mouse button?
        /// </summary>
        [JsonProperty]
        public EnumMouseButton MouseButton;

        /// <summary>
        /// Does it require pressing a key to perform this action (e.g. "sneak" for sneaking)
        /// </summary>
        [JsonProperty]
        public string HotKeyCode;

        /// <summary>
        /// Does it require pressing multiple keys to perform this action (if set then HotkeyCode is ignored)
        /// </summary>
        [JsonProperty]
        public string[] HotKeyCodes { get; set; }

        /// <summary>
        /// The text to show, will be used in the form of Lang.Get(ActionLangCode); 
        /// </summary>
        [JsonProperty]
        public string ActionLangCode;

        /// <summary>
        /// Does the player need to hold a certain items/blocks in hands? (e.g. a knife). You can define an array of item stacks here and the game will loop through them in a 1 second interval.
        /// This property is loaded from the entitytypes and blocktype json files and then resolved.
        /// </summary>
        [JsonProperty("ItemStacks")]
        public JsonItemStack[] JsonItemStacks;

        public ItemStack[] Itemstacks;

        /// <summary>
        /// If true, the interaction only applies when the player has no slot in hands
        /// </summary>
        public bool RequireFreeHand;

        /// <summary>
        /// Only applicable when ItemStacks is non null. If set, this method will be executed before adding the interaction. Lets you return a filtered list of itemstacks that can be used for this interaction (or null/empty array for not interactable)
        /// </summary>
        public InteractionStacksDelegate GetMatchingStacks;

        /// <summary>
        /// Only applicable when ItemStacks is null. If set and the method returns falls, the interaction will not be displayed
        /// </summary>
        public InteractionMatcherDelegate ShouldApply;

        
    }

    /// <summary>
    /// The world accessor implemented by the client, offers some extra features only available on the client
    /// </summary>
    public interface IClientWorldAccessor : IWorldAccessor
    {
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
        /// Makes an attempt to attack a particular entity.
        /// </summary>
        /// <param name="sele"></param>
        void TryAttackEntity(EntitySelection sele);

        /// <summary>
        /// The internal cache of all currently loaded entities. Warning: You should not set or remove anything from this dic unless you *really* know what you're doing. Use SpawnEntity/DespawnEntity instead.
        /// </summary>
        Dictionary<long, Entity> LoadedEntities { get; }
    }
}
