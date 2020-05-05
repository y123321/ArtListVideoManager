using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArtListVideoManager.DAL.Entities
{
    public class ThumbnailLink
    {
        [Key]
        public int Id { get; set; }
        public string Link { get; set; }
    }
}
