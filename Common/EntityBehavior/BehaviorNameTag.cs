using Vintagestory.API;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    public class EntityBehaviorNameTag : EntityBehavior
    {
        public string DisplayName
        {
            get { return entity.WatchedAttributes.GetTreeAttribute("nametag")?.GetString("name"); }
        }

        public bool ShowOnlyWhenTargeted
        {
            get { return entity.WatchedAttributes.GetTreeAttribute("nametag")?.GetInt("showtagonlywhentargeted") > 0; }
        }

        public EntityBehaviorNameTag(Entity entity) : base(entity)
        {
            ITreeAttribute nametagTree = entity.WatchedAttributes.GetTreeAttribute("nametag");
            if (nametagTree == null)
            {
                entity.WatchedAttributes.SetAttribute("nametag", nametagTree = new TreeAttribute());
                nametagTree.SetString("name", "");
                nametagTree.SetInt("showtagonlywhentargeted", 0);
                entity.WatchedAttributes.MarkPathDirty("nametag");
            }
        }

        public override void Initialize(EntityProperties entityType, JsonObject attributes)
        {
            base.Initialize(entityType, attributes);

            if (attributes["selectFromRandomName"].Exists)
            {
                string[] randomName = attributes["selectFromRandomName"].AsStringArray();

                SetName(randomName[entity.World.Rand.Next(randomName.Length)]);

            }

        }

        public override void OnEntitySpawn()
        {
            base.OnEntitySpawn();
        }

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
