using System.Threading.Tasks;
using Documate.Data;

namespace Documate.Data
{
    public interface IDataService
    {
         Task Save(Document document);

         Task<Document> Load(string hash);
    }
}