using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationAPI.DTOs
{
    public class UserAccountDto
    {
        public string Login { get; set; }
        public ObjectId ProfileId { get; set; }
    }
}
