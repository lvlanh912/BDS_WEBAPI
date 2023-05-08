using BDS_WEBAPI.IRespository;
using BDS_WEBAPI.Model;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BDS_WEBAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertiesRespository propertiesRespository;
        public PropertiesController (IPropertiesRespository _I)
        {
            propertiesRespository = _I;
        }
        [HttpGet("properties")]//d
        public async Task<ActionResult<PagingResult<Properties>>> GetALL(string? keyword,int page, int size)
        {
            try
            {
                if(keyword != null)
                {

                }
                var myModel = await propertiesRespository.GetAll(keyword,page, size);
                if (myModel == null)
                {
                    return NotFound();
                }
                return Ok(myModel);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi, ví dụ ghi log, trả về mã lỗi 500 Internal Server Error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("properties/{id}")]//d
        public async Task<ActionResult<Properties>> GetbyId(string id)//done
        {
            try
            {
                var myModel = await propertiesRespository.GetbyId(id);

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
        [HttpPost("properties")]
        public async Task<ActionResult> Insert([FromForm] Properties model)//done
        {
            try
            {
                if (model == null)
                {
                    return BadRequest();
                }
              ///////////  string path= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "config.txt");
                // Kiểm tra xem đối tượng đã tồn tại trong cơ sở dữ liệu hay chưa
                model._id = ObjectId.GenerateNewId().ToString();//tạo 1 id mới trong collection
                model.Status = 1;
                model.CreateAt = DateTime.Now;
                // Thêm đối tượng vào cơ sở dữ liệu
                await propertiesRespository.Insert(model);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("properties")]
        public async Task<ActionResult> Update(string id,[FromBody] Properties model)//done
        {
            try
            {
                if (model == null)
                {
                    return BadRequest();
                }
                // Kiểm tra xem đối tượng đã tồn tại trong cơ sở dữ liệu hay chưa
                if (await propertiesRespository.Exits(id))
                {
                    model._id= id;
                    var curent= await propertiesRespository.GetbyId(id);//dữ liệu bản ghi hiện tại
                    var newmodel = new Properties()
                    {
                        _id = id,
                        Title = model.Title ?? curent.Title,
                        Address = model.Address ?? curent.Address,
                        CreateAt = curent.CreateAt,
                        Description=model.Description?? curent.Description,
                        Status=model.Status?? curent.Status,
                        Images=model.Images?? curent.Images,
                        Price=model.Price??curent.Price

                    };
                    await propertiesRespository.Update(newmodel);
                    return StatusCode(202);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete("properties")]//d
        public async Task<ActionResult> Delete(string id)//done
        {
            try
            {
                if (id == null)
                {
                    return BadRequest();
                }
                // Kiểm tra xem đối tượng đã tồn tại trong cơ sở dữ liệu hay chưa
                if (await propertiesRespository.Exits(id))
                {
                    await propertiesRespository.DeletebyId(id);
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
