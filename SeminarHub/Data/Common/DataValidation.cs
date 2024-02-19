namespace SeminarHub.Data.Common
{
    public static class DataValidation
    {
        public const int SeminarTopicMinLength = 3;
        public const int SeminarTopicMaxLength = 100;

        public const int SeminarLecturerMinLength = 5;
        public const int SeminarLecturerMaxLength = 60;

        public const int SeminarDetailsMinLength = 10;
        public const int SeminarDetailsMaxLength = 500;

        public const string DateTimeExactFormat = "dd/MM/yyyy HH:mm";

        public const int SeminarDurationMinTime = 30;
        public const int SeminarDurationMaxTime = 180;

        public const int CategoryTopicMinLength = 3;
        public const int CategoryTopicMaxLength = 50;

        public const string RequiredField = "The field {0} is required";
        public const string RequiredLength = "The field {0} must be between {2} and {1} characters";

        public const string RequiredDuration = "The Duration must be between 30 and 180 min";

    }
}
