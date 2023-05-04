using BDS_WEBAPI.Model;

namespace BDS_WEBAPI.IRespository
{
    public interface IPropertiesRespository
    {
        Task<IEnumerable<Properties>> GetAll();
        Task<Properties> GetbyId(string id);
        Task DeletebyId(string id);
        Task<Properties> Insert(Properties entity);
        Task<Properties> Update(Properties entity);
        Task<bool> Exits(Properties entity);
    }
}
