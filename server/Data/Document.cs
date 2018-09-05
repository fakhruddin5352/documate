using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documate.Data
{
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id{get;set;}
        [Column(TypeName="character(64)")]
        public string Hash{get;set;}
        [Column(TypeName="character(40)")]
        public string Owner{get;set;}
        public DateTime When{get;set;}

        [MaxLength(256)]
        public string Name{get;set;}
        [Column(TypeName="character(40)")]
        public bool Verified{get;set;}

    }
}