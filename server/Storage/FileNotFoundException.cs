namespace Documate.Storage
{
    public class FileNotFoundException : StorageException
    {
        public FileNotFoundException(string message) : base(message)
        {
        }
    }
}