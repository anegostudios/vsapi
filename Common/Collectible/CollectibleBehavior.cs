using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public abstract class CollectibleBehavior
    {
        /// <summary>
        /// The collectible object (item or block) for this behavior instance.
        /// </summary>
        public CollectibleObject collObj;

        /// <summary>
        /// The properties of this block behavior.
        /// </summary>
        public string propertiesAtString;

        /// <summary>
        /// If true, this behavior is not required on the client. This is here because copygirl doesn't stop asking for it. Probably breaks things. If it breaks things, complain to copygirl please :p
        /// </summary>
        public virtual bool ClientSideOptional => false;

        public CollectibleBehavior(CollectibleObject collObj)
        {
            this.collObj = collObj;
        }

        /// <summary>
        /// Called right after the block behavior was created, must call base method
        /// </summary>
        /// <param name="properties"></param>
        public virtual void Initialize(JsonObject properties)
        {
            this.propertiesAtString = properties.ToString();
        }

        /// <summary>
        /// Server Side: Called once the collectible has been registered
        /// Client Side: Called once the collectible has been loaded from server packet
        /// </summary>
        /// <param name="api"></param>
        public virtual void OnLoaded(ICoreAPI api)
        {

        }

        /// <summary>
        /// When the player has begun using this item for attacking (left mouse click). Return true to play a custom action.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="handHandling"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual void OnHeldAttackStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandHandling handHandling, ref EnumHandHandling handling)
        {
            handHandling = EnumHandHandling.NotHandled;
        }


        /// <summary>
        /// When the player has canceled a custom attack action. Return false to deny action cancellation.
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySel"></param>
        /// <param name="cancelReason"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool OnHeldAttackCancel(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel, EnumItemUseCancelReason cancelReason, ref EnumHandHandling handling)
        {
            handling = EnumHandHandling.NotHandled;
            return false;
        }

        /// <summary>
        /// Called continously when a custom attack action is playing. Return false to stop the action.
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySel"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool OnHeldAttackStep(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel, ref EnumHandHandling handling)
        {
            handling = EnumHandHandling.NotHandled;
            return false;
        }

        /// <summary>
        /// Called when a custom attack action is finished
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySel"></param>
        /// <param name="handling"></param>
        public virtual void OnHeldAttackStop(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel, ref EnumHandHandling handling)
        {
            handling = EnumHandHandling.NotHandled;
        }



        /// <summary>
        /// Called when the player right clicks while holding this block/item in his hands
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="handHandling"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
        }


        /// <summary>
        /// Called every frame while the player is using this collectible
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }

        /// <summary>
        /// Called when the player successfully completed the using action, not called when successfully cancelled
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="handling"></param>
        public virtual void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return;
        }

        /// <summary>
        /// Called when the collectible is rendered in hands, inventory or on the ground
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="itemstack"></param>
        /// <param name="target"></param>
        /// <param name="renderinfo"></param>
        public virtual void OnBeforeRender(ICoreClientAPI capi, ItemStack itemstack, EnumItemRenderTarget target, ref ItemRenderInfo renderinfo)
        {
            
        }


        /// <summary>
        /// Interaction help that is shown when selecting the item in the hotbar slot
        /// </summary>
        /// <param name="inSlot"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return new WorldInteraction[0];
        }

    }
}