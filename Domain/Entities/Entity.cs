using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("TABLE_NAME")]
    public class Entity : BaseEntity
    {
        public int ID { get; set; }
        public string? DESCRIPTION { get; set; }
        public DateTime DATE { get; set; }
    }
}
