using AI.CryptoAdvisor.Api.Models.Enums;

namespace AI.CryptoAdvisor.Api.Dtos
{
    public class OnboardingDto
    {
        public string InvestorType { get; set; } 
        public string PreferredAssets { get; set; }

        public string[] ContentTypes { get; set; }
    }
}
