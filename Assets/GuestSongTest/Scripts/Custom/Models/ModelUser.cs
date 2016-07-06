namespace Photon.LoadBalancing.Custom.Models
{
#if !UNITY_5_3_OR_NEWER
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("users")]
#endif
    public class ModelUser
    {
#if !UNITY_5_3_OR_NEWER
        [Key]
        [Column("id")]
#endif
        public int id { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string nickname { get; set; }

        public int created_at { get; set; }
    }
}