# StegoCore

A high-performance **steganography library** for .NET 8 that lets you **hide secret data inside images** without anyone knowing it's there.

[![Build and Test](https://github.com/paw3lx/StegoCore/actions/workflows/ci-build-and-test.yml/badge.svg?branch=master)](https://github.com/paw3lx/StegoCore/actions/workflows/ci-build-and-test.yml)
[![NuGet Version](https://img.shields.io/nuget/v/StegoCore.svg)](https://www.nuget.org/packages/StegoCore/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/StegoCore.svg)](https://www.nuget.org/packages/StegoCore/)

## What is Steganography?

Steganography is the practice of hiding secret information within ordinary, non-secret files or messages. Unlike encryption, which scrambles data, steganography conceals the very existence of the data. With StegoCore, you can:

- 🔒 **Hide sensitive files** inside innocent-looking images
- 📱 **Secure communication** by embedding messages in photos
- 🔐 **Digital watermarking** for copyright protection
- 🕵️ **Covert data storage** for security applications

## Why StegoCore?

✅ **High Performance** - 4.6x faster than previous versions  
✅ **.NET 8 Native** - Modern, optimized for latest runtime  
✅ **Memory Efficient** - 12.3x less memory allocation  
✅ **Multiple Algorithms** - LSB and Zhao-Koch implementations  
✅ **Easy to Use** - Simple, intuitive API  
✅ **Well Tested** - Comprehensive test suite and benchmarks  

## Quick Start

### Installation

```bash
# Package Manager Console
Install-Package StegoCore

# .NET CLI
dotnet add package StegoCore
```

### Basic Usage

**Hide a secret message in an image:**

```csharp
using StegoCore;
using StegoCore.Algorithms;

// Load your secret data
byte[] secretData = System.Text.Encoding.UTF8.GetBytes("This is my secret message!");

// Hide it in an image
using var stego = new Stego("cover-image.jpg");
stego.SetSecretData(secretData);
var stegoImage = stego.Embed(AlgorithmEnum.Lsb);

// Save the image with hidden data
stegoImage.Save("image-with-secret.png");
```

**Extract the secret from the image:**

```csharp
using StegoCore;
using StegoCore.Algorithms;

// Load image containing secret data
using var stego = new Stego("image-with-secret.png");

// Extract the hidden data
byte[] recoveredSecret = stego.Decode(AlgorithmEnum.Lsb);
string message = System.Text.Encoding.UTF8.GetString(recoveredSecret);

Console.WriteLine($"Secret message: {message}");
// Output: Secret message: This is my secret message!
```

### Real-World Example: Hide a File

```csharp
using StegoCore;
using StegoCore.Algorithms;
using StegoCore.Model;

// Hide an entire file inside an image
byte[] fileToHide = File.ReadAllBytes("confidential-document.pdf");

using var stego = new Stego("family-photo.jpg");
stego.SetSecretData(fileToHide);

// Add password protection
stego.SetSettings(new Settings { Key = "my-secret-password" });

var stegoImage = stego.Embed(AlgorithmEnum.Lsb);
stegoImage.Save("family-photo-with-secret.png");

// Later, extract the file
using var extractStego = new Stego("family-photo-with-secret.png");
extractStego.SetSettings(new Settings { Key = "my-secret-password" });

byte[] recoveredFile = extractStego.Decode(AlgorithmEnum.Lsb);
File.WriteAllBytes("recovered-document.pdf", recoveredFile);
```

## Algorithms

### LSB (Least Significant Bit) - Recommended
- **Speed**: ⚡ Very Fast (optimized for .NET 8)
- **Capacity**: High data capacity
- **Detection**: Low visibility to naked eye
- **Best for**: Most use cases, large amounts of data

### Zhao-Koch DCT-based
- **Speed**: 🐌 Slower but robust
- **Capacity**: Lower data capacity
- **Detection**: More resistant to image analysis
- **Best for**: When maximum security is needed

```csharp
// Use specific algorithms
stego.Embed(AlgorithmEnum.Lsb);      // Fast, high capacity
stego.Embed(AlgorithmEnum.ZhaoKoch); // Slower, more secure
```

## Advanced Features

### Password Protection

```csharp
// Secure your hidden data with a password
stego.SetSettings(new Settings 
{ 
    Key = "your-secure-password-123" 
});
```

### Capacity Check

```csharp
// Check if your data fits in the image
byte[] data = File.ReadAllBytes("large-file.zip");
using var stego = new Stego("small-image.jpg");

if (stego.CanEmbed(data.Length, AlgorithmEnum.Lsb))
{
    // Safe to embed
    stego.SetSecretData(data);
    var result = stego.Embed(AlgorithmEnum.Lsb);
}
else
{
    Console.WriteLine("Image too small for this data!");
}
```

## Supported Image Formats

**Input formats** (for cover images):
- JPEG (.jpg, .jpeg)
- PNG (.png) 
- BMP (.bmp)
- TIFF (.tiff, .tif)
- GIF (.gif)

**Output formats** (recommended):
- PNG - Best for preserving hidden data
- BMP - Highest fidelity, larger files
- TIFF - Good compromise

> ⚠️ **Important**: Avoid saving steganographic images as JPEG, as compression may damage hidden data.

## Performance

StegoCore v1.0.0 delivers significant performance improvements:

| Algorithm | Speed Improvement | Memory Reduction | 
|-----------|-------------------|------------------|
| LSB       | 4.6x faster      | 12.3x less RAM   |
| Zhao-Koch | Similar          | 15% improvement  |

Run benchmarks yourself:
```bash
cd tests/StegoBenchmark
dotnet run -c Release
```

## FAQ

**Q: Can anyone detect that an image contains hidden data?**  
A: With proper techniques and the right algorithm (Zhao-Koch), it's very difficult to detect. LSB is harder to detect in complex images.

**Q: How much data can I hide?**  
A: Roughly 1 byte per 8 pixels for LSB. A 1920x1080 image can hide ~250KB of data.

**Q: Will the image quality change?**  
A: Changes are minimal and typically imperceptible to human eyes.

**Q: Is this cryptographically secure?**  
A: StegoCore provides *concealment*, not encryption. For security, encrypt your data before hiding it.

## What's New in v1.0.0

🚀 **Major Performance Improvements**
- **4.6x faster** LSB algorithm performance
- **12.3x less** memory allocation
- Updated to **.NET 8** with enhanced nullable reference types
- **ImageSharp 3.1.11** for the latest image processing optimizations
- Comprehensive **benchmarking infrastructure** with BenchmarkDotNet

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues.

## License

This project is open source. Check the repository for license details.

## Links

- 📦 [NuGet Package](https://www.nuget.org/packages/StegoCore/)
- 🐛 [Report Issues](https://github.com/paw3lx/StegoCore/issues)
- 📖 [Project Blog](https://pawelskaruz.pl/category/daj-sie-poznac-2017/)
