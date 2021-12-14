namespace CourseLibrary.API.Helpers
{
    public static class DateTimeOffsetExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset)
        {
            var currentdate = DateTime.UtcNow;
            int age = currentdate.Year - dateTimeOffset.Year;

            if(currentdate < dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }
}
