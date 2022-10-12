using Nest;
using System.ComponentModel.DataAnnotations;


namespace DapperExample.Models.Entites
{
    public class User
    {

        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdateAt { get; set; } 

        [MaxLength(50)]
        public string? UserName { get; set; }


        public CompletionField? Suggest => new()
        {
            Input = new List<string>
        {
               
            string.IsNullOrWhiteSpace(UserName)
                ? string.Empty
                : UserName,

        }
        };

    }
}
