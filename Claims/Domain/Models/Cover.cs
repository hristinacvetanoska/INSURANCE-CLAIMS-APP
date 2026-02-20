namespace Claims.Domain.Models
{
    using Claims.Domain.Enums;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Represents an insurance cover policy that defines coverage details
    /// such as validity period, type of coverage, and premium amount.
    /// </summary>
    public class Cover
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Cover.
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the start date associated with the entity.
        /// </summary>
        [BsonElement("startDate")]
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for the entity.
        /// </summary>
        [BsonElement("endDate")]
        [BsonDateTimeOptions(DateOnly = true)]
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