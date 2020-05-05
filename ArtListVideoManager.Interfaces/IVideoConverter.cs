using ArtListVideoManager.Model;
using System.Threading.Tasks;

namespace ArtListVideoManager.Interfaces
{
    public interface IVideoConverter
    {
        Task<string> ConvertMedia(string videoPath, string videoName, string outputDir, VideoFormatEnum format);
        Task<string> SaveVideoThumbnail(string videoPath, string outputPath, float frameTimeInSeconds);
    }
}