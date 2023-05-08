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
        private readonly IWebHostEnvironment?  host;
        private readonly IPropertiesRespository propertiesRespository;

        [Obsolete]
        public PropertiesController (IPropertiesRespository _I, IWebHostEnvironment _host)
        {
            this.host = _host;
            propertiesRespository = _I;
        }
        [HttpGet("properties")]//d
        public async Task<ActionResult<PagingResult<Properties>>> GetALL(string? keyword,int page, int size)
        {
            try
            {
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
        [HttpGet("images/{filename}")]
        public  ActionResult Getimage(string filename)
        {
            try
            {
                string imagePath = host.ContentRootPath + "Upload\\PropertiesIMG\\" + filename;
                if (!System.IO.File.Exists(imagePath))//nếu không tồn tại file ảnh
                {
                    return NotFound();

                }
                byte[] image= System.IO.File.ReadAllBytes(imagePath);
                return  File(image, "image/*");
            }
            catch
            {
                return NotFound();
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
        public async Task<ActionResult> Insert([FromForm] Image model)//done
        {
            try
            {
                if (model == null)
                {
                    return BadRequest();
                }
               
                // Kiểm tra xem đối tượng đã tồn tại trong cơ sở dữ liệu hay chưa
                if (!model.File.ContentType.StartsWith("image/"))
                {
                    //không phải là ảnh 
                    return BadRequest();           
                }
                //lưu ảnh
                model._id = ObjectId.GenerateNewId().ToString();
                string filename = model._id + Path.GetExtension(model.File.FileName);
                string path = Path.Combine(host.ContentRootPath+"Upload\\PropertiesIMG\\",filename);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }
                model.File = null;
                // Thêm đối tượng vào cơ sở dữ liệu
                await propertiesRespository.Insert(new Properties()
                {
                    _id=model._id,Title = model.Title,Description=model.Description,Address=model.Address,CreateAt=DateTime.Now,Images= filename,
                    Price=model.Price,Status=1
                });
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
                        Images=curent.Images,
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

       void SaveImage(byte img)
        {

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
