using BDS_WEBAPI.IRespository;
using BDS_WEBAPI.Model;
using MongoDB.Driver;

namespace BDS_WEBAPI.Respository
{
    public class PropertiesRespository : Database, IPropertiesRespository
    {
        private IMongoCollection<Properties>? Properties;
        public PropertiesRespository():base()
        {
            //this.database = new Database();
            if (_database != null)
                this.Properties = this._database.GetCollection<Properties>("Propeties");
        }
        public async Task<PagingResult<Properties>> GetAll(string? keywords, int pageindex, int pagesize)
        {
            var test = new PagingResult<Properties>();
            List<Properties> LUsers;
            //filter
            if (keywords == null)
            {
                LUsers = await Properties.Find(_ => true).ToListAsync();
            }
            else
                LUsers = await Properties.Find(filter: e => e.Title.Contains(keywords)||e.Address.Contains(keywords)||e._id.Contains(keywords)).ToListAsync();
            test.TotalCount = LUsers.Count;
            var result = LUsers.Skip((pageindex - 1) * pagesize);
            test.Items = result.Take(pagesize);
            test.PageIndex = pageindex;
            test.PageSize = pagesize;

            //var a = await Users.Find(_ => true).ToListAsync();
            return test;
        }
        public Task DeletebyId(string id)
        {
            Properties.DeleteOne(e => e._id == id);
            return  Task.FromResult("đã xoá");
        }
        public async Task<bool> Exits(string id)
        {
            var rs = await Properties.FindSync(p => p._id == id).FirstOrDefaultAsync();
            if (rs != null)
                return true;
            return false;
        }
        public async Task<Properties> GetbyId(string id)
        {
            return await Properties.Find(x => x._id == id).FirstOrDefaultAsync();
        }

        public async Task<Properties> Insert(Properties entity)
        {
            Properties?.InsertOne(entity);
            return await Task.FromResult(entity);
        }

        public async Task<Properties> Update(Properties entity)
        {
            Properties.ReplaceOne(x => x._id == entity._id, entity);
            return await Task.FromResult(entity);
        }
    }
}
