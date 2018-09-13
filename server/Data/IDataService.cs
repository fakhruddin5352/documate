using System.Threading.Tasks;
using Documate.Data;
using Documate.ValueObjects;

namespace Documate.Data
{
    public interface IDataService
    {
         Task Save(Document document);

         Task<Document> Load(Hash hash);
    }
}