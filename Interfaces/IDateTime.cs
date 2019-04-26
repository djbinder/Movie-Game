using System;
using movieGame.Models;

namespace movieGame.Interfaces
{
    public interface IDateTime
    {
        // System.DateTime
        // DateTime IDateTime.Now
        DateTime Now { get; }
    }

    public interface ISessionUser
    {
        Models.SessionUser SessionUser { get; set; }
    }
}
