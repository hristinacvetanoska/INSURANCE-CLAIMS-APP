namespace Claims.Domain.Models
{
    using Claims.Domain.Enums;
    using MongoDB.Bson.Serialization.Attributes;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents an insurance claim associated with a specific cover.
    /// </summary>
    public class Claim
    {
        /// <summary>
        /// Gets or sets the unique identifier of the claim.
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the related insurance cover.
        /// </summary>
        [BsonElement("coverId")]
        public string CoverId { get; set; }

        /// <summary>
        /// Gets or sets the date when the claim was created.
        /// Only the date portion is stored.
        /// </summary>
        [BsonElement("created")]
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the name of the claimant.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }


        /// <summary>
        /// Gets or sets the type of the claim.
        /// </summary>
        [BsonElement("claimType")]
        public ClaimType Type { get; set; }

        /// <summary>
        /// Gets or sets the estimated damage cost for the claim.
        /// The value must be between 0 and 100,000.
        /// </summary>
        [BsonElement("damageCost")]
        [Range(0, 100000, ErrorMessage = "DamageCost cannot exceed 100,000.")]
        public decimal DamageCost { get; set; }
    }
}
