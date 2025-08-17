namespace StegoCore.Constants;

/// <summary>
/// Contains constant values used throughout the steganography library
/// </summary>
public static class StegoConstants
{
    /// <summary>
    /// The length of the secret data header in bits (used to store the data length)
    /// </summary>
    public const int SecretDataLengthBits = 32;

    /// <summary>
    /// The length of the secret data header in bytes
    /// </summary>
    public const int SecretDataLengthBytes = SecretDataLengthBits / 8;

    /// <summary>
    /// The number of bits per pixel used by the LSB algorithm (R and B channels)
    /// </summary>
    public const int LsbBitsPerPixel = 2;

    /// <summary>
    /// The capacity ratio for LSB algorithm (bits per pixel / 8 bits per byte)
    /// </summary>
    public const double LsbCapacityRatio = LsbBitsPerPixel / 8.0;

    /// <summary>
    /// The size of DCT blocks used by the Zhao-Koch algorithm
    /// </summary>
    public const int DctBlockSize = 8;

    /// <summary>
    /// The number of DCT blocks required per bit in Zhao-Koch algorithm
    /// </summary>
    public const int ZhaoKochBlocksPerBit = 1;

    /// <summary>
    /// The capacity ratio for Zhao-Koch algorithm (approximately 1 bit per 64 pixels)
    /// </summary>
    public const double ZhaoKochCapacityRatio = 1.0 / (DctBlockSize * DctBlockSize);

    /// <summary>
    /// Maximum supported image dimension (width or height)
    /// </summary>
    public const int MaxImageDimension = 65536;

    /// <summary>
    /// Minimum supported image dimension (width or height)
    /// </summary>
    public const int MinImageDimension = 8;
}