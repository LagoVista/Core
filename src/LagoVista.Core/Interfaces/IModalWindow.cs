// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 81f654dad700e612ea3dd5968cf805396ccf820efd23deddc7b76ed35089088d
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IModalWindow
    {
        IViewModel ViewModel { get; set;  }

        bool? ShowDialog();

        bool ForCreate { get; set; }

        bool? DialogResult { get; }
        object DataContext { get; set; }
    }
}
