using System.ComponentModel.DataAnnotations;

namespace abcsxl.Models.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "请输入当前密码")]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "请输入新密码")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度至少6位")]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword { get; set; }
    }
}
