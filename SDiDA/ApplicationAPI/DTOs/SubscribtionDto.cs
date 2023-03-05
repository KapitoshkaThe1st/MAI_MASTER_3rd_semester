using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationAPI.DTOs
{
    public class SubscriptionDto
    {
        [Required(ErrorMessage = "SubscriptionTargetId is a required field.")]
        public string SubscriptionTargetId { get; set; }
    }
}
