using ArtListVideoManager.Interfaces;
using ArtListVideoManager.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtListVideoManager.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoConverter _videoConverter;
        private readonly IVideoRepository _videoRepository;
        private readonly ILogger _logger;
        private readonly string _outputDir;
        private readonly string _videosBaseUrl;
        private readonly int[] _thumbnailsBySeconds;
        public VideoService(IVideoConverter videoConverterService, IWebHostEnvironment env, IVideoRepository videoRepository, IConfiguration configuration, ILogger<VideoService> logger)
        {
            _videoConverter = videoConverterService;
            _videoRepository = videoRepository;
            _logger = logger;
            _videosBaseUrl = configuration.GetValue<string>("VideosBaseUrl");
            var outputRelDir = configuration.GetValue<string>("StoredFilesPath");
            _outputDir = Path.Combine(env.ContentRootPath, outputRelDir);
            _thumbnailsBySeconds = configuration.GetSection("ThumbnailsBySeconds")?.AsEnumerable()
                .Where(p => p.Value != null)
                .Select(p => int.Parse(p.Value))
                .ToArray();
        }

        public async Task<ICollection<VideoModel>> GetVideos()
        {
            return await _videoRepository.GetVideos();
        }

        /// <summary>
        /// Convert and save the video in the requested format,
        /// extract thumbnails, save in DB and delete the oriinal file
        /// </summary>
        /// <param name="videoRequest"></param>
        /// <returns></returns>
        public async Task<VideoModel> AddVideo(AddVideoModel videoRequest)
        {
            try
            {
                //create a folder for the video
                var videoOutputDir = Path.Combine(_outputDir, videoRequest.Name);
                Directory.CreateDirectory(videoOutputDir);
                //list to hold all the video conversion tasks and execute them in parallel
                var tasks = new List<Task>();
                string convertedVideoPath = null;
                var thumbnails = new ConcurrentBag<string>();
                var convertVideoTask = Task.Run(async () => convertedVideoPath = await ConverVideo(videoRequest, videoOutputDir));
                tasks.Add(convertVideoTask);
                tasks.AddRange(CreateThumbnails(videoRequest, videoOutputDir, thumbnails));
                await Task.WhenAll(tasks.ToArray());

                //create a model from all the collected data
                var model = new VideoModel
                {
                    Name = videoRequest.Name,
                    Link = UrlCombine(_videosBaseUrl, videoRequest.Name, Path.GetFileName(convertedVideoPath)),
                    ThumbnailLinks = thumbnails
                            .Select(t => UrlCombine(_videosBaseUrl, videoRequest.Name,Path.GetFileName(t)))
                            .ToArray(),
                    VideoFormat = videoRequest.VideoFormat
                };
                //save the data in db
                var added = await _videoRepository.AddVideo(model);
                return added;
            }
            finally
            {
                try
                {
                    //delete the original file that was uploaded
                    File.Delete(videoRequest.FilePath);
                }
                catch (Exception e)
                {
                    _logger.LogError(videoRequest.FilePath + Environment.NewLine + e.ToString());
                }
            }
        }

        private Task<string> ConverVideo(AddVideoModel videoRequest, string videoOutputDir)
        {
            return _videoConverter.ConvertMedia(videoRequest.FilePath, videoRequest.Name, videoOutputDir, videoRequest.VideoFormat);
        }

        private IEnumerable<Task> CreateThumbnails(AddVideoModel videoRequest, string videoOutputDir, ConcurrentBag<string> thumbnails)
        {
            foreach (var thumbTime in _thumbnailsBySeconds)
            {
                var createThumbnailTask = Task.Run(async () =>
                {
                    var thumbPath = Path.Combine(videoOutputDir, $"thumbnail{thumbTime}.png");
                    await _videoConverter.SaveVideoThumbnail(videoRequest.FilePath, thumbPath, thumbTime);
                    thumbnails.Add(thumbPath);
                });
                yield return createThumbnailTask;
            }
        }

        private static string UrlCombine(params string[] urlParts)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < urlParts.Length; i++)
            {
                var part = urlParts[i];
                sb.Append(part.Trim('/'));
                sb.Append("/");
            }
            if (urlParts[0].StartsWith('/'))
                sb.Insert(0, '/');
            var res = sb.ToString().TrimEnd('/');
            return res;
        }
    }
}