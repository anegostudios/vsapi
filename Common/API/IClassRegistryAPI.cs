using System;
using System.Collections.Generic;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Interface for creating instances
    /// </summary>
    public interface IClassRegistryAPI
    {
        Dictionary<string, Type> BlockClassToTypeMapping { get; }
        Dictionary<string, Type> ItemClassToTypeMapping { get; }

        string GetBlockBehaviorClassName(Type blockBehaviorType);
        string GetCollectibleBehaviorClassName(Type blockBehaviorType);

        /// <summary>
        /// Creates a block instance from given block class 
        /// </summary>
        /// <param name="blockclass"></param>
        /// <returns></returns>
        Block CreateBlock(string blockclass);

        /// <summary>
        /// Returns the type of the registered block class or null otherwise
        /// </summary>
        /// <param name="blockclass"></param>
        /// <returns></returns>
        Type GetBlockClass(string blockclass);

        /// <summary>
        /// Creates a block entity instance from given block entity class 
        /// </summary>
        /// <param name="blockEntityClass"></param>
        /// <returns></returns>
        BlockEntity CreateBlockEntity(string blockEntityClass);

        /// <summary>
        /// Creates a entity instance from given entity class 
        /// </summary>
        /// <param name="entityClass"></param>
        /// <returns></returns>
        Entity CreateEntity(string entityClass);

        /// <summary>
        /// Creates a entity instance from given entity type 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        Entity CreateEntity(EntityProperties entityType);

        /// <summary>
        /// Creates an instance of a mountable that has been registered with api.RegisterMountable
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        IMountableSeat GetMountable(TreeAttribute tree);
        

        /// <summary>
        /// Creates a block behavior instance from given behavior code
        /// </summary>
        /// <param name="forBlock"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        BlockBehavior CreateBlockBehavior(Block forBlock, string code);

        /// <summary>
        /// Creates a collectible behavior instance from given behavior code
        /// </summary>
        /// <param name="forCollectible"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        CollectibleBehavior CreateCollectibleBehavior(CollectibleObject forCollectible, string code);

        /// <summary>
        /// Returns the block entity behavior type if such is registered under supplied name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Type GetBlockEntityBehaviorClass(string name);

        /// <summary>
        /// Creates a block entity behavior instance for given block entity
        /// </summary>
        /// <param name="blockEntity"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        BlockEntityBehavior CreateBlockEntityBehavior(BlockEntity blockEntity, string name);

        /// <summary>
        /// Returns the block behavior type registered for given name or null
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Type GetBlockBehaviorClass(string code);

        /// <summary>
        /// Returns the collectible behavior type registered for given name or null
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Type GetCollectibleBehaviorClass(string code);

        /// <summary>
        /// Creates a block behavior instance from given block class 
        /// </summary>
        /// <param name="forEntity"></param>
        /// <param name="entityBehaviorName"></param>
        /// <returns></returns>
        EntityBehavior CreateEntityBehavior(Entity forEntity, string entityBehaviorName);

        Type GetEntityBehaviorClass(string entityBehaviorName);


        IInventoryNetworkUtil CreateInvNetworkUtil(InventoryBase inv, ICoreAPI api);


        /// <summary>
        /// Creates an item instance from given item class 
        /// </summary>
        /// <param name="itemclass"></param>
        /// <returns></returns>
        Item CreateItem(string itemclass);


        /// <summary>
        /// Gets the registered item type or null if not registered
        /// </summary>
        /// <param name="itemClass"></param>
        /// <returns></returns>
        Type GetItemClass(string itemClass);
        
        /// <summary>
        /// Creates a json serializable version of an ITreeAttribute
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        JsonTreeAttribute CreateJsonTreeAttributeFromDict(Dictionary<string, JsonTreeAttribute> attributes);

        /// <summary>
        /// Returns the type for given BlockEntity class name as register in the ClassRegistry
        /// </summary>
        /// <param name="bockEntityClass"></param>
        /// <returns></returns>
        Type GetBlockEntity(string bockEntityClass);

        /// <summary>
        /// Returns the block entity class code if give Type is a registered block entity class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetBlockEntityClass(Type type);

        /// <summary>
        /// Returns the entity class code if give Type is a registered entity class
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        string GetEntityClassName(Type entityType);

        /// <summary>
        /// Creates a crop behavior instance from given block class 
        /// </summary>
        /// <param name="forBlock"></param>
        /// <param name="cropBehaviorName"></param>
        /// <returns></returns>
        CropBehavior CreateCropBehavior(Block forBlock, string cropBehaviorName);


        void RegisterParticlePropertyProvider(string className, Type ParticleProvider);
        IParticlePropertiesProvider CreateParticlePropertyProvider(Type entityType);

        IParticlePropertiesProvider CreateParticlePropertyProvider(string className);

    }
}
