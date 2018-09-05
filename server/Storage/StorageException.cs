using System;

namespace Documate.Storage
{
    public class StorageException:Exception
    {
        public StorageException(string message) : base(message)
        {
        }
    }


}