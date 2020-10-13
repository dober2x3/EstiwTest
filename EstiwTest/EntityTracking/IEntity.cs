using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lx.Data.Repository
{
    public interface IEntity
    {
        event EventHandler HasChangesChanged;
        bool HasChanges { get; }
        void AcceptChanges();
        void RejectChanges();
        
        bool Validate();
        bool IsPropertyValid(string propertyName);
        ICollection<ValidationResult> GetValidationResults(string propertyName);
    }
}
