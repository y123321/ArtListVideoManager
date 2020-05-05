using ArtListVideoManager.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ArtListVideoManager.Interfaces
{
    public interface IVideoService
    {
        Task<VideoModel> AddVideo(AddVideoModel request);
        Task<ICollection<VideoModel>> GetVideos();
    }
}