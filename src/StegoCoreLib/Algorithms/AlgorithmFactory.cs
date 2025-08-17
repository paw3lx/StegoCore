using System;
using StegoCore.Exceptions;

namespace StegoCore.Algorithms;

/// <summary>
/// Factory class for creating steganography algorithm instances
/// </summary>
public static class AlgorithmFactory
{
    /// <summary>
    /// Creates an instance of the specified steganography algorithm
    /// </summary>
    /// <param name="selection">The algorithm to create</param>
    /// <returns>An instance of the requested algorithm</returns>
    /// <exception cref="InvalidAlgorithmException">Thrown when the algorithm is not supported or cannot be created</exception>
    public static StegoAlgorithm Create(AlgorithmEnum selection)
    {
        try
        {
            Type? type = Type.GetType(typeof(StegoAlgorithm).Namespace + "." + selection.ToString(), throwOnError: false);

            if (type is null)
            {
                throw new InvalidAlgorithmException($"Algorithm '{selection}' is not implemented or not found", selection.ToString());
            }

            if (!typeof(StegoAlgorithm).IsAssignableFrom(type))
            {
                throw new InvalidAlgorithmException($"Type '{type.Name}' does not inherit from StegoAlgorithm", selection.ToString());
            }

            StegoAlgorithm? instance = Activator.CreateInstance(type) as StegoAlgorithm;
            if (instance is null)
            {
                throw new InvalidAlgorithmException($"Failed to create instance of algorithm '{selection}'", selection.ToString());
            }

            return instance;
        }
        catch (Exception ex) when (ex is not InvalidAlgorithmException)
        {
            throw new InvalidAlgorithmException($"Unexpected error creating algorithm '{selection}': {ex.Message}", selection.ToString(), ex);
        }
    }
}
