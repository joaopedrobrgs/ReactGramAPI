using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReactGramAPI.Models;

namespace ReactGramAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PhotoController : ControllerBase
{

    [HttpPost]
    [Authorize]
    [Route("upload")]
    public IActionResult UploadPhoto([FromForm] Photo photo)
    {
        try
        {
            string path = System.IO.Path.Combine(Utils.Utils.GetPublicDir("photos"), photo.FileName);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                photo.File.CopyTo(stream);
            }
            return Ok("Foto salva com sucesso");
        }
        catch(Exception ex) 
        {
            throw new Exception(ex.Message);
        }
    }

}
