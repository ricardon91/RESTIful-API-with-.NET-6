using CourseLibrary.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    
    public class CourseForCreationDto : CourseForManipulationDto
    {
        
        //We already have the authorId from URI, so we don't need it
        //public Guid AuthorId { get; set; }
    }
}
