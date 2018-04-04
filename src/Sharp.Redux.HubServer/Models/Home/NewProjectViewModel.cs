using System.ComponentModel.DataAnnotations;

namespace Sharp.Redux.HubServer.Models.Home
{
    public class NewProjectViewModel
    {
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
