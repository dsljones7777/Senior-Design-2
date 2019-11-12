namespace RFIDCommandCenter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tag")]
    public partial class Tag
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(255)]
        public byte[] TagNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public int? LastLocation { get; set; }

        public bool? InLocation { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
