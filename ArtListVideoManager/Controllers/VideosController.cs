using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArtListVideoManager.Filters;
using ArtListVideoManager.Helpers;
using ArtListVideoManager.Interfaces;
using ArtListVideoManager.Model;
using ArtListVideoManager.Services;
using HeyRed.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ArtListVideoManager.Controllers
{
    [Route("api/[controller]")]
    public class VideosController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly string _tempFilesPath;
        private readonly string[] _allowedExtensions;

        public VideosController(IVideoService videoService, IConfiguration configuration)
        {
            _videoService = videoService;
            _tempFilesPath = configuration.GetValue<string>("TempFilesPath");
            Directory.CreateDirectory(_tempFilesPath);
            _allowedExtensions = configuration.GetSection("PermittedExtensions")
                ?.AsEnumerable()
                ?.Select(v=>v.Value)
                ?.ToArray()??new string[0];

        }


        // GET: api/<controller>
        [HttpGet]
        [Produces(typeof(VideoModel[]))]
        public async Task<IActionResult> Get()
        {
            var videos = await _videoService.GetVideos();
            return Ok(videos);
        }

 

        //   POST api/<controller>
        [HttpPost]
        [DisableFormValueModelBinding]
        [RequestFormLimits(MultipartBodyLengthLimit = 20971520000)]
        [RequestSizeLimit(20971520000)]
        public async Task<IActionResult> Post()
        {

            if (!Request.HasFormContentType)
                return BadRequest();
            var model = await MultipartRequestHelper.HandleAddVideoRequest(Request, _tempFilesPath, ModelState, _allowedExtensions);
            if(!ModelState.IsValid)
                return ValidationProblem(ModelState);
            if (!TryValidateModel(model))
                return ValidationProblem(ModelState);
            await _videoService.AddVideo(model);
            return Ok();
        }

    }
}
