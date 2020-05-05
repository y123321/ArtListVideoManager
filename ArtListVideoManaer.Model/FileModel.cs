using System;
using System.Collections.Generic;
using System.Text;

namespace ArtListVideoManager.Model
{
    public class FileModel
    {
        public int Id { get; set; }

        public string Path { get; set; }

        public string UploadFileName { get; set; }


        public long Size { get; set; }

        public DateTime UploadDate { get; set; }
    }
}
