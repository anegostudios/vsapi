using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    public class EntityBehaviorNameTag : EntityBehavior
    {
        /// <summary>
        /// The display name for the entity.
        /// </summary>
        public string DisplayName
        {
            get { return entity.WatchedAttributes.GetTreeAttribute("nametag")?.GetString("name"); }
        }

        /// <summary>
        /// Whether or not to show the nametag constantly or only when being looked at.
        /// </summary>
        public bool ShowOnlyWhenTargeted
        {
            get { return entity.WatchedAttributes.GetTreeAttribute("nametag")?.GetBool("showtagonlywhentargeted") == true; }
            set { entity.WatchedAttributes.GetTreeAttribute("nametag")?.SetBool("showtagonlywhentargeted", value); }
        }

        public int RenderRange
        {
            get { return entity.WatchedAttributes.GetTreeAttribute("nametag").GetInt("renderRange"); }
            set { entity.WatchedAttributes.GetTreeAttribute("nametag")?.SetInt("renderRange", value); }
        }


        public EntityBehaviorNameTag(Entity entity) : base(entity)
        {
            ITreeAttribute nametagTree = entity.WatchedAttributes.GetTreeAttribute("nametag");
            if (nametagTree == null)
            {
                entity.WatchedAttributes.SetAttribute("nametag", nametagTree = new TreeAttribute());
                nametagTree.SetString("name", "");
                nametagTree.SetInt("showtagonlywhentargeted", 0);
                nametagTree.SetInt("renderRange", 999);
                entity.WatchedAttributes.MarkPathDirty("nametag");
            }
        }

        public override void Initialize(EntityProperties entityType, JsonObject attributes)
        {
            base.Initialize(entityType, attributes);

            if ((DisplayName == null || DisplayName.Length == 0) && attributes["selectFromRandomName"].Exists)
            {
                string[] randomName = attributes["selectFromRandomName"].AsArray<string>();

                SetName(randomName[entity.World.Rand.Next(randomName.Length)]);
            }

            RenderRange = attributes["renderRange"].AsInt(999);
            ShowOnlyWhenTargeted = attributes["showtagonlywhentargeted"].AsBool(false);
        }

        public override void OnEntitySpawn()
        {
            base.OnEntitySpawn();
        }

        /// <summary>
        /// Sets the display name of the entity to playername.
        /// </summary>
        /// <param name="playername"></param>
        public void SetName(string playername)
        {
            ITreeAttribute nametagTree = entity.WatchedAttributes.GetTreeAttribute("nametag");
            if (nametagTree == null)
            {
                entity.WatchedAttributes.SetAttribute("nametag", nametagTree = new TreeAttribute());
            }

            nametagTree.SetString("name", playername);
            entity.WatchedAttributes.MarkPathDirty("nametag");
        }

        public override string PropertyName()
        {
            return "displayname";
        }
    }
}
