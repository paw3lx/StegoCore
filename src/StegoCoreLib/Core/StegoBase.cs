namespace StegoCore.Core
{
    using System.Collections;
    using ImageSharp;
    using StegoCoreLib.Extensions;

    public abstract class StegoBase : IStegoEntry
    {
        protected Image image;
        protected BitArray secretDataBits;
        
        /// <summary>
        /// Get secret data representation in byte array
        /// </summary>
        /// <returns>Byte array</returns>
        public byte[] SecretDataBytes => secretDataBits.ToByteArray();

        public Image StegoImage => image;

        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    image?.Dispose();
                    secretDataBits = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StegoCoreBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void System.IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

    }
}