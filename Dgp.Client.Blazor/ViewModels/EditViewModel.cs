using Dgp.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dgp.Client.Blazor.ViewModels
{
    public class EditViewModel<TEntity> where TEntity : class
    {
        public void MarkForDelete()
        {
            IsDelete = true;
            SaveButtonClass = "btn btn-danger";
        }

        public string Caption { get; set; } = string.Empty;
        public TEntity Entity { get; set; } = Activator.CreateInstance<TEntity>();
        public List<string> Errors { get; set; } = new List<string>();
        public bool HasErrors => Errors.Any();
        public bool IsDelete { get; set; }
        public bool IsNew { get; set; }
        public string SaveButtonClass { get; set; } = "btn btn-success";
        public Func<bool>? Validate { get; set; }

    }
}
