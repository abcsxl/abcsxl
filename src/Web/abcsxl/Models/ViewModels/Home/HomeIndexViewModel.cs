using System.ComponentModel;

namespace abcsxl.Models.ViewModels.Home
{
    public class HomeIndexViewModel
    {
        public List<HomePostViewModel>? LastPosts { get; set; }

        public int LastCount { get; set; } = 4;
    }
}
