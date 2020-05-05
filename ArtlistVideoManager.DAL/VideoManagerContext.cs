using ArtListVideoManager.DAL.Entities;
using ArtListVideoManager.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArtListVideoManager.DAL
{
    public class VideoManagerContext : DbContext
    {
        public VideoManagerContext(DbContextOptions<VideoManagerContext> options) : base(options)
        {

        }
        public VideoManagerContext() { }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<ThumbnailLink> ThumbnailLinks { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
