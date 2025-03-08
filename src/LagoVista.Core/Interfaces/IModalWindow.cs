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
