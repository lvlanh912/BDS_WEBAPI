using BDS_WEBAPI.IRespository;
using BDS_WEBAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BDS_WEBAPI.Controllers
{
    
    [Route("api")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsRespository newsRespository;
        public NewsController (INewsRespository _I)
        {
            newsRespository = _I;
        }
        [HttpGet("news")]
        public async Task<ActionResult<PagingResult<News>>> GetALL(string? keyword,int page, int size)
        {
            try
            {
                var myModel = await newsRespository.GetAll(keyword,page, size);
                if (myModel == null)
                {
                    return NotFound();
                }
                return Ok(new ResponseAPI<PagingResult<News>>(true,"success",myModel));
            }
            catch (Exception ex)
            {
                // Xử lý lỗi, ví dụ ghi log, trả về mã lỗi 500 Internal Server Error
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = "admin")]
        [HttpGet("news/{id}")]
        public async Task<ActionResult<News>> GetbyId(string id)//done
        {
            try
            {
                
                var myModel = await newsRespository.GetbyId(id);

                if (myModel == null)
                {
                    return NotFound();
                }

                return Ok(myModel);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi, ví dụ ghi log, trả về mã lỗi 500 Internal Server Error
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "admin")]//yêu cầu token của role admin
        [HttpPost("news")]
        public async Task<ActionResult> Insert([FromBody] News model)//done
        {
            try
            {
                if (model == null)
                {
                    return BadRequest();
                }
                // Kiểm tra xem đối tượng đã tồn tại trong cơ sở dữ liệu hay chưa
                model._id = ObjectId.GenerateNewId().ToString();//tạo 1 id mới trong collection
                model.Date_Public = DateTime.Now;
                model.By = "admin";
                // Thêm đối tượng vào cơ sở dữ liệu
                await newsRespository.Insert(model);
                return StatusCode(201, model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("news")]
        public async Task<ActionResult> Update(string id,[FromBody] News model)//done
        {
            try
            {
                if (model == null)
                {
                    return BadRequest();
                }
                // Kiểm tra xem đối tượng đã tồn tại trong cơ sở dữ liệu hay chưa
                if (await newsRespository.Exits(id))
                {
                    model._id= id;
                    var curent= await newsRespository.GetbyId(id);//dữ liệu bản ghi hiện tại
                    var newmodel = new News()
                    {
                        _id= id,
                        Title = model.Title ?? curent.Title,
                        content =model.content?? curent.content,
                        By=curent.By,
                        Date_Public=curent.Date_Public
                    };
                    await newsRespository.Update(newmodel);
                    return StatusCode(202);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete("news")]
        public async Task<ActionResult> Delete(string id)//done
        {
            try
            {
                if (id == null)
                {
                    return BadRequest();
                }
                // Kiểm tra xem đối tượng đã tồn tại trong cơ sở dữ liệu hay chưa
                if (await newsRespository.Exits(id))
                {
                    await newsRespository.DeletebyId(id);
                    return Ok("deleted");
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
