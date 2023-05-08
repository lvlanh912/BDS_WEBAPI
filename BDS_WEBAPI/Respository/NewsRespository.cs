using BDS_WEBAPI.IRespository;
using BDS_WEBAPI.Model;
using MongoDB.Driver;

namespace BDS_WEBAPI.Respository
{
    public class NewsRespository :Database, INewsRespository
    {
        private IMongoCollection<News>? News;
        public NewsRespository():base()
        {
            if(this._database!=null)
                News=this._database.GetCollection<News>("News");
        }
        public async Task<PagingResult<News>> GetAll(string? keywords, int pageindex, int pagesize)
        {
            var test = new PagingResult<News>();
            List<News> LUsers;
            //filter
            if (keywords == null)
            {
                LUsers = await News.Find(_ => true).ToListAsync();
            }
            else
                LUsers = await News.Find(filter: e => e.Title.Contains(keywords)).ToListAsync();
            test.TotalCount = LUsers.Count;
            var result = LUsers.Skip((pageindex - 1) * pagesize);
            test.Items = result.Take(pagesize);
            test.PageIndex = pageindex;
            test.PageSize = pagesize;

            //var a = await Users.Find(_ => true).ToListAsync();
            return test;
        }

        public async Task<News> GetbyId(string id)
        {
            return await News.Find(x => x._id == id).FirstOrDefaultAsync();
        }

        public Task DeletebyId(string id)
        {
            News.DeleteOne(e => e._id == id);
            return Task.FromResult("đã xoá");
        }

        public async Task<News> Insert(News entity)
        {
            News?.InsertOne(entity);
            return await Task.FromResult(entity);
        }

        public async Task<News> Update(News entity)
        {
            News.ReplaceOne(x => x._id == entity._id, entity);
            return await Task.FromResult(entity);
        }

        public async Task<bool> Exits(string id)
        {
            var rs = await News.FindSync(p => p._id == id).FirstOrDefaultAsync();
            if (rs != null)
                return true;
            return false;
        }

    }
}
