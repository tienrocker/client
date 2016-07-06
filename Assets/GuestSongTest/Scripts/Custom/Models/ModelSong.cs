namespace Photon.LoadBalancing.Custom.Models
{
#if !UNITY_5_3_OR_NEWER
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("songs")]
#endif
    public class ModelSong
    {
#if !UNITY_5_3_OR_NEWER
        [Key]
        [Column("id")]
#endif
        public int id { get; set; }

        public string name { get; set; }

        public string slug { get; set; }

        public int playlist_id { get; set; }

        public string url { get; set; }

        public string bundle_name { get; set; }

        public string asset_name { get; set; }

        public int created_at { get; set; }
    }
}