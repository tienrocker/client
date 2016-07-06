namespace Photon.LoadBalancing.Custom.Models
{
#if !UNITY_5_3_OR_NEWER
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("questions")]
#endif
    public class ModelQuestion
    {
#if !UNITY_5_3_OR_NEWER
        [Key]
        [Column("id")]
#endif
        public int id { get; set; }

        public string question { get; set; }

        public string option1 { get; set; }

        public string option2 { get; set; }

        public string option3 { get; set; }

        public string option4 { get; set; }

        public int answer { get; set; }

        public int song_id { get; set; }

        public int playlist_id { get; set; }

        public int created_at { get; set; }
    }
}