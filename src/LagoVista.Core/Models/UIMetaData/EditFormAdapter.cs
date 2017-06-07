using LagoVista.Core.Interfaces;
using LagoVista.Core.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LagoVista.Core.Models.UIMetaData
{
    public class EditFormAdapter
    {
        Action<string, bool> _visibilityMethod;
        Func<bool> _validationMethod;
        Action _refreshMethod;
        IViewModelNavigation _navigationService;
        IDictionary<string, FormField> _view;
        Dictionary<string, Type> _editorTypes;
        Dictionary<string, IEnumerable<IEntityHeaderEntity>> _entityLists;

        object _parent;

        public event EventHandler<OptionSelectedEventArgs> OptionSelected;

        public EditFormAdapter(object parent, IDictionary<string, FormField> view, IViewModelNavigation navigationService)
        {
            _entityLists = new Dictionary<string, IEnumerable<IEntityHeaderEntity>>();
            _editorTypes = new Dictionary<string, Type>();
            _view = view;
            _parent = parent;
            _navigationService = navigationService;
            FormItems = new ObservableCollection<FormField>();
            ChildLists = new Dictionary<string, IEnumerable<IEntityHeaderEntity>>();
        }

        public void InvokeOptionSelected(OptionSelectedEventArgs optionSelectedEventArgs)
        {
            OptionSelected?.Invoke(this, optionSelectedEventArgs);
        }

        public void InvokeAdd(string type)
        {
            _navigationService.NavigateAsync(new ViewModelLaunchArgs()
            {
                Parent = _parent,
                ViewModelType = _editorTypes[type],
                LaunchType = LaunchTypes.Create
            });
        }

        public void InvokeItemSelected(ItemSelectedEventArgs selectedItem)
        {
            _navigationService.NavigateAsync(new ViewModelLaunchArgs()
            {
                Parent = _parent,
                Child = _entityLists[selectedItem.Type].Where(itm => itm.ToEntityHeader().Id == selectedItem.Id).First(),
                ViewModelType = _editorTypes[selectedItem.Type],
                LaunchType = LaunchTypes.Edit
            });
        }


        ObservableCollection<FormField> _formItems;
        public ObservableCollection<FormField> FormItems
        {
            get { return _formItems; }
            set { _formItems = value; }
        }

        public void SetValidationMethod(Func<bool> validationMethod)
        {
            _validationMethod = validationMethod;
        }

        public void SetRefreshMethod(Action refreshMethod)
        {
            _refreshMethod = refreshMethod;
        }

        public void SetVisibilityMethod(Action<string, bool> visibilityMethod)
        {
            _visibilityMethod = visibilityMethod;
        }

        public bool Validate()
        {
            if (_validationMethod == null)
            {
                throw new InvalidOperationException("Must call SetValidationMethod prior to calling validate with a method that will iterate through all the form items and perform validation");
            }
            return _validationMethod.Invoke();
        }

        public void Refresh()
        {
            if (_refreshMethod == null)
            {
                throw new InvalidOperationException("Must call SetRefreshMethod prior to calling Refresh with a method that will iterate through all the form items and peform a refresh from data.");
            }

            _refreshMethod.Invoke();
        }

        public void ShowView(String name)
        {
            _visibilityMethod(name, true);
        }

        public void HideView(string name)
        {
            _visibilityMethod(name, false);
        }

        public void AddViewCell(string name)
        {
            FormItems.Add(_view[name.ToFieldKey()]);
        }

        public Dictionary<string, IEnumerable<IEntityHeaderEntity>> ChildLists
        {
            get; set;
        }

        public void AddChildList<TEditorType>(string name, IEnumerable<IEntityHeaderEntity> items) where TEditorType : ViewModelBase
        {
            AddViewCell(name);

            var propertyName = $"{name.Substring(0, 1).ToLower()}{name.Substring(1)}";

            ChildLists.Add(propertyName, items);
            _entityLists.Add(propertyName, items);
            _editorTypes.Add(propertyName, typeof(TEditorType));
        }
    }


    public class OptionSelectedEventArgs : EventArgs
    {
        public String Key { get; set; }
        public String Value { get; set; }
    }

    public class ItemSelectedEventArgs : EventArgs
    {
        public String Type { get; set; }
        public String Id { get; set; }
    }

}
