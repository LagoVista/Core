// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 78920ced5e71e0e15b826d07bd902b9b7bb008ab8d9ec0570f593f5e5e24dae7
// IndexVersion: 2
// --- END CODE INDEX META ---
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
