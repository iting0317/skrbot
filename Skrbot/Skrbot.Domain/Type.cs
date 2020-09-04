using System;

namespace Skrbot.Domain
{
    public class Type
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime? CreateDate { get; set; }

        public string CreateUser { get; set; }
    }
}