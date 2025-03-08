using LagoVista.Core.Interfaces;
using LagoVista.Core.ViewModels;
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

        Task<String> PromptForStringAsync(String label, String defaultvalue = null, string help = "", bool isRequired = false);
        Task<int?> PromptForIntAsync(String label, int? defaultvalue = null, string help = "", bool isRequired = false);
        Task<double?> PromptForDoubleAsync(String label, double? defaultvalue = null, string help = "", bool isRequired = false);

        Task<bool> ShowModalAsync<TModalWindow, TViewModel>(TViewModel viewModal) where TModalWindow : IModalWindow where TViewModel : IViewModel;
    }
}
