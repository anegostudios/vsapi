using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    public delegate Vec3d PositionProviderDelegate();

    public class CommandArgumentParsers
    {
        ICoreAPI api;
        public CommandArgumentParsers(ICoreAPI api)
        {
            this.api = api;
        }

        public UnparsedArg Unparsed(string argname, params string[] validRange) => new UnparsedArg(argname, validRange);

        public DirectionArgParser<Vec3i> IntDirection(string argName) => new DirectionArgParser<Vec3i>(argName, true);

        public EntitiesArgParser Entities(string argName) => new EntitiesArgParser(argName, api, true);

        /// <summary>
        /// Defaults to caller entity
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public EntitiesArgParser OptionalEntities(string argName) => new EntitiesArgParser(argName, api, false);

        public EntityTypeArgParser EntityType(string argName) => new EntityTypeArgParser(argName, api, true);

        public IntArgParser IntRange(string argName, int min, int max) => new IntArgParser(argName, min, max, 0, true);

        public IntArgParser OptionalIntRange(string argName, int min, int max, int defaultValue = 0) => new IntArgParser(argName, min, max, defaultValue, false);

        public IntArgParser OptionalInt(string argName, int defaultValue = 0) => new IntArgParser(argName, defaultValue, false);
        public IntArgParser Int(string argName) => new IntArgParser(argName, 0, true);

        public BoolArgParser Bool(string argName, string trueAlias = "on") => new BoolArgParser(argName, trueAlias, true);

        public BoolArgParser OptionalBool(string argName, string trueAlias = "on") => new BoolArgParser(argName, trueAlias, false);

        public DoubleArgParser OptionalDouble(string argName) => new DoubleArgParser(argName, false);

        public FloatArgParser OptionalFloat(string argName) => new FloatArgParser(argName, false);

        public DoubleArgParser Double(string argName) => new DoubleArgParser(argName, true);
        public DoubleArgParser DoubleRange(string argName, double min, double max) => new DoubleArgParser(argName, min, max, true);
        /// <summary>
        /// A currently online player
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public OnlinePlayerArgParser OnlinePlayer(string argName) => new OnlinePlayerArgParser(argName, api, true);
        /// <summary>
        /// All selected players
        /// </summary>
        /// <param name="argName"></param>
        /// <param name="onResultReady"></param>
        /// <returns></returns>
        public PlayersArgParser PlayerUids(string argName) => new PlayersArgParser(argName, api, true);

        /// <summary>
        /// All selected players
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public PlayersArgParser OptionalPlayerUids(string argName) => new PlayersArgParser(argName, api, false);

        public WordArgParser Word(string argName) => new WordArgParser(argName, true, null);

        public WordArgParser OptionalWord(string argName) => new WordArgParser(argName, false, null);

        public WordRangeArgParser OptionalWordRange(string argName, params string[] words) => new WordRangeArgParser(argName, false, words);

        public WordArgParser Word(string argName, string[] wordSuggestions) => new WordArgParser(argName, true, wordSuggestions);

        /// <summary>
        /// All remaining arguments together
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public StringArgParser All(string argName) => new StringArgParser(argName, true);

        /// <summary>
        /// All remaining arguments together
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public StringArgParser OptionalAll(string argName) => new StringArgParser(argName, false);

        public WordRangeArgParser WordRange(string argName, params string[] words) => new WordRangeArgParser(argName, true, words);
        public WorldPositionArgParser WorldPosition(string argName) => new WorldPositionArgParser(argName, api, true);
        public WorldPosition2DArgParser WorldPosition2D(string argName) => new WorldPosition2DArgParser(argName, api, true);
        public CollectibleArgParser Item(string argName) => new CollectibleArgParser(argName, api, EnumItemClass.Item, true);
        public CollectibleArgParser Block(string argName) => new CollectibleArgParser(argName, api, EnumItemClass.Block, true);
        
        /// <summary>
        /// Defaults to caller position
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public WorldPositionArgParser OptionalWorldPosition(string argName) => new WorldPositionArgParser(argName, api, false);

        /// <summary>
        /// Currently only supports time spans (i.e. now + time)
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public DatetimeArgParser DateTime(string argName) => new DatetimeArgParser(argName, true);
    }

    public abstract class ArgumentParserBase : ICommandArgumentParser
    {
        protected string lastErrorMessage;
        protected bool isMandatoryArg;
        protected int argCount = 1;
        protected string argName;

        protected ArgumentParserBase(string argName, bool isMandatoryArg)
        {
            this.argName = argName;
            this.isMandatoryArg = isMandatoryArg;
        }

        public string LastErrorMessage => lastErrorMessage;
        public string ArgumentName => argName;
        public bool IsMandatoryArg => isMandatoryArg;
        public bool IsMissing { get; set; }
        public int ArgCount => argCount;

        public virtual string[] GetValidRange(CmdArgs args)
        {
            return null;
        }

        public abstract object GetValue();
        public abstract void SetValue(object data);

        public virtual string GetSyntax()
        {
            return isMandatoryArg ? "<i>&lt;" + argName + "&gt;</i>" : "<i>[" + argName + "]</i>";
        }
        public virtual string GetSyntaxExplanation()
        {
            return null;
        }

        public virtual string GetLastError()
        {
            return lastErrorMessage;
        }

        public abstract EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null);


        protected Dictionary<string, string> parseSubArgs(string strargs)
        {
            Dictionary<string, string> subargs = new Dictionary<string, string>();
            if (strargs.Length == 0) return subargs;

            StringBuilder argssb = new StringBuilder();
            bool inside = false;
            for (int i = 0; i < strargs.Length; i++)
            {
                char a = strargs[i];
                if (a == '[') { inside = true; continue; }
                if (a == ']') break;
                if (inside)
                {
                    argssb.Append(a);
                }
            }
            var args = argssb.ToString().Split(',');

            foreach (var arg in args)
            {
                if (arg.Length == 0) continue;

                var keyval = arg.Split('=');
                if (keyval.Length < 2) continue;
                subargs[keyval[0].ToLowerInvariant().Trim()] = keyval[1].Trim();
            }

            return subargs;
        }

        public virtual void PreProcess(TextCommandCallingArgs args)
        {
            IsMissing = args.RawArgs.Length == 0;
        }
    }


    public abstract class PositionArgumentParserBase : ArgumentParserBase
    {
        protected PositionArgumentParserBase(string argName, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
        }

        protected EnumParseResult tryGetPositionBySelector(char v, TextCommandCallingArgs args, ref Vec3d pos, ICoreAPI api)
        {
            var subargsstr = args.RawArgs.PopWord();
            var subargs = parseSubArgs(subargsstr);

            Vec3d sourcePos = args.Caller.Pos;
            Entity callingEntity = args.Caller.Entity;

            float? range = null;
            string strrange;
            if (subargs.TryGetValue("range", out strrange)) range = strrange.ToFloat();

            string typestr;
            AssetLocation type = null;
            if (subargs.TryGetValue("type", out typestr))
            {
                type = new AssetLocation(typestr);
            }

            string name;
            subargs.TryGetValue("name", out name);

            bool? alive = null;
            if (subargs.TryGetValue("alive", out var stralive))
            {
                alive = stralive.ToBool();
            }


            if (range != null && sourcePos == null)
            {
                lastErrorMessage = "Can't use range argument without source position";
                return EnumParseResult.Bad;
            }

            switch (v)
            {
                // Nearest Player
                case 'p':
                    IPlayer nearestPlr = null;

                    foreach (var plr in api.World.AllOnlinePlayers)
                    {
                        if (range != null && plr.Entity.Pos.DistanceTo(sourcePos) > range) continue;
                        if (name != null && !WildcardUtil.Match(name, plr.PlayerName)) continue;
                        if (alive != null && plr.Entity.Alive != alive) continue;

                        if (nearestPlr == null) nearestPlr = plr;
                        else
                        {
                            if (sourcePos == null)
                            {
                                lastErrorMessage = "Two matching players found. Can't get nearest player without source position";
                                return EnumParseResult.Bad;
                            }

                            if (nearestPlr.Entity.Pos.DistanceTo(sourcePos) > plr.Entity.Pos.DistanceTo(sourcePos))
                            {
                                nearestPlr = plr;
                            }
                        }
                    }

                    pos = nearestPlr?.Entity.Pos.XYZ;
                    return EnumParseResult.Good;

                // Nearest Entity
                case 'e':
                    ICollection<Entity> entities;
                    if (api.Side == EnumAppSide.Server) entities = (api as ICoreServerAPI).World.LoadedEntities.Values;
                    else entities = (api as ICoreClientAPI).World.LoadedEntities.Values;

                    Entity nearestEntity = null;

                    foreach (Entity e in entities)
                    {
                        if (range != null && e.Pos.DistanceTo(sourcePos) > range) continue;
                        if (type != null && !WildcardUtil.Match(type, e.Code)) continue;
                        if (alive != null && e.Alive != alive) continue;
                        if (name != null && !WildcardUtil.Match(name, e.GetBehavior<EntityBehaviorNameTag>()?.DisplayName)) continue;

                        if (nearestEntity == null) nearestEntity = e;
                        else
                        {
                            if (sourcePos == null)
                            {
                                lastErrorMessage = "Two matching entities found. Can't get nearest entity without source position";
                                return EnumParseResult.Bad;
                            }

                            if (nearestEntity.Pos.DistanceTo(sourcePos) > e.Pos.DistanceTo(sourcePos))
                            {
                                nearestEntity = e;
                            }
                        }
                    }

                    pos = nearestEntity?.Pos.XYZ;
                    return EnumParseResult.Good;

                // Looked at entity
                case 'l':
                    var eplr = callingEntity as EntityPlayer;
                    if (eplr == null)
                    {
                        lastErrorMessage = "Can't use 'l' without source player";
                        return EnumParseResult.Bad;
                    }
                    if (eplr.Player.CurrentEntitySelection == null && eplr.Player.CurrentBlockSelection == null)
                    {
                        lastErrorMessage = "Not looking at an entity or block";
                        return EnumParseResult.Bad;
                    }

                    pos = eplr.Player.CurrentEntitySelection?.Entity.Pos.XYZ ?? eplr.Player.CurrentBlockSelection.Position.ToVec3d(); // FullPosition borks block reads
                    return EnumParseResult.Good;

                // Executing entity
                case 's':
                    pos = callingEntity.Pos.XYZ;
                    return EnumParseResult.Good;

                default:
                    lastErrorMessage = "Wrong selector, needs to be p,e,l or s";
                    return EnumParseResult.Bad;
            }
        }
    }

    public class UnparsedArg : ArgumentParserBase
    {
        string[] validRange;
        public UnparsedArg(string argName, params string[] validRange) : base(argName, false)
        {
            this.validRange = validRange;
            this.argCount = -1;
        }

        public override object GetValue()
        {
            return null;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return validRange;
        }

        public override void SetValue(object data)
        {
            
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            return EnumParseResult.Good;
        }
    }

    public class StringArgParser : ArgumentParserBase
    {
        string value;

        public StringArgParser(string argName, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.argCount = -1;
        }

        public override object GetValue()
        {
            return this.isMandatoryArg || !this.IsMissing ? value : null;
        }

        public override void SetValue(object data)
        {
            value = (string)data;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            value = args.RawArgs.PopAll();
            return EnumParseResult.Good;
        }
    }

    public class EntityTypeArgParser : ArgumentParserBase
    {
        ICoreAPI api;
        EntityProperties type;

        public EntityTypeArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.api = api;
        }

        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is one of the registered entityTypes";
        }


        public override object GetValue()
        {
            return type;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            var str = args.RawArgs.PopWord();
            if (str == null)
            {
                lastErrorMessage = "Missing";
                return EnumParseResult.Bad;
            }
            type = api.World.GetEntityType(new AssetLocation(str));
            if (type == null)
            {
                lastErrorMessage = "No such entity type exists";
                return EnumParseResult.Bad;
            }

            return EnumParseResult.Good;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return api.World.EntityTypes.Select(t => t.Code.ToShortString()).ToArray();
        }

        public override void SetValue(object data)
        {
            type = (EntityProperties)data;
        }
    }

    public class EntitiesArgParser : ArgumentParserBase
    {
        Entity[] entities;
        ICoreAPI api;

        public EntitiesArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.api = api;
        }
        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is either a player name, or else one of the following selection codes:" +
                "\n&nbsp;&nbsp;&nbsp;&nbsp;s[] for self" +
                "\n&nbsp;&nbsp;&nbsp;&nbsp;l[] for the entity currently looked at" +
                "\n&nbsp;&nbsp;&nbsp;&nbsp;p[] for all players" +
                "\n&nbsp;&nbsp;&nbsp;&nbsp;e[] for all entities." +
                "\n&nbsp;&nbsp;Inside the square brackets, one or more filters can be added, to be more selective.  Filters include name, type, class, alive, range.  For example, <code>e[type=gazelle,range=3,alive=true]</code>.  The filters minx/miny/minz/maxx/maxy/maxz can also be used to specify a volume to search, coordinates are relative to the command caller's position." +
                "\n&nbsp;&nbsp;This argument may be omitted if the remainder of the command makes sense, in which case it will be interpreted as self.";
        }

        public override object GetValue()
        {
            return entities;
        }

        public override void SetValue(object data)
        {
            entities = (Entity[])data;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            if (IsMissing)
            {
                this.entities = new Entity[] { args.Caller.Entity };
            }
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            string maybeplayername = args.RawArgs.PeekWord();
            var mplr = api.World.AllOnlinePlayers.FirstOrDefault(p => p.PlayerName.Equals(maybeplayername, StringComparison.InvariantCultureIgnoreCase));
            if (mplr != null)
            {
                args.RawArgs.PopWord();
                this.entities = new Entity[] { mplr.Entity };
                return EnumParseResult.Good;
            }

            char v = maybeplayername?.Length > 0 ? maybeplayername[0] : ' ';
            if (v != 'p' && v !='e' && v != 'l' && v != 's')
            {
                lastErrorMessage = Lang.Get("Not a player name and not a selector p, e, l or s: {0}'", maybeplayername);
                this.entities = new Entity[] { args.Caller.Entity };
                return EnumParseResult.DependsOnSubsequent;
            }
            v = args.RawArgs.PopChar() ?? ' ';


            Dictionary<string, string> subargs;
            if (args.RawArgs.PeekChar() == '[')
            {
                var subargsstr = args.RawArgs.PopCodeBlock('[', ']', out string errorMsg);
                if (errorMsg != null)
                {
                    lastErrorMessage = errorMsg;
                    return EnumParseResult.Bad;
                }

                subargs = parseSubArgs(subargsstr);
            } else
            {
                if (args.RawArgs.PeekChar() == ' ')
                {
                    args.RawArgs.PopWord(); // pop empty space
                } else
                {
                    lastErrorMessage = "Invalid selector, needs to be p,e,l,s followed by [";
                    return EnumParseResult.Bad;
                }
                subargs = new Dictionary<string, string>();
            }

            Vec3d sourcePos = args.Caller.Pos;
            Entity callingEntity = args.Caller.Entity;

            float? range = null;
            string strrange;
            if (subargs.TryGetValue("range", out strrange))
            {
                range = strrange.ToFloat();
                subargs.Remove("range");
            }

            string typestr;
            AssetLocation type = null;
            if (subargs.TryGetValue("type", out typestr))
            {
                type = new AssetLocation(typestr);
                subargs.Remove("type");
            }

            string classstr = null;
            if (subargs.TryGetValue("class", out classstr))
            {
                classstr = classstr.ToLowerInvariant();
                subargs.Remove("class");
            }

            string name = null;
            if (subargs.TryGetValue("name", out name))
            {
                subargs.Remove("name");
            }

            bool? alive = null;
            if (subargs.TryGetValue("alive", out var stralive))
            {
                alive = stralive.ToBool();
                subargs.Remove("alive");
            }

            Cuboidi box = null;
            if (sourcePos != null)
            {
                string boxParam = null;
                if (subargs.TryGetValue("minx", out boxParam))
                {
                    if (box == null) box = new Cuboidi(sourcePos.AsBlockPos, 1);
                    box.Set(box.MinX + boxParam.ToInt(), box.MinY, box.MinZ, box.MaxX, box.MaxY, box.MaxZ);
                    subargs.Remove("minx");
                }
                if (subargs.TryGetValue("maxx", out boxParam))
                {
                    if (box == null) box = new Cuboidi(sourcePos.AsBlockPos, 1);
                    box.Set(box.MinX, box.MinY, box.MinZ, box.MaxX + boxParam.ToInt() - 1, box.MaxY, box.MaxZ);
                    subargs.Remove("maxx");
                }
                if (subargs.TryGetValue("miny", out boxParam))
                {
                    if (box == null) box = new Cuboidi(sourcePos.AsBlockPos, 1);
                    box.Set(box.MinX, box.MinY + boxParam.ToInt(), box.MinZ, box.MaxX, box.MaxY, box.MaxZ);
                    subargs.Remove("miny");
                }
                if (subargs.TryGetValue("maxy", out boxParam))
                {
                    if (box == null) box = new Cuboidi(sourcePos.AsBlockPos, 1);
                    box.Set(box.MinX, box.MinY, box.MinZ, box.MaxX, box.MaxY + boxParam.ToInt() - 1, box.MaxZ);
                    subargs.Remove("maxy");
                }
                if (subargs.TryGetValue("minz", out boxParam))
                {
                    if (box == null) box = new Cuboidi(sourcePos.AsBlockPos, 1);
                    box.Set(box.MinX, box.MinY, box.MinZ + boxParam.ToInt(), box.MaxX, box.MaxY, box.MaxZ);
                    subargs.Remove("minz");
                }
                if (subargs.TryGetValue("maxz", out boxParam))
                {
                    if (box == null) box = new Cuboidi(sourcePos.AsBlockPos, 1);
                    box.Set(box.MinX, box.MinY, box.MinZ, box.MaxX, box.MaxY, box.MaxZ + boxParam.ToInt() - 1);
                    subargs.Remove("maxz");
                }
            }


            if (subargs.Count > 0)
            {
                lastErrorMessage = "Unknown selector '" + string.Join(", ", subargs.Keys) + "'";
                return EnumParseResult.Bad;
            }

            List<Entity> foundEntities = new List<Entity>();

            if (range != null && sourcePos == null)
            {
                lastErrorMessage = "Can't use range argument without source pos";
                return EnumParseResult.Bad;
            }

            switch (v)
            {
                // Players
                case 'p':
                    foreach (var plr in api.World.AllOnlinePlayers)
                    {
                        if (entityMatches(plr.Entity, sourcePos, type, classstr, range, box, name, alive))
                        {
                            foundEntities.Add(plr.Entity);
                        }
                    }

                    this.entities = foundEntities.ToArray();
                    return EnumParseResult.Good;
               
                // Entities
                case 'e':
                    if (range == null)
                    {
                        ICollection<Entity> entities;
                        if (api.Side == EnumAppSide.Server) entities = (api as ICoreServerAPI).World.LoadedEntities.Values;
                        else entities = (api as ICoreClientAPI).World.LoadedEntities.Values;

                        foreach (Entity e in entities)
                        {
                            if (entityMatches(e, sourcePos, type, classstr, range, box, name, alive))
                            {
                                foundEntities.Add(e);
                            }
                        }

                        this.entities = foundEntities.ToArray();
                    }
                    else
                    {
                        float r = (float)range;
                        entities = api.World.GetEntitiesAround(sourcePos, r, r, (e) =>
                        {
                            return entityMatches(e, sourcePos, type, classstr, range, box, name, alive);
                        });

                    }

                    return EnumParseResult.Good;

                // Looked at entity
                case 'l':
                    var eplr = callingEntity as EntityPlayer;
                    if (eplr == null)
                    {
                        lastErrorMessage = "Can't use 'l' without source player";
                        return EnumParseResult.Bad;
                    }
                    if (eplr.Player.CurrentEntitySelection == null)
                    {
                        lastErrorMessage = "Not looking at an entity";
                        return EnumParseResult.Bad;
                    }

                    var lookedAtEntity = eplr.Player.CurrentEntitySelection.Entity;
                    if (entityMatches(lookedAtEntity, sourcePos, type, classstr, range, box, name, alive))
                    {
                        this.entities = new Entity[] { lookedAtEntity }; 
                    }
                    else
                    {
                        this.entities = new Entity[0];
                    }
                    return EnumParseResult.Good;

                // Executing entity
                case 's':
                    if (entityMatches(callingEntity, sourcePos, type, classstr, range, box, name, alive))
                    {
                        this.entities = new Entity[] { callingEntity }; 
                    }
                    else
                    {
                        this.entities = new Entity[0];
                    }
                    return EnumParseResult.Good;

                default:
                    lastErrorMessage = "Wrong selector, needs to be a player name or p,e,l or s";
                    return EnumParseResult.Bad;
            }
        }

        private bool entityMatches(Entity e, Vec3d sourcePos, AssetLocation type, string classstr, float? range, Cuboidi box, string name, bool? alive)
        {
            if (range != null && e.Pos.DistanceTo(sourcePos) > range) return false;
            if (box != null && !box.ContainsOrTouches(e.Pos)) return false;
            if (classstr != null && classstr != e.Class.ToLowerInvariant()) return false;
            if (type != null && !WildcardUtil.Match(type, e.Code)) return false;
            if (alive != null && e.Alive != alive) return false;
            if (name != null && !WildcardUtil.Match(name, e.GetBehavior<EntityBehaviorNameTag>()?.DisplayName)) return false;

            return true;
        }
    }

    public class CollectibleArgParser : ArgumentParserBase
    {
        private EnumItemClass itemclass;
        private ICoreAPI api;

        ItemStack stack;

        public CollectibleArgParser(string argName, ICoreAPI api, EnumItemClass itemclass, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.api = api;
            this.itemclass = itemclass;
        }
        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is a precise " + itemclass + " name, optionally immediately followed by braces {} containing attributes for the " + itemclass;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            string word = args.PeekWord();
            if (word.Length == 0) return null;

            if (word.Contains("{"))
            {
                var parts = word.Split(new string[] { "{" }, 1, StringSplitOptions.None);
                word = parts[0];
            }

            if (itemclass == EnumItemClass.Block)
            {
                return api.World.SearchBlocks(new AssetLocation(word)).Select((b) => b.Code.ToShortString()).ToArray();
            } else
            {
                return api.World.SearchItems(new AssetLocation(word)).Select((i) => i.Code.ToShortString()).ToArray();
            }
        }

        public override object GetValue()
        {
            return stack;
        }

        public override void SetValue(object data)
        {
            stack = (ItemStack)data;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            string code = args.RawArgs.PeekWord(null);
            if (code == null)
            {
                lastErrorMessage = "Missing";
                return EnumParseResult.Bad;
            }

            string attributes = null;

            if (code.Contains("{"))
            {
                code = args.RawArgs.PopUntil('{');
                if (code.Length == 0)
                {
                    lastErrorMessage = "Missing Item/Block Code";
                    return EnumParseResult.Bad;
                }

                attributes = args.RawArgs.PopCodeBlock('{', '}', out string errorMsg);
                if (errorMsg != null)
                {
                    lastErrorMessage = errorMsg;
                    return EnumParseResult.Bad;
                }
            } else
            {
                args.RawArgs.PopWord();
            }


            if (itemclass == EnumItemClass.Block)
            {
                var block = api.World.GetBlock(new AssetLocation(code));
                if (block == null)
                {
                    lastErrorMessage = "No such block exists";
                    return EnumParseResult.Bad;
                }
                stack = new ItemStack(block);
            }
            else
            {
                var item = api.World.GetItem(new AssetLocation(code));
                if (item == null)
                {
                    lastErrorMessage = "No such item exists";
                    return EnumParseResult.Bad;
                }
                stack = new ItemStack(item);
            }

            if (attributes != null)
            {
                try
                {
                    stack.Attributes = JsonObject.FromJson(attributes).ToAttribute() as ITreeAttribute;
                } catch (Exception e)
                {
                    lastErrorMessage = "Unable to json parse attributes, likely invalid syntax: " + e.Message;
                    return EnumParseResult.Bad;
                }
            }


            return EnumParseResult.Good;
        }
    }

    public class WorldPositionArgParser : PositionArgumentParserBase
    {
        Vec3d pos;
        PositionProviderDelegate mapmiddlePosProvider;
        ICoreAPI api;

        public WorldPositionArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.api = api;
            this.mapmiddlePosProvider = () => new Vec3d(api.World.DefaultSpawnPosition.X, 0, api.World.DefaultSpawnPosition.Z);
            argCount = 3;
        }
        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is a world position.  A world position can be either 3 coordinates, or a target selector." +
                "\n&nbsp;&nbsp;&nbsp;&nbsp;3 coordinates are specified as in the following examples:" +
                "\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<code>100 150 -180</code> means 100 blocks East and 180 blocks North from the map center, with height 150 blocks" +
                "\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<code>~-5 ~0 ~4</code> means 5 blocks West and 4 blocks South from the caller's position" +
                "\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<code>=512100 =150 =511880</code> means the absolute x,y,z position specified (at default settings this is near the map center)" +
                "\n&nbsp;&nbsp;&nbsp;&nbsp;A target selector is either a player's name (meaning that player's current position), or one of: <code>s[]</code> for self, <code>l[]</code> for looked-at entity or block, <code>p[]</code> for players, <code>e[]</code> for entities." +
                "One or more filters can be specified inside the brackets.  For p[] or e[], the target will be the nearest player or entity which passes all the filters." +
                "Filters include name, type, class, alive, range.  For example, <code>e[type=gazelle,range=3,alive=true]</code>.  The filters minx/miny/minz/maxx/maxy/maxz can also be used to specify a volume to search, coordinates are relative to the command caller's position.";
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return null;
        }

        public override object GetValue()
        {
            return pos;
        }

        public override void SetValue(object data)
        {
            pos = (Vec3d)data;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            pos = args.Caller.Pos?.Clone();
            base.PreProcess(args);
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            string maybeplayername = args.RawArgs.PeekWord();
            var mplr = api.World.AllOnlinePlayers.FirstOrDefault(p => p.PlayerName.Equals(maybeplayername, StringComparison.InvariantCultureIgnoreCase));
            if (mplr != null)
            {
                args.RawArgs.PopWord();
                this.pos = mplr.Entity.Pos.XYZ;
                return EnumParseResult.Good;
            }

            var v = args.RawArgs.PeekChar();
            if (v == 'p' || v == 'e' || v == 'l' || v == 's')
            {
                args.RawArgs.PopChar();
                return tryGetPositionBySelector((char)v, args, ref this.pos, api);
            }

            if (args.RawArgs.Length < 3)
            {
                lastErrorMessage = "World position must be either 3 coordinates or a target selector beginning with p (nearest player), e (nearest entity), l (looked at entity/block) or s (executing entity)";
                return EnumParseResult.Bad;
            }

            pos = args.RawArgs.PopFlexiblePos(args.Caller.Pos, mapmiddlePosProvider());
            if (pos == null) lastErrorMessage = Lang.Get("Invalid position");

            return pos == null ? EnumParseResult.Bad : EnumParseResult.Good;
        }

    }

    public class WorldPosition2DArgParser : PositionArgumentParserBase
    {
        Vec2i pos;
        PositionProviderDelegate mapmiddlePosProvider;
        ICoreAPI api;

        public WorldPosition2DArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.api = api;
            this.mapmiddlePosProvider = () => api.World.DefaultSpawnPosition.XYZ;
            argCount = 3;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return null;
        }

        public override object GetValue()
        {
            return pos;
        }

        public override void SetValue(object data)
        {
            pos = (Vec2i)data;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            pos = posTo2D(args.Caller.Pos?.Clone());
            base.PreProcess(args);
        }

        private Vec2i posTo2D(Vec3d callerPos)
        {
            return callerPos == null ? null : new Vec2i(callerPos);
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            if (args.RawArgs.Length == 1)
            {
                string maybeplayername = args.RawArgs.PeekWord();
                var mplr = api.World.AllOnlinePlayers.FirstOrDefault(p => p.PlayerName.Equals(maybeplayername, StringComparison.InvariantCultureIgnoreCase));
                if (mplr != null)
                {
                    args.RawArgs.PopWord();
                    this.pos = new Vec2i(mplr.Entity.Pos.XYZ);
                    return EnumParseResult.Good;
                }

                var v = args.RawArgs.PopChar();
                if (v == 'p' || v == 'e' || v == 'l' || v == 's')
                {
                    Vec3d pos3d = new Vec3d();
                    var result = tryGetPositionBySelector((char)v, args, ref pos3d, api);
                    pos = posTo2D(pos3d);
                    return result;
                }
                else
                {
                    lastErrorMessage = "World position 2D must be either 2 coordinates or a target selector beginning with p (nearest player), e (nearest entity), l (looked at entity) or s (executing entity)";
                    return EnumParseResult.Bad;
                }
            }

            if (args.RawArgs.Length < 2)
            {
                lastErrorMessage = "Need 2 values";
                return EnumParseResult.Good;
            }

            pos = args.RawArgs.PopFlexiblePos2D(args.Caller.Pos, mapmiddlePosProvider());
            if (pos == null) lastErrorMessage = Lang.Get("Invalid position");

            return pos == null ? EnumParseResult.Bad : EnumParseResult.Good;
        }
    }

    public class WordArgParser : ArgumentParserBase
    {
        string word;
        string[] suggestions;

        public WordArgParser(string argName, bool isMandatoryArg, string[] suggestions = null) : base(argName, isMandatoryArg)
        {
            this.suggestions = suggestions;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return suggestions;
        }

        public override object GetValue()
        {
            return this.isMandatoryArg || !this.IsMissing ? word : null;
        }

        public override void SetValue(object data)
        {
            word = (string)data;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            word = args.RawArgs.PopWord();
            if (word == null)
            {
                lastErrorMessage = "Missing";
                return EnumParseResult.Bad;
            }

            return EnumParseResult.Good;
        }
    }

    public class WordRangeArgParser : ArgumentParserBase
    {
        private string[] words;
        protected string word;

        public WordRangeArgParser(string argName, bool isMandatoryArg, params string[] words) : base(argName, isMandatoryArg)
        {
            this.words = words;
        }
        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is one of the following:" + string.Join(",", words);
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return words;
        }

        public override object GetValue()
        {
            return word;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            word = args.RawArgs.PopWord();
            if (!words.Contains(word))
            {
                word = null;
                lastErrorMessage = Lang.Get("Invalid");
                return EnumParseResult.Bad;
            }

            return EnumParseResult.Good;
        }

        public override void SetValue(object data)
        {
            word = (string)data;
        }
    }


    public class PlayerUidName
    {
        public string Uid;
        public string Name;

        public PlayerUidName() { }
        public PlayerUidName(string uid, string name)
        {
            this.Uid = uid;
            this.Name = name;
        }
    }

    public class PlayersArgParser : ArgumentParserBase
    {
        protected ICoreServerAPI api;
        PlayerUidName[] players;

        public PlayersArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.api = api as ICoreServerAPI;
            if (api.Side != EnumAppSide.Server) throw new InvalidOperationException("Players arg parser is only available server side");
        }
        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is the name of one player, or a selector in this format: s[] for self, o[] for online players, a[] for all players." +
                "Some filters can be specified inside the brackets, though that doesn't make much sense for s[]." +
                "Filters include name, namematches, group, role, range.";
        }


        public override string[] GetValidRange(CmdArgs args)
        {
            return base.GetValidRange(args).Append("or any other valid player name");
        }

        public override object GetValue()
        {
            return players;
        }

        public override void SetValue(object data)
        {
            players = (PlayerUidName[])data;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            var v = args.RawArgs.PeekChar();
            if (v == null)
            {
                lastErrorMessage = "Wrong selector, needs to be either character o (online players), a (all players - online or offline)";
                return EnumParseResult.Bad;
            }

            var subargsstr = args.RawArgs.PopWord();
            Dictionary<string, string> subargs;
            if (subargsstr.Contains('['))
            {
                subargs = parseSubArgs(subargsstr.Substring(1));
            } else
            {
                subargs = new Dictionary<string, string>();
                subargs["name"] = subargsstr;
                v = 'a';
            }

            List<PlayerUidName> players = new List<PlayerUidName>();

            string role = subargs.Get("role");
            string name = subargs.Get("name");
            string group = subargs.Get("group");
            string namematches = subargs.Get("namematches");
            float? range = subargs.Get("range")?.ToFloat() ?? null;

            switch (v)
            {
                case 's':
                    if (args.Caller.Player == null)
                    {
                        throw new InvalidOperationException("s selector can only be used when the caller is a player.");
                    }
                    players.Add(new PlayerUidName(args.Caller.Player.PlayerUID, args.Caller.Player.PlayerName));
                    break;

                // Online players
                case 'o':
                    foreach (IServerPlayer plr in api.World.AllOnlinePlayers)
                    {
                        if (matches(plr.Entity.Pos.XYZ, args.Caller.Pos, plr.PlayerName, plr.GetGroups(), plr.Role.Code, role, name, group, namematches, range))
                        {
                            players.Add(new PlayerUidName(plr.PlayerUID, plr.PlayerName));
                        }
                    }

                    break;
                // Online or offline players
                case 'a':
                    if (range != null)
                    {
                        throw new InvalidOperationException("Range arg can only be used on online players");
                    }

                    var allPlayers = api.PlayerData.PlayerDataByUid;
                    foreach (var plr in allPlayers.Values)
                    {
                        if (matches(null, null, plr.LastKnownPlayername, plr.PlayerGroupMemberships.Values.ToArray(), plr.RoleCode, role, name, group, namematches, null))
                        {
                            players.Add(new PlayerUidName(plr.PlayerUID, plr.LastKnownPlayername));
                        }
                    }
                    break;

                default:
                    lastErrorMessage = "Wrong selector, needs to be either character o (online players), a (all players - online or offline)";
                    return EnumParseResult.Bad;
            }

            if (players.Count == 0 && name != null)
            {
                api.PlayerData.ResolvePlayerName(name, (resp, uid) =>
                {
                    if (resp == EnumServerResponse.Good)
                    {
                        onReady(new AsyncParseResults() { Status = EnumParseResultStatus.Ready, Data = new PlayerUidName[] { new PlayerUidName(uid, name) } });
                    }
                    else
                    {
                        lastErrorMessage = Lang.Get("No player with name '{0}' exists", name);
                        onReady(new AsyncParseResults() { Status = EnumParseResultStatus.Error });
                    }
                });

                return EnumParseResult.Deferred;
            }

            this.players = players.ToArray();
            return EnumParseResult.Good;
        }

        private bool matches(Vec3d pos, Vec3d callerPos, string playerName, PlayerGroupMembership[] plrGroups, string plrRoleCode, string role, string name, string group, string namematches, float? range)
        {
            if (name != null) return playerName == name;

            if (namematches != null && !WildcardUtil.Match(namematches, playerName)) return false;
            if (role != null && plrRoleCode != role) return false;
            if (range != null && pos.DistanceTo(callerPos) > (float)range) return false;
            if (group != null && plrGroups.Where(pgm => pgm.GroupName == group).Count() == 0) return false;

            return true;
        }
    }

    public class OnlinePlayerArgParser : ArgumentParserBase
    {
        protected ICoreAPI api;
        protected IPlayer player;

        public OnlinePlayerArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.api = api;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return api.World.AllOnlinePlayers.Select(p => p.PlayerName).ToArray();
        }

        public override object GetValue()
        {
            return player;
        }

        public override void SetValue(object data)
        {
            player = (IPlayer)data;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            string playername = args.RawArgs.PopWord();
            if (playername == null)
            {
                lastErrorMessage = Lang.Get("Missing");
                return EnumParseResult.Bad;
            }

            player = api.World.AllOnlinePlayers.FirstOrDefault(p => p.PlayerName == playername);
            if (player == null)
            {
                lastErrorMessage = Lang.Get("No such player online");
            }

            return player != null ? EnumParseResult.Good : EnumParseResult.Bad;
        }
    }

    public class DoubleArgParser : ArgumentParserBase
    {
        double min, max;
        double value;

        public DoubleArgParser(string argName, double min, double max, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.min = min;
            this.max = max;
        }
        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is a decimal number, for example 0.5";
        }

        public DoubleArgParser(string argName, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.min = double.MinValue;
            this.max = double.MaxValue;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return new string[] { double.MinValue + "", double.MaxValue + "" };
        }

        public override object GetValue()
        {
            return value;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            if (args.RawArgs.Length == 0)
            {
                lastErrorMessage = Lang.Get("Missing");
                return EnumParseResult.Bad;
            }
            var val = args.RawArgs.PopDouble();
            if (val == null)
            {
                lastErrorMessage = Lang.Get("Not a number");
                return EnumParseResult.Bad;
            }

            if (val < min || val > max)
            {
                lastErrorMessage = Lang.Get("Number out of range");
                return EnumParseResult.Bad;
            }

            this.value = (double)val;
            return EnumParseResult.Good;
        }

        public override void SetValue(object data)
        {
            value = (double)data;
        }
    }


    public class FloatArgParser : ArgumentParserBase
    {
        float min, max;
        float value;

        public FloatArgParser(string argName, float min, float max, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.min = min;
            this.max = max;
        }
        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is a decimal number, for example 0.5";
        }

        public FloatArgParser(string argName, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.min = float.MinValue;
            this.max = float.MaxValue;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return new string[] { float.MinValue + "", float.MaxValue + "" };
        }

        public override object GetValue()
        {
            return value;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            if (args.RawArgs.Length == 0)
            {
                lastErrorMessage = Lang.Get("Missing");
                return EnumParseResult.Bad;
            }
            var val = args.RawArgs.PopFloat();
            if (val == null)
            {
                lastErrorMessage = Lang.Get("Not a number");
                return EnumParseResult.Bad;
            }

            if (val < min || val > max)
            {
                lastErrorMessage = Lang.Get("Number out of range");
                return EnumParseResult.Bad;
            }

            this.value = (float)val;
            return EnumParseResult.Good;
        }

        public override void SetValue(object data)
        {
            value = (float)data;
        }
    }

    public class BoolArgParser : ArgumentParserBase
    {
        bool value;
        string trueAlias;

        public BoolArgParser(string argName, string trueAlias, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.trueAlias = trueAlias;
        }
        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is a boolean, including 1 or 0, yes or no, true or false, or " + trueAlias;
        }

        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object data)
        {
            value = (bool)data;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            var val = args.RawArgs.PopBool(null, trueAlias);
            if (val == null)
            {
                lastErrorMessage = "Missing";
                return EnumParseResult.Bad;
            }

            value = (bool)val;
            return EnumParseResult.Good;
        }
    }

    public class IntArgParser : ArgumentParserBase
    {
        int min, max;
        int value;
        int defaultValue;

        public IntArgParser(string argName, int min, int max, int defaultValue, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.defaultValue = defaultValue;
            this.min = min;
            this.max = max;
        }
        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is an integer number";
        }

        public IntArgParser(string argName, int defaultValue, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.defaultValue = defaultValue;
            this.min = int.MinValue;
            this.max = int.MaxValue;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return new string[] { int.MinValue+"", int.MaxValue+"" };
        }

        public override object GetValue()
        {
            return value;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            value = defaultValue;

            base.PreProcess(args);
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            var val = args.RawArgs.PopInt();
            if (val == null)
            {
                lastErrorMessage = Lang.Get("Not a number");
                return EnumParseResult.Bad;
            }
            if (val < min || val > max)
            {
                lastErrorMessage = Lang.Get("Number out of range");
                return EnumParseResult.Bad;
            }

            this.value = (int)val;
            return EnumParseResult.Good;
        }

        public override void SetValue(object data)
        {
            value = (int)data;
        }
    }



    public class DatetimeArgParser : ArgumentParserBase
    {
        DateTime datetime;
        List<string> timeUnits = new List<string>() { "minute", "hour", "day", "week", "year" };

        public DatetimeArgParser(string argName, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
        }
        public override string GetSyntaxExplanation()
        {
            return "&nbsp;&nbsp;<i>" + argName + "</i> is a date and time";
        }

        public override object GetValue()
        {
            return datetime;
        }

        public override void SetValue(object data)
        {
            datetime = (DateTime)data;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            int quantity = (int)args.RawArgs.PopInt(0);
            string timeunit = args.RawArgs.PopWord();

            if (quantity <= 0)
            {
                lastErrorMessage = Lang.Get("cmdban-invalidtimespan");
                return EnumParseResult.Bad;
            }
            
            switch (timeunit)
            {
                case "minute": datetime = DateTime.Now.AddMinutes(quantity); break;
                case "hour": datetime = DateTime.Now.AddHours(quantity); break;
                case "day": datetime = DateTime.Now.AddDays(quantity); break;
                case "week": datetime = DateTime.Now.AddDays(quantity * 7); break;
                case "year": datetime = DateTime.Now.AddYears(quantity); break;
                default:
                    lastErrorMessage = Lang.Get("cmdban-invalidtimeunit", string.Join(", ", timeUnits));
                    return EnumParseResult.Bad;
            }

            return EnumParseResult.Good;
        }
    }




    public class DirectionArgParser<T> : ArgumentParserBase where T : IVec3, new()
    {
        //BlockFacing Face;
        IVec3 value;

        public DirectionArgParser(string argName, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return new string[] { double.MinValue + "", double.MaxValue + "" };
        }

        public override object GetValue()
        {
            return value;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            /*if (args.RawArgs.Length == 0)
            {
                lastErrorMessage = Lang.Get("Missing direction type. Must be a face (north, east, south, west, down, up) or any combination of xyz");
                return EnumParseResult.Bad;
            }

            string dir = args.RawArgs.PopWord();

            Face = BlockFacing.FromCode(dir);
            if (Face != null)
            {
                value = new T();
                if (value.IsIntegral)
                {
                    double len = (double)args.RawArgs.PopInt(1);
                    value.Set(Face.Normali.X * len, Face.Normali.Y * len, Face.Normali.Z * len);
                } else
                {
                    double len = (double)args.RawArgs.PopDouble(1);
                    value.Set(Face.Normali.X * len, Face.Normali.Y * len, Face.Normali.Z * len);
                }
                
                return EnumParseResult.Good;
            }

            value = new T();
            for (int i = 0; i < dir.Length; i++)
            {
                var chr = dir[i];
                switch (chr)
                {
                    case 'x':
                        if (value.IsIntegral) value.XAsInt = (int)args.RawArgs.PopInt(1); 
                        else value.XAsDouble = (double)args.RawArgs.PopDouble(1);
                        break;
                    case 'y':
                        if (value.IsIntegral) value.YAsInt = (int)args.RawArgs.PopInt(1);
                        else value.YAsDouble = (double)args.RawArgs.PopDouble(1);
                        break;
                    case 'z':
                        if (value.IsIntegral) value.ZAsInt = (int)args.RawArgs.PopInt(1);
                        else value.ZAsDouble = (double)args.RawArgs.PopDouble(1);
                        break;
                    default:
                        lastErrorMessage = Lang.Get("Incorrect direction type. must be any combination of xyz, but found " + chr);
                        return EnumParseResult.Bad;
                }
            }*/

            return EnumParseResult.Good;
        }

        public override void SetValue(object data)
        {
            value = (Vec3d)data;
        }
    }
}
