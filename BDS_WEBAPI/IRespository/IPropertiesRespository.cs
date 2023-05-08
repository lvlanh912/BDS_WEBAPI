using BDS_WEBAPI.Model;

namespace BDS_WEBAPI.IRespository
{
    public interface IPropertiesRespository
    {
        Task<PagingResult<Properties>> GetAll(string? keywords, int pageindex, int pagesize);
        Task<Properties> GetbyId(string id);
        Task DeletebyId(string id);
        Task<Properties> Insert(Properties entity);
        Task<Properties> Update(Properties entity);
        Task<bool> Exits(string id);
    }
}
