using System;

namespace movieGame.Models
{
    public interface IBaseEntity
    {
        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
    }


    public abstract class BaseEntity
    {
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}


        public BaseEntity ()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
