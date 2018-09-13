using System;
using System.IO;
using Documate.ValueObjects;

namespace Documate.Document
{
    public class DocumentData {
        public DocumentData (string name, Stream data, DateTime createdOn, Hash hash) {
            this.Name = name;
            this.Data = data;
            this.CreatedOn = createdOn;
            this.Hash = hash;

        }
        public string Name { get; }
        public Stream Data { get; }

        public DateTime CreatedOn { get; }

        public Hash Hash { get; }

    }
}