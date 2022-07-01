using System;

namespace Sample.Models
{
    public class Measurement
    {
        /// <summary>
        /// Gets or sets the Id property value.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Value property value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Default constructor.    
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="value"></param>
        public Measurement(Guid id, string description, double value)
        {
            Id = id;
            Description = description;
            Value = value;
        }
    }
}
