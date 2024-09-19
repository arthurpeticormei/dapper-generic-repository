namespace Domain.Models
{
    public class Model : BaseModel
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
    }
}
