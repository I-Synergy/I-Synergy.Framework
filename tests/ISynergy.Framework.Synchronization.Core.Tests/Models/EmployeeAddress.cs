﻿using System;

namespace ISynergy.Framework.Synchronization.Core.Tests.Models
{
    public partial class EmployeeAddress
    {
        public int EmployeeId { get; set; }
        public int AddressId { get; set; }
        public string AddressType { get; set; }
        public Guid? Rowguid { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Address Address { get; set; }
        public Employee Employee { get; set; }
    }
}
