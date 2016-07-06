namespace Photon.LoadBalancing.Custom.Models
{
#if !UNITY_5_3_OR_NEWER
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("playlists")]
#endif
    public class ModelPlayList
    {
#if !UNITY_5_3_OR_NEWER
        [Key]
        [Column("id")]
#endif
        public int id { get; set; }

        public string name { get; set; }

        public string slug { get; set; }

        public bool enable { get; set; }

        public int start { get; set; }

        public int end { get; set; }

        public PlaylistType type { get; set; } // 0: free | 1: premium

        public int price { get; set; }

        public int created_at { get; set; }

        public enum PlaylistType : int { FREE, PREMIUM }
    }
}