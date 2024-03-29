﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.ViewModels
{
    public interface IViewModelNavigation
    {
        Task NavigateAsync(ViewModelLaunchArgs args);
        Task NavigateAsync(ViewModelBase parentViewModel, Type viewModelType, params KeyValuePair<string, object>[] args);
        Task NavigateAsync<TViewModel>(ViewModelBase parentViewModel, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndCreateAsync<TViewModel>(ViewModelBase parentViewModel, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndCreateAsync<TViewModel>(ViewModelBase parentViewModel, object parentModel, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndEditAsync<TViewModel>(ViewModelBase parentViewModel, object parentModel, object child, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndViewAsync<TViewModel>(ViewModelBase parentViewModel, object parentModel, object child, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndEditAsync<TViewModel>(ViewModelBase parentViewModel, object parentModel, String id, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndViewAsync<TViewModel>(ViewModelBase parentViewModel, object parentModel, String id, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndEditAsync<TViewModel>(ViewModelBase parentViewModel, String id, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task NavigateAndPickAsync<TViewModel>(ViewModelBase parentViewModel, Action<Object> selectedAction, Action cancelledAction = null, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;

        Task SetAsNewRootAsync<TViewModel>(params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase;
        Task SetAsNewRootAsync(Type viewModelType, params KeyValuePair<string, object>[] args);

        bool CanGoBack();
        Task GoBackAsync();
        Task GoBackAsync(int dropPageCount);
    }
}
