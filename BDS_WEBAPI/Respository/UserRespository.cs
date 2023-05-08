using BDS_WEBAPI.IRespository;
using BDS_WEBAPI.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BDS_WEBAPI.Respository
{
    public class UserRespository: Database,IUserRespository
    {
        private readonly IMongoCollection<Users>? Users;
        public UserRespository():base()
        {
            if (_database != null)
            {
                this.Users = _database.GetCollection<Users>("Users");//lấy danh sách các bản ghi trong bảng SinhVien
            }
        }
        public Task DeletebyId(string id)
        {
            Users.DeleteOne(e => e._id ==id);
            return Task.FromResult("đã xoá");
        }

        public async Task<bool> Exits(Users entity)
        {
            var rs = await Users.FindSync(p => p.Username == entity.Username || p._id == entity._id).FirstOrDefaultAsync();
            if (rs != null)
                return true;
            return false;
        }
        public async Task<bool> Exits(string id)
        {
            var rs = await Users.FindSync(p=> p._id ==id).FirstOrDefaultAsync();
            if (rs != null)
                return true;
            return false;
        }

        public async Task<PagingResult<Users>> GetAll(string? keywords,int pageindex, int pagesize)
        {
            var test= new PagingResult<Users>();
            List<Users> LUsers;
            //filter
            if (keywords == null)
            {
                LUsers = await Users.Find(_ => true).ToListAsync();
            }
            else
            LUsers = await Users.Find(filter: e => e.Username.Contains(keywords) || e.Email.Contains(keywords) || e.Fullname.Contains(keywords)).ToListAsync();
            test.TotalCount = LUsers.Count;
            var result = LUsers.Skip((pageindex - 1) * pagesize);
            test.Items = result.Take(pagesize);
            test.PageIndex = pageindex;
            test.PageSize = pagesize;
           
            //var a = await Users.Find(_ => true).ToListAsync();
            return test;
        }
        public async Task<Users> GetbyId(string id)
        {
            return await Users.Find(x => x._id ==id).FirstOrDefaultAsync();
        }
        public async Task<Users> GetbyUsername(string username)
        {
            return await Users.Find(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<Users> InsertMember(Users entity)
        {
            entity.CreateDate = DateTime.Now;
            entity.Role = 2;
            Users?.InsertOne(entity);
            return await Task.FromResult(entity);
        }

        public async Task<Users> Update(Users entity)
        {
            Users.ReplaceOne(x => x._id == entity._id, entity);
            return  await Task.FromResult(entity);
        }
    }
}
