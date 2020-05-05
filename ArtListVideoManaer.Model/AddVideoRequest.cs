using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArtListVideoManager.Model
{
    public class AddVideoModel
    {
        [Required]
        //max length of file is 255, 5 chars is the . and extension 
        [MaxLength(255-5)]
        public string Name { get; set; }
        [Required]
        public VideoFormatEnum VideoFormat { get; set; }
        [Required]
        public string FilePath { get; set; }
    }
}
