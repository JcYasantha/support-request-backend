namespace SupportRequest.Service.Helpers
{
    public static class OfficeHoursHelper
    {
        public static Func<DateTime> TimeProvider = () => DateTime.UtcNow;

        public static bool IsOfficeHours()
        {
            var now = TimeProvider();
            return now.Hour >= 8 && now.Hour < 17;
        }

    }
}
