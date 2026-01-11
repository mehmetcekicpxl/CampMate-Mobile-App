namespace CampMate.Shared.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        public int CampSiteId { get; set; }

        public int UserId { get; set; }
    }
}
