﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using Linq.PropertyTranslator.Core;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    public class Employee : Entity
    {
        private static readonly CompiledExpressionMap<Employee, string> FullNameExpr =
            DefaultTranslationOf<Employee>.Property(e => e.FullName).Is(e => e.FirstName + " " + e.LastName);

        public int EmployeeNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime HireDate { get; set; }

        public long? CompanyId { get; set; }

        public long? CountryId { get; set; }
        
        public long? FunctionId { get; set; }
        
        public long? SubFunctionId { get; set; }

        public Company Company { get; set; }

        public Country Country { get; set; }

        public Function Function { get; set; }

        public SubFunction SubFunction { get; set; }

        public int? Assigned { get; set; }

        [NotMapped]
        public string FullName => FullNameExpr.Evaluate(this);
    }
}
