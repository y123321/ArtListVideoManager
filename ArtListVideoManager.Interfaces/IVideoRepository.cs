using ArtListVideoManager.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArtListVideoManager.Interfaces
{
    public interface IVideoRepository
    {
        Task<VideoModel> AddVideo(VideoModel video);
        Task<ICollection<VideoModel>> GetVideos();
    }
}