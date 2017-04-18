using System;

namespace SampleMvcApp.Models
{
    public class UserInfo
    {
        public string Sub { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Picture { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
