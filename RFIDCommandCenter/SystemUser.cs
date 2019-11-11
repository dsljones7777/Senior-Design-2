namespace RFIDCommandCenter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SystemUser")]
    public partial class SystemUser
    {
        [Key]
        [StringLength(16)]
        public string Username { get; set; }

        [Required]
        [MaxLength(64)]
        public byte[] Pass { get; set; }

        public int UserRole { get; set; }
    }
}
