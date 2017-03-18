using Xunit;
using StegoCore;
using System;

namespace StegoCoreTests
{
    public class EntryTests
    {
        [Fact]
        public void ConstructorFileSystem_FileNotFound()
        {
            System.IO.FileNotFoundException ex = Assert.Throws<System.IO.FileNotFoundException>(
                () =>
                {
                    new Stego(Guid.NewGuid().ToString());
                });
        }
    }
}