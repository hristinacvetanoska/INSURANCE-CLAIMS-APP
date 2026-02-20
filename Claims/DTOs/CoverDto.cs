using Claims.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.DTOs
{
    public class CoverDto
    {
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the start date associated with the entity.
        /// </summary>
        [BsonElement("startDate")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for the entity.
        /// </summary>
        [BsonElement("endDate")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the type of coverage provided by this policy.
        /// </summary>
        [BsonElement("claimType")]
        public CoverType Type { get; set; }

        /// <summary>
        /// Gets or sets the premium amount that must be paid for this cover.
        /// </summary>
        [BsonElement("premium")]
        public decimal Premium { get; set; }
    }
}
