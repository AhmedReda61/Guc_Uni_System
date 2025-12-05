using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Guc_Uni_System.Pages
{
    public class AnnouncementsModel : PageModel
    {
        public List<NewsItem> Broadcasts { get; set; }

        public void OnGet()
        {
            // MOCK DATA: Simulating a database fetch
            Broadcasts = new List<NewsItem>
            {
                new NewsItem {
                    Title = "System Maintenance Scheduled",
                    Category = "Critical",
                    Date = DateTime.Now,
                    Body = "The HR Portal will undergo mandatory server maintenance this Friday from 02:00 AM to 04:00 AM. Please ensure all attendance logs are finalized before this window."
                },
                new NewsItem {
                    Title = "Winter Semester Registration",
                    Category = "Academic",
                    Date = DateTime.Now.AddDays(-2),
                    Body = "Academic staff are requested to submit their course availability for the upcoming Winter Semester W25. Deadline is next Monday."
                },
                new NewsItem {
                    Title = "New Health Insurance Policy",
                    Category = "HR Update",
                    Date = DateTime.Now.AddDays(-5),
                    Body = "We have updated the medical leave requirements. Electronic submission of medical documents is now mandatory for all sick leave requests exceeding 2 days."
                },
                new NewsItem {
                    Title = "Campus Entry Protocols",
                    Category = "General",
                    Date = DateTime.Now.AddDays(-10),
                    Body = "Please remember to use your ID card at the main gates. Manual entry logging will be discontinued starting next month."
                }
            };
        }

        public class NewsItem
        {
            public string Title { get; set; }
            public string Category { get; set; }
            public DateTime Date { get; set; }
            public string Body { get; set; }
        }
    }
}