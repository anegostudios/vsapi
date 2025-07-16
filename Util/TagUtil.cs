using System.Collections.Generic;

namespace Vintagestory.API.Util;

public static class TagUtil
{
    /// <summary>
    /// Checks if two sorted arrays of tags have a common element.
    /// </summary>
    /// <param name="first">Must be sorted in ascending order</param>
    /// <param name="second">Must be sorted in ascending order</param>
    /// <returns></returns>
    public static bool Intersects(ushort[] first, ushort[] second)
    {
        int firstIndex = 0;
        int secondIndex = 0;

        while (firstIndex < first.Length && secondIndex < second.Length)
        {
            if (first[firstIndex] == second[secondIndex])
            {
                return true;
            }

            if (first[firstIndex] < second[secondIndex])
            {
                firstIndex++;
            }
            else
            {
                secondIndex++;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if all requirement tags are in the sample.
    /// </summary>
    /// <param name="requirement">Must be sorted in ascending order</param>
    /// <param name="sample">Must be sorted in ascending order</param>
    /// <returns></returns>
    public static bool ContainsAll(ushort[] requirement, ushort[] sample)
    {
        int requirementIndex = 0;
        int sampleIndex = 0;

        while (requirementIndex < requirement.Length && sampleIndex < sample.Length)
        {
            if (requirement[requirementIndex] == sample[sampleIndex])
            {
                requirementIndex++;
            }
            else if (requirement[requirementIndex] < sample[sampleIndex])
            {
                return false;
            }

            sampleIndex++;
        }

        return requirementIndex == requirement.Length;
    }

    /// <summary>
    /// Checks if sample has at least one tag from each requirement group.
    /// </summary>
    /// <param name="requirementGroups">Each group must be sorted in ascending order</param>
    /// <param name="sample">Must be sorted in ascending order</param>
    /// <returns></returns>
    public static bool IntersectsAll(IEnumerable<ushort[]> requirementGroups, ushort[] sample)
    {
        foreach (ushort[] requirement in requirementGroups)
        {
            if (!Intersects(requirement, sample))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if sample contains all tags from at least one requirement group.
    /// </summary>
    /// <param name="requirementGroups">Each group must be sorted in ascending order</param>
    /// <param name="sample">Must be sorted in ascending order</param>
    /// <returns></returns>
    public static bool ContainsAllFromAtLeastOne(IEnumerable<ushort[]> requirementGroups, ushort[] sample)
    {
        foreach (ushort[] requirement in requirementGroups)
        {
            if (ContainsAll(requirement, sample))
            {
                return true;
            }
        }

        return false;
    }
}
