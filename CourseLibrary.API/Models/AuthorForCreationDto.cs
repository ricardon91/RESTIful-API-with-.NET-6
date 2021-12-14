namespace CourseLibrary.API.Models
{
    public class AuthorForCreationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string MainCategory { get; set; }

        //Allow to create an author with many courses in one request
        public ICollection<CourseForCreationDto> Courses { get; set; } = new List<CourseForCreationDto>();
    }
}
