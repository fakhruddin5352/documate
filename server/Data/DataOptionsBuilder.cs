namespace Documate.Data
{
    public class DataOptionsBuilder
    {
        public string Connection{get;private set;}

        public DataOptionsBuilder UseConnection(string connection){
            return new DataOptionsBuilder{Connection = connection};
        }
    }
}