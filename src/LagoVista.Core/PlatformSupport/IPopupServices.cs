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

        Task<String> ShowOpenFileAsync(string fileMask = "");

        Task<String> ShowSaveFileAsync(string fileMask = "", string defaultFileName = "");

        Task<String> PromptForStringAsync(String label, String defaultvalue = "");
        Task<int?> PromptForIntAsync(String label, int defaultvalue = 0);
        Task<decimal?> PromptForDecimalAsync(String label, decimal defaultvalue = 0);
    }
}
