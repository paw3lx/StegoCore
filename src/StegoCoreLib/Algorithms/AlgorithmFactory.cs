using System;
using System.Reflection;

namespace StegoCore.Algorithms;

public static class AlgorithmFactory
{
    public static StegoAlgorithm Create(AlgorithmEnum selection)
    {
        var type = Type.GetType(typeof(StegoAlgorithm).Namespace + "." + selection.ToString(), throwOnError: false);

        if (type == null)
        {
            throw new NullReferenceException(selection.ToString() + " is not a known algorithm type");
        }

        if (!typeof(StegoAlgorithm).IsAssignableFrom(type))
        {
            throw new InvalidOperationException(type.Name + " does not inherit from StegoAlghorithm");
        }

        return (StegoAlgorithm)Activator.CreateInstance(type);
    }

}
