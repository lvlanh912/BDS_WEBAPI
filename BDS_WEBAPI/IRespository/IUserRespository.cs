using BDS_WEBAPI.Model;

namespace BDS_WEBAPI.IRespository
{
    public interface IUserRespository
    {
        Task<PagingResult<Users>> GetAll(string? keywords,int pageindex, int pagesize);
        Task<Users> GetbyId(string id);
        Task<Users> GetbyUsername(string id);
        Task DeletebyId(string id);
        Task<Users> InsertMember(Users entity);
        Task<Users> Update(Users entity);
        Task<bool> Exits(Users entity);
        Task<bool> Exits(string id);
    }
}
