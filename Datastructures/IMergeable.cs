namespace Vintagestory.API.Datastructures
{
    public interface IMergeable<T>
    {
        bool MergeIfEqual(T target);
    }
}
