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
        public byte[] Data{get;set;}
        public string Hash{get;set;}
        public string Owner{get;set;}
        public DateTime When{get;set;}
        public bool Verified{get;set;}

    }
}