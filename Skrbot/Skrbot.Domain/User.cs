using System;

namespace Skrbot.Domain
{
    public class User
    {
        public virtual string Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string LineUserId { get; set; }

        public virtual DateTime? CreateDate { get; set; }
    }
}
