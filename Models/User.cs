using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string FIO {  get; set; }

        public string RolesString
        {
            get { return Roles == null || !Roles.Any() ? "Нет ролей" : string.Join(", ", Roles); }
        }
    }
}
