using System.ComponentModel.DataAnnotations;
using Nest;


namespace ElasticExample.Models
{
    public class UserVM
    {

        [MaxLength(50)]
        public string? UserName { get; set; }


      
    }
}
