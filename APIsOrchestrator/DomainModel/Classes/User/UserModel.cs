using System;

namespace DomainModel.Classes.User
{
    public class UserModel
    {
        public System.Guid id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string timestamp { set; get; }
        public bool active { get; set; }
    }
}