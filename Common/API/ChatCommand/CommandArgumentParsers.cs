using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

#nullable disable

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

        public LongArgParser OptionalLong(string argName, int defaultValue = 0) => new LongArgParser(argName, defaultValue, false);
        public LongArgParser Long(string argName) => new LongArgParser(argName, 0, true);

        public BoolArgParser Bool(string argName, string trueAlias = "on") => new BoolArgParser(argName, trueAlias, true);

        public BoolArgParser OptionalBool(string argName, string trueAlias = "on") => new BoolArgParser(argName, trueAlias, false);

        public DoubleArgParser OptionalDouble(string argName, double defaultvalue = 0d) => new DoubleArgParser(argName, defaultvalue, false);

        public FloatArgParser Float(string argName) => new FloatArgParser(argName, 0f,true);

        public FloatArgParser OptionalFloat(string argName, float defaultvalue = 0f) => new FloatArgParser(argName, defaultvalue, false);

        public DoubleArgParser Double(string argName) => new DoubleArgParser(argName, 0d, true);
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
        /// <returns></returns>
        public PlayersArgParser PlayerUids(string argName) => new PlayersArgParser(argName, api, true);

        /// <summary>
        /// All selected players
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public PlayersArgParser OptionalPlayerUids(string argName) => new PlayersArgParser(argName, api, false);

        /// <summary>
        /// Parses IPlayerRole, only works on Serverside since it needs the Serverconfig
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public PlayerRoleArgParser PlayerRole(string argName) => new PlayerRoleArgParser(argName, api, true);

        /// <summary>
        /// Parses IPlayerRole, only works on Serverside since it needs the Serverconfig
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public PlayerRoleArgParser OptionalPlayerRole(string argName) => new PlayerRoleArgParser(argName, api, false);

        public PrivilegeArgParser Privilege(string privilege) =>
            new PrivilegeArgParser(privilege, api, true);

        public PrivilegeArgParser OptionalPrivilege(string privilege) =>
            new PrivilegeArgParser(privilege, api, false);

        public WordArgParser Word(string argName) => new WordArgParser(argName, true, null);

        public WordArgParser OptionalWord(string argName) => new WordArgParser(argName, false, null);

        public WordRangeArgParser OptionalWordRange(string argName, params string[] words) => new WordRangeArgParser(argName, false, words);

        public WordArgParser Word(string argName, string[] wordSuggestions) => new WordArgParser(argName, true, wordSuggestions);

        /// <summary>
        /// Parses a string which is either a color name or a hex value as a <see cref="System.Drawing.Color"/>
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public ColorArgParser Color(string argName) => new ColorArgParser(argName, true);

        /// <summary>
        /// Parses a string which is either a color name or a hex value as a <see cref="System.Drawing.Color"/>
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public ColorArgParser OptionalColor(string argName) => new ColorArgParser(argName, false);

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

        public Vec3iArgParser Vec3i(string argName) => new Vec3iArgParser(argName, api, true);

        public Vec3iArgParser OptionalVec3i(string argName) => new Vec3iArgParser(argName, api, true);

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
        public virtual string GetSyntaxUnformatted()
        {
            return isMandatoryArg ? "&lt;" + argName + "&gt;" : "[" + argName + "]";
        }
        public virtual string GetSyntaxExplanation(string indent)
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
                if (keyval.Length < 2) {
                    lastErrorMessage = "Invalid syntax. Needs to be a key=value pair, only single value found";
                    return null;
                }

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
            if (subargs == null) return EnumParseResult.Bad;

            Vec3d sourcePos = args.Caller.Pos;
            Entity callingEntity = args.Caller.Entity;

            float? range = null;
            if (subargs.TryGetValue("range", out string strrange)) range = strrange.ToFloat();

            AssetLocation type = null;
            if (subargs.TryGetValue("type", out string typestr))
            {
                type = new AssetLocation(typestr);
            }

            subargs.TryGetValue("name", out string name);

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

                    if (range == null && type == null && alive == null && name == null && !subargsstr.Equals("[!]"))
                    {
                        lastErrorMessage = "No selector defined, use e[!] to select all entities";
                        return EnumParseResult.Bad;
                    }

                    foreach (Entity e in entities)
                    {
                        if (range != null && e.Pos.DistanceTo(sourcePos) > range) continue;
                        if (type != null && !WildcardUtil.Match(type, e.Code)) continue;
                        if (alive != null && e.Alive != alive) continue;
                        if (name != null && !WildcardUtil.Match(name, e.GetName())) continue;

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

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            value = default;
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

        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + " is one of the registered entityTypes";
        }


        public override object GetValue()
        {
            return type;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            type = default;
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
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + " is either a player name, or else one of the following selection codes:\n" +
                indent + "  s[] for self\n" +
                indent + "  l[] for the entity currently looked at\n" +
                indent + "  p[] for all players\n" +
                indent + "  e[] with filters for entities, or e[!] for all entities.\n" +
                indent + "  Inside the square brackets, one or more filters can be added, to be more selective. Filters include name, type, class, tag, alive, range. Type can have wildcards. For example, <code>e[type=gazelle*,range=3,alive=true]</code>. The filters minx/miny/minz/maxx/maxy/maxz can also be used to specify a volume to search, coordinates are relative to the command caller's position.\n" +
                indent + "  This argument may be omitted if the remainder of the command makes sense, in which case it will be interpreted as self.";
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
            entities = default;
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

            if (maybeplayername.Equals("e[!]") && v == 'e')
            {
                args.RawArgs.PopWord();
                if (api.Side == EnumAppSide.Server) this.entities = (api as ICoreServerAPI).World.LoadedEntities.Values.ToArray();
                else this.entities = (api as ICoreClientAPI).World.LoadedEntities.Values.ToArray();
                return EnumParseResult.Good;
            }

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
                if (subargs == null) return EnumParseResult.Bad;
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

            if (subargs.Count == 0 && v == 'e')
            {
                lastErrorMessage = "No selector defined, use e[!] to select all entities";
                return EnumParseResult.Bad;
            }

            float? range = null;
            if (subargs.TryGetValue("range", out string strrange))
            {
                range = strrange.ToFloat();
                subargs.Remove("range");
            }

            AssetLocation type = null;
            if (subargs.TryGetValue("type", out string typestr))
            {
                type = new AssetLocation(typestr);
                subargs.Remove("type");
            }

            if (subargs.TryGetValue("class", out string classstr))
            {
                classstr = classstr.ToLowerInvariant();
                subargs.Remove("class");
            }

            if (subargs.TryGetValue("tag", out string tagstr))
            {
                tagstr = tagstr.ToLowerInvariant();
                subargs.Remove("tag");
            }

            if (subargs.TryGetValue("name", out string name))
            {
                subargs.Remove("name");
            }

            bool? alive = null;
            if (subargs.TryGetValue("alive", out var stralive))
            {
                alive = stralive.ToBool();
                subargs.Remove("alive");
            }

            long? id = null;
            if (subargs.TryGetValue("id", out var strid))
            {
                id = strid.ToLong();
                subargs.Remove("id");
            }

            Cuboidi box = null;
            if (sourcePos != null)
            {
                bool hasBox = false;
                string[] codes = { "minx", "miny", "minz", "maxx", "maxy", "maxz" };
                int[] values = new int[6];
                for (int i = 0; i < codes.Length; i++)
                {
                    if (subargs.TryGetValue(codes[i], out string val))
                    {
                        values[i] = val.ToInt() + i/3; // +1 for maxx, maxy and maxz
                        subargs.Remove(codes[i]);
                        hasBox = true;
                    }
                }
                if (hasBox)
                {
                    var center = sourcePos.AsBlockPos;
                    box = new Cuboidi(values).Translate(center.X, center.Y, center.Z);
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
                        if (entityMatches(plr.Entity, sourcePos, type, classstr, tagstr, range, box, name, alive, id))
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
                            if (entityMatches(e, sourcePos, type, classstr, tagstr, range, box, name, alive, id))
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
                            return entityMatches(e, sourcePos, type, classstr, tagstr, range, box, name, alive, id);
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
                    if (entityMatches(lookedAtEntity, sourcePos, type, classstr, tagstr, range, box, name, alive, id))
                    {
                        this.entities = new Entity[] { lookedAtEntity };
                    }
                    else
                    {
                        this.entities = Array.Empty<Entity>();
                    }
                    return EnumParseResult.Good;

                // Executing entity
                case 's':
                    if (entityMatches(callingEntity, sourcePos, type, classstr, tagstr, range, box, name, alive, id))
                    {
                        this.entities = new Entity[] { callingEntity };
                    }
                    else
                    {
                        this.entities = Array.Empty<Entity>();
                    }
                    return EnumParseResult.Good;

                default:
                    lastErrorMessage = "Wrong selector, needs to be a player name or p,e,l or s";
                    return EnumParseResult.Bad;
            }
        }

        private bool entityMatches(Entity e, Vec3d sourcePos, AssetLocation type, string classstr, string tagstr, float? range, Cuboidi box, string name, bool? alive, long? id)
        {
            if (id != null && e.EntityId != id) return false;
            if (range != null && e.SidedPos.DistanceTo(sourcePos) > range) return false;
            if (box != null && !box.ContainsOrTouches(e.SidedPos)) return false;
            if (classstr != null && classstr != e.Class.ToLowerInvariant()) return false;
            if (type != null && !WildcardUtil.Match(type, e.Code)) return false;
            if (alive != null && e.Alive != alive) return false;
            if (name != null && !WildcardUtil.Match(name, e.GetName())) return false;
            if (tagstr != null && !e.HasTags(tagstr)) return false;

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
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + Lang.Get("{0} is a precise {1} name, optionally immediately followed by braces {{}} containing attributes for the {1}", GetSyntax(), itemclass);
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

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            stack = default;
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
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + " is a world position. A world position can be either 3 coordinates, or a target selector.\n" +
                indent + "&nbsp;&nbsp;3 coordinates are specified as in the following examples:\n" +
                indent + "&nbsp;&nbsp;&nbsp;&nbsp;<code>100 150 -180</code> means 100 blocks East and 180 blocks North from the map center, with height 150 blocks\n" +
                indent + "&nbsp;&nbsp;&nbsp;&nbsp;<code>~-5 ~0 ~4</code> means 5 blocks West and 4 blocks South from the caller's position\n" +
                indent + "&nbsp;&nbsp;&nbsp;&nbsp;<code>=512100 =150 =511880</code> means the absolute x,y,z position specified (at default settings this is near the map center)\n\n" +
                indent + "&nbsp;&nbsp;A target selector is either a player's name (meaning that player's current position), or one of: <code>s[]</code> for self, <code>l[]</code> for looked-at entity or block, <code>p[]</code> for players, <code>e[]</code> for entities.\n" +
                indent + "One or more filters can be specified inside the brackets. For p[] or e[], the target will be the nearest player or entity which passes all the filters.\n" +
                indent + "Filters include name, type, class, alive, range. Type can use wildcards. For example, <code>e[type=gazelle*,range=3,alive=true]</code>. The filters minx/miny/minz/maxx/maxy/maxz can also be used to specify a volume to search, coordinates are relative to the command caller's position.\n";
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
            if (pos == null) lastErrorMessage = Lang.Get("Invalid position, must be 3 numbers");

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
            if (pos == null) lastErrorMessage = Lang.Get("Invalid position, must be 2 numbers");

            return pos == null ? EnumParseResult.Bad : EnumParseResult.Good;
        }
    }

    public class Vec3iArgParser : ArgumentParserBase
    {
        private Vec3i _vector;

        public Vec3iArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            argCount = 3;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            _vector = default;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            int? x = args.RawArgs.PopInt();
            int? y = args.RawArgs.PopInt();
            int? z = args.RawArgs.PopInt();

            if (x != null && y != null && z != null)
                _vector = new Vec3i((int)x, (int)y, (int)z);
            return _vector == null ? EnumParseResult.Bad : EnumParseResult.Good;
        }

        public override object GetValue()
        {
            return _vector;
        }

        public override void SetValue(object data)
        {
            _vector = (Vec3i)data;
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

        public override string GetSyntaxExplanation(string indent)
        {
            if(suggestions != null)
                return indent + GetSyntax() + " here are some suggestions: " + string.Join(", ", suggestions);

            return indent + GetSyntax() + " is a string without spaces";
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            word = default;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            word = args.RawArgs.PopWord();
            if (word == null)
            {
                lastErrorMessage = "Argument is missing";
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
        public override string GetSyntax()
        {
            string options = string.Join("/", words);
            return isMandatoryArg ? "<i>&lt;" + options + "&gt;</i>" : "<i>[" + options + "]</i>";
        }
        public override string GetSyntaxUnformatted()
        {
            string options = string.Join("/", words);
            return isMandatoryArg ? "&lt;" + options + "&gt;" : "[" + options + "]";
        }
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + (isMandatoryArg ? " is the " : " is (optionally) the ") + argName;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return words;
        }

        public override object GetValue()
        {
            return word;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            word = default;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            word = args.RawArgs.PopWord();
            if (!words.Contains(word))
            {
                word = null;
                lastErrorMessage = $"{Lang.Get("Invalid word, not in word range")} [{ string.Join(", ", words)}]";
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
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + " is the name or uid of one player, or a selector in this format: s[] for self, o[] for online players, a[] for all players." +
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

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            players = default;
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
                if (subargs == null) return EnumParseResult.Bad;
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
                        else if(plr.PlayerUID.Equals(name))
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
                    if (resp == EnumServerResponse.Good && !string.IsNullOrEmpty(uid))
                    {
                        onReady(new AsyncParseResults() { Status = EnumParseResultStatus.Ready, Data = new[] { new PlayerUidName(uid, name) } });
                    }
                    else
                    {
                        // name maybe a uid
                        api.PlayerData.ResolvePlayerUid(name, (resp, playername) =>
                        {
                            if (resp == EnumServerResponse.Good)
                            {
                                onReady(new AsyncParseResults() { Status = EnumParseResultStatus.Ready, Data = new[] { new PlayerUidName(name, playername) }});
                            }
                            else
                            {
                                lastErrorMessage = Lang.Get("No player with name or uid '{0}' exists", name);
                                onReady(new AsyncParseResults() { Status = EnumParseResultStatus.Error });
                            }
                        });
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

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            player = default;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            string playername = args.RawArgs.PopWord();
            if (playername == null)
            {
                lastErrorMessage = Lang.Get("Argument is missing");
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
        double value, defaultvalue;

        public DoubleArgParser(string argName, double min, double max, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.min = min;
            this.max = max;
        }
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + Lang.Get("{0} is a decimal number, for example 0.5", GetSyntax());
        }

        public DoubleArgParser(string argName, double defaultvalue, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.defaultvalue = defaultvalue;
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

        public override void PreProcess(TextCommandCallingArgs args)
        {
            value = defaultvalue;
            base.PreProcess(args);
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
        float defaultvalue;

        public FloatArgParser(string argName, float min, float max, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.min = min;
            this.max = max;
        }
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + " is a decimal number, for example 0.5";
        }

        public FloatArgParser(string argName, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            defaultvalue = 0f;
            min = float.MinValue;
            max = float.MaxValue;
        }

        public FloatArgParser(string argName, float defaultvalue, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.defaultvalue = defaultvalue;
            min = float.MinValue;
            max = float.MaxValue;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return new string[] { float.MinValue + "", float.MaxValue + "" };
        }

        public override object GetValue()
        {
            return value;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            value = defaultvalue;
            base.PreProcess(args);
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
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + " is a boolean, including 1 or 0, yes or no, true or false, or " + trueAlias;
        }

        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object data)
        {
            value = (bool)data;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            value = default;
            base.PreProcess(args);
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
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + " is an integer number";
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

    public class LongArgParser : ArgumentParserBase
    {
        long min, max;
        long value;
        long defaultValue;

        public LongArgParser(string argName, long min, long max, long defaultValue, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.defaultValue = defaultValue;
            this.min = min;
            this.max = max;
        }
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + " is an integer number";
        }

        public LongArgParser(string argName, long defaultValue, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.defaultValue = defaultValue;
            this.min = long.MinValue;
            this.max = long.MaxValue;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return new string[] { long.MinValue+"", long.MaxValue+"" };
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
            var val = args.RawArgs.PopLong();
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

            this.value = (long)val;
            return EnumParseResult.Good;
        }

        public override void SetValue(object data)
        {
            value = (long)data;
        }
    }

    public class DatetimeArgParser : ArgumentParserBase
    {
        DateTime datetime;
        List<string> timeUnits = new List<string>() { "minute", "hour", "day", "week", "year" };

        public DatetimeArgParser(string argName, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
        }
        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + " is a number and time, for example 1 day, where time can be any of [" + string.Join(",", timeUnits) + "]";
        }

        public override object GetValue()
        {
            return datetime;
        }

        public override void SetValue(object data)
        {
            datetime = (DateTime)data;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            datetime = default;
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

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            value = default;
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

    public class PlayerRoleArgParser : ArgumentParserBase
    {
        private readonly ICoreServerAPI _api;
        private IPlayerRole _value;
        public PlayerRoleArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            _api = api as ICoreServerAPI;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            _value = default;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            var roleString = args.RawArgs.PopWord();
            if (roleString == null)
            {
                lastErrorMessage = Lang.Get("Argument is missing");
                return EnumParseResult.Bad;
            }

            _value = _api.Server.Config.Roles.Find(
                grp => grp.Code.Equals(roleString, StringComparison.InvariantCultureIgnoreCase));
            if (_value == null)
            {
                lastErrorMessage = Lang.Get("No such role found: " + string.Join(", ", _api.Server.Config.Roles.Select(role => role.Code)));
            }


            return _value != null ? EnumParseResult.Good : EnumParseResult.Bad;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object data)
        {
            _value = (IPlayerRole)data;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return _api.Server.Config.Roles.Select(role => role.Code).ToArray();
        }
    }

    public class PrivilegeArgParser : ArgumentParserBase
    {
        private string Value;
        private ICoreServerAPI _api;
        public PrivilegeArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            if (api is ICoreServerAPI serverApi)
            {
                _api = serverApi;
            }
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            Value = default;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            var privilegeString = args.RawArgs.PopWord();
            if (privilegeString == null)
            {
                lastErrorMessage = Lang.Get("Argument is missing");
                return EnumParseResult.Bad;
            }

            if (_api != null)
            {
                var privileges = new HashSet<string>();
                _api.Server.Config.Roles.ForEach(r =>
                {
                    privileges.AddRange(r.Privileges);
                });
                Value = privileges.First(privilege => privilege.Equals(privilegeString, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                Value = Privilege.AllCodes().First(
                    privilege => privilege.Equals(privilegeString, StringComparison.InvariantCultureIgnoreCase));
            }
            if (Value == null)
            {
                lastErrorMessage = Lang.Get("No such privilege found: " + string.Join(", ", Privilege.AllCodes()));
            }


            return Value != null ? EnumParseResult.Good : EnumParseResult.Bad;
        }

        public override object GetValue()
        {
            return Value;
        }

        public override void SetValue(object data)
        {
            Value = (string)data;
        }

        public override string[] GetValidRange(CmdArgs args)
        {
            return Privilege.AllCodes();
        }
    }

    public class ColorArgParser : ArgumentParserBase
    {
        private Color _value;

        public ColorArgParser(string argName, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
        }

        public override string GetSyntaxExplanation(string indent)
        {
            return indent + GetSyntax() + " can be either a color string like (red, blue, green,.. <a href=\"https://learn.microsoft.com/en-us/dotnet/api/system.drawing.knowncolor?view=net-7.0\">See full list</a>) or a hex value like #F9D0DC";
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void PreProcess(TextCommandCallingArgs args)
        {
            base.PreProcess(args);
            _value = default;
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            var colorString = args.RawArgs.PopWord();
            if (colorString == null)
            {
                lastErrorMessage = Lang.Get("Not a color");
                return EnumParseResult.Bad;
            }
            if (colorString.StartsWith('#'))
            {
                try
                {
                    var argb = int.Parse(colorString.Replace("#", ""), NumberStyles.HexNumber);
                    _value = Color.FromArgb(argb);
                }
                catch (FormatException)
                {
                    lastErrorMessage = Lang.Get("command-waypoint-invalidcolor");
                    return EnumParseResult.Bad;
                }
            }
            else
            {
                _value = Color.FromName(colorString);
            }
            return EnumParseResult.Good;
        }

        public override void SetValue(object data)
        {
            _value = (Color)data;
        }
    }

    public class IsBlockArgParser : ArgumentParserBase
    {
        ICoreAPI api;
        int blockId;
        Vec3d pos;
        bool isFluid;
        Dictionary<string, string> subargs;

        public IsBlockArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, isMandatoryArg)
        {
            this.api = api;
        }

        public override object GetValue()
        {
            if (pos == null) return false;
            BlockPos bpos = pos.AsBlockPos;
            if (api.World.BlockAccessor.GetBlock(bpos, isFluid ? BlockLayersAccess.Fluid : BlockLayersAccess.Solid).Id != blockId) return false;
            if (subargs == null || subargs.Count == 0) return true;

            BlockEntity be = api.World.BlockAccessor.GetBlockEntity(bpos);
            if (be == null) return false;
            TreeAttribute tree = new TreeAttribute();
            be.ToTreeAttributes(tree);
            foreach (var pair in subargs)
            {
                if (!tree.HasAttribute(pair.Key)) return false;
                if (tree.GetAttribute(pair.Key).ToString() != pair.Value) return false;
            }
            return true;
        }

        public override void SetValue(object data)
        {
            throw new NotImplementedException();
        }

        public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
        {
            pos = null;
            if (args.RawArgs.Length != 4)
            {
                lastErrorMessage = "Required format: isBlock[code=...] x y z";
                return EnumParseResult.Bad;
            }

            if (args.RawArgs.PopUntil('[') != "isBlock")
            {
                lastErrorMessage = "Required format: isBlock[code=...] x y z";
                return EnumParseResult.Bad;
            }

            string subargsstr = args.RawArgs.PopCodeBlock('[', ']', out string errorMsg);
            if (errorMsg != null)
            {
                lastErrorMessage = errorMsg;
                return EnumParseResult.Bad;
            }

            if (subargsstr.Length > 2)
            {
                subargs = parseSubArgs(subargsstr);
                if (subargs == null) return EnumParseResult.Bad;

                if (!subargs.TryGetValue("code", out string code))
                {
                    lastErrorMessage = "Requires [code=...] to be specified";
                    return EnumParseResult.Bad;
                }

                Block block = api.World.GetBlock(new AssetLocation(code));
                if (block == null)
                {
                    lastErrorMessage = "Code " + code + " is not a valid block";
                    return EnumParseResult.Bad;
                }
                blockId = block.Id;
                isFluid = block.ForFluidsLayer;
                subargs.Remove("code");
            }

            var mapmiddlePos = new Vec3d(api.World.DefaultSpawnPosition.X, 0, api.World.DefaultSpawnPosition.Z);
            pos = args.RawArgs.PopFlexiblePos(args.Caller.Pos, mapmiddlePos);
            if (pos == null)
            {
                lastErrorMessage = Lang.Get("Invalid position, must be 3 numbers");
                return EnumParseResult.Bad;
            }

            return EnumParseResult.Good;
        }

        public static string Test(ICoreAPI api, Caller caller, string testcmd)
        {
            TextCommandCallingArgs packedArgs = new TextCommandCallingArgs()
            {
                Caller = caller,
                RawArgs = new CmdArgs(testcmd)
            };
            IsBlockArgParser blockCondParser = new IsBlockArgParser("cond", api, true);
            EnumParseResult bresult = blockCondParser.TryProcess(packedArgs);

            if (bresult == EnumParseResult.Bad) return blockCondParser.LastErrorMessage;

            return blockCondParser.TestCond();
        }

        private string TestCond()
        {
            if (pos == null) return "No position specified";
            BlockPos bpos = pos.AsBlockPos;
            Block bSolid = api.World.BlockAccessor.GetBlock(bpos, BlockLayersAccess.Solid);
            Block bFluid = api.World.BlockAccessor.GetBlock(bpos, BlockLayersAccess.Fluid);

            StringBuilder sb = new StringBuilder();
            if (bSolid.Id > 0) sb.AppendLine("Solid: " + bSolid.Code.ToShortString());
            if (bFluid.Id > 0) sb.AppendLine("Fluid: " + bFluid.Code.ToShortString());

            BlockEntity be = api.World.BlockAccessor.GetBlockEntity(bpos);
            if (be == null) sb.AppendLine("(no BlockEntity here)");
            else
            {
                TreeAttribute tree = new TreeAttribute();
                be.ToTreeAttributes(tree);
                foreach (var attr in tree)
                {
                    if ("posx".Equals(attr.Key)) continue;
                    if ("posy".Equals(attr.Key)) continue;
                    if ("posz".Equals(attr.Key)) continue;
                    sb.AppendLine("  " + attr.Key + "=" + attr.Value.ToString());
                }
            }

            return sb.ToString();
        }
    }

}
