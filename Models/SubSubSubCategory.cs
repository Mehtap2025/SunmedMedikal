using System.ComponentModel.DataAnnotations;

namespace SunmedMedikal.Models
{
    public class SubSubSubCategory
    {
        public int Id { get; set; }
        [Required] public string MainCategory { get; set; }
        [Required] public string SubCategory { get; set; }
        [Required] public string SubSubCategory { get; set; }
        [Required] public string Name { get; set; }
    }
}