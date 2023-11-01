using StegoCore;
using System;
using Xunit;

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
