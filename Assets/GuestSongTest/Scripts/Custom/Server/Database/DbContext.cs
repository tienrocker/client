#if !UNITY_5_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Photon.LoadBalancing.Custom.Server.Database
{
    public class DbContext : System.Data.Entity.DbContext
    {
        public DbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
        public DbContext() : base("name=MonkeyFist") { }

        public DbSet<Models.ModelUser> Users { get; set; }
        public DbSet<Models.ModelPlayList> PlayLists { get; set; }
        public DbSet<Models.ModelSong> Songs { get; set; }
        public DbSet<Models.ModelQuestion> Questions { get; set; }
    }
}
#endif