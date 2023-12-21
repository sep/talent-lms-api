using System.Threading.Tasks;
using Refit;

namespace TalentLMS.Api
{
    public partial interface ITalentApi
    {
        [Get("/siteinfo")]
        Task<SiteInfo.DomainDetails> DomainDetails();

        [Get("/ratelimit")]
        Task <SiteInfo.RateLimit> RateLimit();
    }

    namespace SiteInfo
    {
        public record DomainDetails(
            int TotalUsers,
            int TotalCourses,
            int TotalCategories,
            int TotalGroup,
            int TotalBranches,
            int MonthlyActiveUsrs,
            string SignupMethod,
            string PaypalEmail,
            string DomainMap,
            string DateFormat);

        public record RateLimit(
            int Limit,
            int Remaining,
            long Reset,
            string FormattedReset);
    }
}