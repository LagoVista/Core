using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.ViewModels
{
    public interface IViewModelNavigation
    {
        Task NavigateAsync(ViewModelLaunchArgs args);
        Task NavigateAsync<TViewModel>() where TViewModel : ViewModelBase;
        Task NavigateAndCreateAsync<TViewModel>(params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndCreateAsync<TViewModel>(object parent, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndEditAsync<TViewModel>(object parent, object child, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndEditAsync<TViewModel>(object parent, String id, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndEditAsync<TViewModel>(String id, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndPickAsync<TViewModel>(Action<Object> selectedAction, Action cancelledAction = null, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task SetAsNewRootAsync<TViewModel>() where TViewModel : ViewModelBase;
        bool CanGoBack();
        Task GoBackAsync();
     }
}
