StegoCore is steganography library for .NET Core. Hide secret data inside images using many algorithms.

[![Build and Test](https://github.com/paw3lx/StegoCore/actions/workflows/ci-build-and-test.yml/badge.svg?branch=master)](https://github.com/paw3lx/StegoCore/actions/workflows/ci-build-and-test.yml)

More info about this project on https://pawelskaruz.pl/category/daj-sie-poznac-2017/

## Installation

StegoCore is available on [nuget](https://www.nuget.org/packages/StegoCore/) and [MyGet (dev build)](https://www.myget.org/feed/stegocore/package/nuget/StegoCore).


### Package manager
```bash
Install-Package StegoCore -Version 0.3.1
```

### .NET CLI
```bash
dotnet add package StegoCore --version 0.3.1
```

## Getting started
 
StegoCore is using ImaheSharp as image processing library. 

To hide some secret data inside an image do following:

```cs
byte[] secretData = System.IO.File.ReadAllBytes("secret.data");
using(var stego = new Stego("someimage.jpg"))
{
    stego.SetSecretData(fileBytes);
    Image<Rgba32> secretImage = stego.Embed(AlgorithmEnum.Lsb);
}
```

Pretty simple, right? :) Now you can save the image with secret. But how to extract secret from image? It's even simpler.

```cs
using(var stego = new Stego("secretImage.jpg"))
{
    byte[] secret = stego.Decode(AlgorithmEnum.Lsb);
}
```

Right now there are 2 steganography algorithms implemented:
- LSB (least significant bit)
- Zhao & Koch (algorithm based on DCT)

These algorithms can be parameterized. You can pass a key parameter, which will be used as random seed to determine where to hide secret data:

```cs
stego.SetSettings(new StegoCore.Model.Settings
    {
        Key = "aaa"
    });
```