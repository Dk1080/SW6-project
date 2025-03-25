using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FitnessApi.Models
{
    [Collection("Users")]
    public class User
    {
        public ObjectId Id { get; set; }

        [Required(ErrorMessage ="User has to have a name!")]
        [Display(Name = "Username")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Must have password")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

}
