using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Playland.Database.Model
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı giriniz")]
        public string Username { get; set; }

        [Required(ErrorMessage = "şifre  giriniz")]
        public string Password { get; set; }
    }
}
