using System.ComponentModel.DataAnnotations;

namespace UOKO.SSO.Models
{
    public class UserInfo
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class AppInfo
    {
        public string Url { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

    }
}