namespace Documate.Data
{
    public sealed class DataOptionsBuilder
    {
        public string Connection{get;private set;}

        public DataOptionsBuilder UseConnection(string connection){
            this.Connection = connection;
            return this;

        }

        internal DataOptions Build() {
            return new DataOptions(Connection);
        }
    }
}