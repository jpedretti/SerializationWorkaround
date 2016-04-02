using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Prism.Windows.Validation;

namespace SerializationWorkaround
{
    /// <summary>
    /// The IValidatableBindableBase interface was created to add validation support for model classes that contain validation rules.
    /// The default implementation of IValidatableBindableBase is the ValidatableBindableBase class, which contains the logic to run the validation rules of
    /// the instance of a model class and return the results of this validation as a list of properties' errors.
    /// </summary>
    // Documentation on validating user input is at http://go.microsoft.com/fwlink/?LinkID=288817&clcid=0x409
    [DataContract]
    public class SerializableValidatableBindableBase : SerializableBindableBase, IValidatableBindableBase
    {
        #region Private Fields

        private BindableValidator _bindableValidator;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableValidatableBindableBase"/> class.
        /// </summary>
        public SerializableValidatableBindableBase()
        {
            //_bindableValidator = new BindableValidator(this);
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// Occurs when the Errors collection changed because new errors were added or old errors were fixed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { Errors.ErrorsChanged += value; }

            remove { Errors.ErrorsChanged -= value; }
        }

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Returns the BindableValidator instance that has an indexer property.
        /// </summary>
        /// <value>
        /// The Bindable Validator Indexer property.
        /// </value>
        public BindableValidator Errors
        {
            get
            {
                if(_bindableValidator == null)
                {
                    _bindableValidator = new BindableValidator(this);
                }

                return _bindableValidator;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is validation enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if validation is enabled for this instance; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidationEnabled
        {
            get { return Errors.IsValidationEnabled; }
            set { Errors.IsValidationEnabled = value; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets all errors.
        /// </summary>
        /// <returns> A ReadOnlyDictionary that's key is a property name and the value is a ReadOnlyCollection of the error strings.</returns>
        public ReadOnlyDictionary<string, ReadOnlyCollection<string>> GetAllErrors()
        {
            return Errors.GetAllErrors();
        }

        /// <summary>
        /// Sets the error collection of this instance.
        /// </summary>
        /// <param name="entityErrors">The entity errors.</param>
        public void SetAllErrors(IDictionary<string, ReadOnlyCollection<string>> entityErrors)
        {
            Errors.SetAllErrors(entityErrors);
        }

        /// <summary>
        /// Validates the properties of the current instance.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if all properties pass the validation rules; otherwise, false.
        /// </returns>
        public bool ValidateProperties()
        {
            return Errors.ValidateProperties();
        }

        /// <summary>
        /// Validates a single property with the given name of the current instance.
        /// </summary>
        /// <param name="propertyName">The property to be validated.</param>
        /// <returns>Returns <c>true</c> if the property passes the validation rules; otherwise, false.</returns>
        public bool ValidateProperty(string propertyName)
        {
            return !Errors.IsValidationEnabled // don't fail if validation is disabled
                || Errors.ValidateProperty(propertyName);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary. We are overriding this property to ensure that the SetProperty and the ValidateProperty methods are
        /// fired in a deterministic way.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>
        /// True if the value was changed, false if the existing value matched the
        /// desired value.
        /// </returns>
        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);

            if (result && !string.IsNullOrEmpty(propertyName))
            {
                if (Errors.IsValidationEnabled)
                {
                    Errors.ValidateProperty(propertyName);
                }
            }
            return result;
        }

        #endregion Protected Methods

    }
}