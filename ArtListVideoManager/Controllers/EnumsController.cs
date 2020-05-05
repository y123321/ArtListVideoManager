using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArtListVideoManager.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArtListVideoManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumsController : ControllerBase
    {
        [HttpGet("VideoFormats")]
        public IActionResult VideoFormats()
        {
            return Ok(Enum.GetNames(typeof(VideoFormatEnum)));
        }
    }
}