using CourseLibrary.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    [CourseTitleMustBeDifferentFromDescriptionAttribute(ErrorMessage = "Title must be different from description")]
    public abstract class CourseForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a title.")]
        [MaxLength(100, ErrorMessage = "The title should not have more than 100 characters.")]
        public string Title { get; set; }

        //virtual class is better here because an implementation in the base class and allow overriding
        [MaxLength(1500, ErrorMessage = "The description should not have more than 1500 characters.")]
        public virtual string Description { get; set; }
    }
}
