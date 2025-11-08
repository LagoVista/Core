// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e54c8f212a49cc189baf43b9ab03371c109401658f60a12582ea3b20c0eee8ad
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.ViewModels
{
    public enum LaunchTypes
    {
        View,
        Create,
        Edit,
        Picker,
        Other
    }

    public class ViewModelLaunchArgs
    {
        public ViewModelLaunchArgs()
        {
            Parameters = new Dictionary<string, object>();
            IsNewRoot = false;            
        }

        public ViewModelBase ParentViewModel { get; set; }

        public ViewModelBase ThisViewModel { get; set; }

        public Object Parent { get; set; }
        public Object Child { get; set; }

        public String ParentId { get; set; }
        public String ChildId { get; set; }




        public bool IsNewRoot { get; set; }

        public Type ViewModelType { get; set; }
        public LaunchTypes LaunchType { get; set; }

        public TModel GetParent<TModel>() where TModel : class
        {
            return Parent as TModel;
        }

        public TModel GetChild<TModel>() where TModel : class
        {
            return Child as TModel;
        }

        public Dictionary<String, object> Parameters { get; private set; }

        public bool HasParam(string key)
        {
            return Parameters.ContainsKey(key);
        }

        public TParam GetParam<TParam>(string key) where TParam : class
        {

            if (!HasParam(key))
            {
                return null;
            }

            return Parameters[key] as TParam;
        }

        public Action<Object> SelectedAction
        {
            get;
            set;
        }

        public Action CancelledAction
        {
            get;
            set;
        }
    }
}
