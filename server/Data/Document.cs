using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Documate.ValueObjects;

namespace Documate.Data {
    public class Document {
        [Required]
        [Column (TypeName = "character(64)")]
        private string hash;
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [NotMapped]
        public Hash Hash {
            get { return Hash.Of (hash); }
            set {
                hash = value.ToHexWithoutPrefix ();
            }
        }

        [Column (TypeName = "character(40)")]
        [Required]
        public string Owner { get; set; }

        [Required]
        public DateTime When { get; set; }

        [Required]
        [MaxLength (256)]
        public string Name { get; set; }

        [Column (TypeName = "character(40)")]
        [Required]
        public bool Verified { get; set; }

    }
}