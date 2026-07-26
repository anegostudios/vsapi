using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Mainly used for block entity based guis
    /// </summary>
    public abstract class GuiDialogGeneric : GuiDialog
    {
        /// <summary>
        /// The title of the Dialog.
        /// </summary>
        public string DialogTitle;

        /// <summary>
        /// Should this Dialog de-register itself once closed?
        /// </summary>
        public override bool UnregisterOnClose => true;

        /// <summary>
        /// The tree attributes for this dialog.
        /// </summary>
        public virtual ITreeAttribute Attributes { get; protected set; } = null;

        /// <summary>
        /// Constructor for a generic Dialog.
        /// </summary>
        /// <param name="DialogTitle">The title of the dialog.</param>
        /// <param name="capi">The Client API</param>
        public GuiDialogGeneric(string DialogTitle, ICoreClientAPI capi) : base(capi)
        {
            this.DialogTitle = DialogTitle;
        }

        public override string ToggleKeyCombinationCode
        {
            get { return null; }
        }

        /// <summary>
        /// Recomposes the dialog with it's set of elements.
        /// </summary>
        public virtual void Recompose()
        {
            foreach (GuiComposer composer in Composers.Values)
            {
                composer.ReCompose();
            }
        }

        /// <summary>
        /// Unfocuses the elements in each composer.
        /// </summary>
        public virtual void UnfocusElements()
        {
            foreach (GuiComposer composer in Composers.Values)
            {
                composer.UnfocusOwnElements();
            }
        }

        /// <summary>
        /// Focuses a specific element in the single composer.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        public virtual void FocusElement(int index)
        {
            SingleComposer.FocusElement(index);
        }

        /// <summary>
        /// Checks if the player is in range of the block.
        /// </summary>
        /// <param name="blockEntityPos">The block's position.</param>
        /// <returns>In range or no?</returns>
        // [Obsolete("Prefer using player."+nameof(IPlayer.IsInInteractionRangeOf)+". A dialog does not have an in-world position.", true)] // Rennorb: Obsolete in 1.23.
        public virtual bool IsInRangeOfBlock(BlockPos blockEntityPos)
        {
            return capi.World.Player.IsInInteractionRangeOf(blockEntityPos, .5f); // Rennorb 2026.07.06: Slack will change to standardized .25 in 1.23.
        }
    }
}
