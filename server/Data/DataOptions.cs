namespace Documate.Data
{
    public class DataOptions
    {
        public string Connection{get;private set;}

        public DataOptions(string connection) {
            this.Connection = connection;
        }
    }
}