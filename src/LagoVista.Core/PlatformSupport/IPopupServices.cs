using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface IPopupServices
    {
        Task<bool> ConfirmAsync(String title, String prompt);

        Task ShowAsync(String title, String message);
        Task ShowAsync(String message);
    }
}
