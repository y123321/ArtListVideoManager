using ArtListVideoManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArtListVideoManager.DAL.Entities
{
    public class Video
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ThumbnailLink> ThumbnailLinks { get; set; }
        public string Link { get; set; }
        public VideoFormatEnum VideoFormat { get; set; }
    }
}
