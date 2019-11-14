namespace RFIDCommandCenter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Location")]
    public partial class Location
    {
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string LocationName { get; set; }

        [Required]
        [StringLength(50)]
        public string ReaderSerialIn { get; set; }

        [StringLength(50)]
        public string ReaderSerialOut { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
