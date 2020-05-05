using ArtListVideoManager.DAL.Entities;
using ArtListVideoManager.Interfaces;
using ArtListVideoManager.Model;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtListVideoManager.DAL
{
    public class VideoRepository : IVideoRepository
    {
        private readonly VideoManagerContext _context;
        private readonly IMapper _mapper;

        public VideoRepository(VideoManagerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ICollection<VideoModel>> GetVideos()
        {
            var videosEntities = await _context.Videos
                                    .Include(v => v.ThumbnailLinks)
                                    .ToListAsync();
            var models = videosEntities
                            .Select(v => new VideoModel
                            {
                                Id = v.Id,
                                Link = v.Link,
                                ThumbnailLinks = v.ThumbnailLinks.Select(l => l.Link).ToArray(),
                                Name = v.Name,
                                VideoFormat = v.VideoFormat
                            }
                            ).ToArray();
            return models;
        }

        public async Task<VideoModel> AddVideo(VideoModel video)
        {
            var entity = _mapper.Map<Video>(video);
            var dbRes = await _context.Videos.AddAsync(entity);
            await _context.SaveChangesAsync();
            var res = _mapper.Map<VideoModel>(dbRes.Entity);
            return res;
        }
    }
}
