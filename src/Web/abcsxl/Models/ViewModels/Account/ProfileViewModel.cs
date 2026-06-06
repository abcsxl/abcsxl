using System.ComponentModel.DataAnnotations;

namespace abcsxl.Models.ViewModels.Account
{
    public class ProfileViewModel
    {
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "显示名称")]
        public string DisplayName { get; set; }

        [Display(Name = "电子邮箱")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "手机号码")]
        [Phone]
        public string Phone { get; set; }

        [Display(Name = "个人简介")]
        public string Bio { get; set; }
    }
}
