using Prism.Windows.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SerializationWorkaround
{
    [TemplatePart(Name = "rootGrid", Type = typeof(Grid))]
    public sealed class PrismValidationWrapper : ContentControl
    {
        private Grid RootGrid;

        #region Public Fields

        public static readonly DependencyProperty AttributeNameProperty =
            DependencyProperty.Register("AttributeName", typeof(string), typeof(PrismValidationWrapper), new PropertyMetadata(""));

        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register("Errors", typeof(ReadOnlyCollection<string>), typeof(PrismValidationWrapper), new PropertyMetadata(null));

        #endregion Public Fields

        #region Public Constructors

        protected override void OnApplyTemplate()
        {
            RootGrid = GetTemplateChild("rootGrid") as Grid;
            base.OnApplyTemplate();
        }

        public PrismValidationWrapper()
        {
            DefaultStyleKey = typeof(PrismValidationWrapper);
            IsTabStop = false;
            DataContextChanged += ValidationWrapper_DataContextChanged;
        }

        #endregion Public Constructors

        #region Public Properties

        public string AttributeName
        {
            get { return (string)GetValue(AttributeNameProperty); }
            set { SetValue(AttributeNameProperty, value); }
        }

        public ReadOnlyCollection<string> Errors
        {
            get { return (ReadOnlyCollection<string>)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }

        #endregion Public Properties
        #region 

        private void UpdateErrors(object sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            var errors = sender as BindableValidator;
            
            if (errors.Errors.ContainsKey(AttributeName))
            {
                Errors = errors.Errors[AttributeName];
                VisualStateManager.GoToState(this, "Error", true);
            }
            else
            {
                Errors = null;
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        private void ValidationWrapper_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var model = args.NewValue as SerializableValidatableBindableBase;
            if (model == null)
            {
                throw new ArgumentException("Datacontext mus be of type SerializableValidatableBindableBase");
            }

            model.ErrorsChanged += UpdateErrors;

        }

        #endregion Private Methods
    }
}