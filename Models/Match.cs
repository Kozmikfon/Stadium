using System.ComponentModel.DataAnnotations.Schema;

namespace Stadyum.API.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int Team1Id { get; set; }
        public Team? Team1 { get; set; }

        public int Team2Id { get; set; }
        public Team? Team2 { get; set; }

        public required string FieldName { get; set; }=string .Empty;

        [Column(TypeName = "timestamp without time zone")]
        public DateTime MatchDate { get; set; }
    }

}