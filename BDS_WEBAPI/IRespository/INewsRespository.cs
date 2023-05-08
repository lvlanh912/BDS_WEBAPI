using BDS_WEBAPI.Model;

namespace BDS_WEBAPI.IRespository
{
    public interface INewsRespository
    {
        Task<PagingResult<News>> GetAll(string? keywords, int pageindex, int pagesize);
        Task<News> GetbyId(string id);
        Task DeletebyId(string id);
        Task<News> Insert(News entity);
        Task<News> Update(News entity);
        Task<bool> Exits(string id);
    }
}
