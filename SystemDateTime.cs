using System;
using movieGame.Interfaces;
using movieGame.Models;

namespace movieGame
{
    // movieGame.SystemDateTime
    // movieGame.Interfaces.IDateTime
    public class SystemDateTime : IDateTime
    {
        // System.DateTime
        // DateTime SystemDateTime.Now
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }

    public class CurrentUser : ISessionUser
    {
        public Models.SessionUser SessionUser
        {
            get { return SessionUser; }
            set { SessionUser.SessionUserId = 0; SessionUser.SessionUserName = "BLANK USER"; }
        }
    }
}