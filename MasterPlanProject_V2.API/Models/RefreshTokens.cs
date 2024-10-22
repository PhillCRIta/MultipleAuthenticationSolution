
namespace MasterPlanProject_V2.API.Models
{
	public class RefreshTokens
	{
		[Key]
		public int Id { get; set; }
		public string UserId { get; set; }
        public string JwtTokenId { get; set; }
        public string Refresh_Token { get; set; }
        public bool IsValid { get; set; }
        public DateTime ExpiresAt { get; set; }
		public DateTime DataInserimento { get; set; }
    }
}
