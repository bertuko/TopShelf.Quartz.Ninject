using Quartz;

namespace Topshelf.Quartz
{
    public class QuartzCalendarConfig
    {
        public string CalName { get; set; }
        public ICalendar Calendar { get; set; }

        public QuartzCalendarConfig(string calName, ICalendar calendar)
        {
            CalName = calName;
            Calendar = calendar;
        }
    }
}