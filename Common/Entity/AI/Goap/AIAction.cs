using System.Collections.Generic;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    public enum EnumComparison {
        Exactly,
        Less,
        More
    }

    public enum EnumDataType
    {
        Bool,
        Int,
        Float,
        Double
    }

    public class PreCondition
    {
        EnumDataType type;
        object desiredValue;
        public EnumComparison comparison;

        public bool Satisfies(object currentValue)
        {
            switch (type)
            {
                case EnumDataType.Bool:
                    return (bool)desiredValue == (bool)currentValue;
                case EnumDataType.Int:
                    if (comparison == EnumComparison.Exactly)
                    {
                        return (int)desiredValue == (int)currentValue;
                    }
                    if (comparison == EnumComparison.Less)
                    {
                        return (int)desiredValue < (int)currentValue;
                    }
                    if (comparison == EnumComparison.More)
                    {
                        return (int)desiredValue > (int)currentValue;
                    }

                    break;
                case EnumDataType.Float:

                    if (comparison == EnumComparison.Exactly)
                    {
                        return (float)desiredValue == (float)currentValue;
                    }
                    if (comparison == EnumComparison.Less)
                    {
                        return (float)desiredValue < (float)currentValue;
                    }
                    if (comparison == EnumComparison.More)
                    {
                        return (float)desiredValue > (float)currentValue;
                    }

                    break;

                case EnumDataType.Double:
                    if (comparison == EnumComparison.Exactly)
                    {
                        return (double)desiredValue == (double)currentValue;
                    }
                    if (comparison == EnumComparison.Less)
                    {
                        return (double)desiredValue < (double)currentValue;
                    }
                    if (comparison == EnumComparison.More)
                    {
                        return (double)desiredValue > (double)currentValue;
                    }
                    break;
            }

            return false;
        }

        

        public static PreCondition IntCondition(int desiredValue, EnumComparison comparison)
        {
            return new PreCondition()
            {
                comparison = comparison,
                desiredValue = desiredValue,
                type = EnumDataType.Int
            };
        }

        public static PreCondition BoolCondition(bool desiredValue)
        {
            return new PreCondition()
            {
                desiredValue = desiredValue,
                type = EnumDataType.Bool
            };
        }

        public static PreCondition FloatCondition(float desiredValue, EnumComparison comparison)
        {
            return new PreCondition()
            {
                comparison = comparison,
                desiredValue = desiredValue,
                type = EnumDataType.Float
            };
        }

        public static PreCondition DoubleCondition(double desiredValue, EnumComparison comparison)
        {
            return new PreCondition()
            {
                comparison = comparison,
                desiredValue = desiredValue,
                type = EnumDataType.Double
            };
        }


    }

    public class AIAction
    {
        Dictionary<string, IAttribute> preConditions = new Dictionary<string, IAttribute>();
        Dictionary<string, IAttribute> effect = new Dictionary<string, IAttribute>();

        public virtual float Cost { get { return 1f; } }


        public virtual bool CanExecute(Dictionary<string, IAttribute> state)
        {
            foreach (var val in preConditions)
            {
                if (!state.ContainsKey(val.Key)) return false;

                if (val.Value is IntAttribute)
                {

                }

            }

            return true;
        }



    }
}
