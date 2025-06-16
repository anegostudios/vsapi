using Newtonsoft.Json;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A world interaction for the object. This is used to prompt the player about what a certain object can do.
    /// </summary>
    [DocumentAsJson]
    [JsonObject(MemberSerialization.OptIn)]
    public class WorldInteraction
    {
        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>Left</jsondefault>-->
        /// What mouse button should be used for this interaction?
        /// </summary>
        [JsonProperty]
        public EnumMouseButton MouseButton;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Does it require a mouse modifier key to perform this action (e.g. "shift" or "ctrl")
        /// </summary>
        [JsonProperty]
        public string HotKeyCode;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// Does it require pressing multiple keys to perform this action (if set then HotkeyCode is ignored)
        /// </summary>
        [JsonProperty]
        public string[] HotKeyCodes { get; set; }

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// The text to show, will be used in the form of Lang.Get(ActionLangCode); 
        /// </summary>
        [JsonProperty]
        public string ActionLangCode;

        /// <summary>
        /// <!--<jsonalias>ItemStacks</jsonalias><jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
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
        /// Only applicable when ItemStacks is null. If set and the method returns false, the interaction will not be displayed
        /// </summary>
        public InteractionMatcherDelegate ShouldApply;
    }
}
