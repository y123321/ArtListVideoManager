using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtListVideoManager.Model
{
    public class VideoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<string> ThumbnailLinks { get; set; }
        public string Link { get; set; }
        public VideoFormatEnum VideoFormat { get; set; }
    }
}
