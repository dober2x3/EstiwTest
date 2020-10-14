using EstiwTest.DB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstiwTest
{

    public abstract class ItemBase : IChangeTracking, INotifyPropertyChanged, ITrackItem, IDataErrorInfo
    {
        //========================================================
        //  Constructors
        //========================================================
        #region ViewModelBase()
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ItemBase()
        {
            this.PropertyChanged += new PropertyChangedEventHandler(OnNotifiedOfPropertyChanged);
        }
        #endregion

        //========================================================
        //  Private Methods
        //========================================================
        #region OnNotifiedOfPropertyChanged(object sender, PropertyChangedEventArgs e)
        /// <summary>
        /// Handles the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for this object.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
        private void OnNotifiedOfPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e != null && !String.Equals(e.PropertyName, "IsChanged", StringComparison.Ordinal))
            {
                this.IsChanged = true;
            }
        }
        #endregion

        //========================================================
        //  IChangeTracking Implementation
        //========================================================
        #region IsChanged
        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the object’s content has changed since the last call to <see cref="AcceptChanges()"/>; otherwise, <see langword="false"/>. 
        /// The initial value is <see langword="false"/>.
        /// </value>
        [NotMapped]
        public bool IsChanged
        {
            get
            {
                lock (_notifyingObjectIsChangedSyncRoot)
                {
                    return _notifyingObjectIsChanged;
                }
            }

            protected set
            {
                lock (_notifyingObjectIsChangedSyncRoot)
                {
                    if (!Boolean.Equals(_notifyingObjectIsChanged, value))
                    {
                        _notifyingObjectIsChanged = value;

                        this.OnPropertyChanged("IsChanged");
                    }
                }
            }
        }
        private bool _notifyingObjectIsChanged;
        private readonly object _notifyingObjectIsChangedSyncRoot = new Object();
        #endregion

        #region AcceptChanges()
        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        public void AcceptChanges()
        {
            this.IsChanged = false;
        }
        #endregion

        //========================================================
        //  INotifyPropertyChanged Implementation
        //========================================================
        #region PropertyChanged
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region OnPropertyChanged(PropertyChangedEventArgs e)
        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> that provides data for the event.</param>
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region OnPropertyChanged(string propertyName)
        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the specified <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The <see cref="MemberInfo.Name"/> of the property whose value has changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region OnPropertyChanged(params string[] propertyNames)
        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the specified <paramref name="propertyNames"/>.
        /// </summary>
        /// <param name="propertyNames">An <see cref="Array"/> of <see cref="String"/> objects that contains the names of the properties whose values have changed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyNames"/> is a <see langword="null"/> reference (Nothing in Visual Basic).</exception>
        protected void OnPropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
            {
                throw new ArgumentNullException("propertyNames");
            }

            foreach (var propertyName in propertyNames)
            {
                this.OnPropertyChanged(propertyName);
            }
        }
        #endregion

        private readonly Dictionary<string, List<ValidationRule>> _validationRules = new Dictionary<string, List<ValidationRule>>();


        #region IDataErrorInfo
        public string this[string columnName]
        {
            get
            {
                var result = string.Empty;
               
                if (_validationRules.ContainsKey(columnName))
                    foreach (var errorCheck in _validationRules[columnName])
                    {
                        if (errorCheck.IsValid.Invoke())
                            continue;

                        result = errorCheck.FailErrorMessage;
                        break;
                    }

                var errorStateChanged = string.IsNullOrWhiteSpace(result)
                    ? _invalidProperties.Remove(columnName)
                    : _invalidProperties.Add(columnName);

                if (errorStateChanged && !nameof(IsValid).Equals(columnName))
                    OnPropertyChanged(nameof(IsValid));

                return result;
            }
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }
        #endregion

        private readonly HashSet<string> _invalidProperties = new HashSet<string>();

        public bool IsValid => !_invalidProperties.Any();

        public bool HasError(string propertyName) => _invalidProperties.Contains(propertyName);

        protected void AddValidationRule(string propertyName, Func<bool> isValid, string failErrorMessage)
        {
            if (!_validationRules.ContainsKey(propertyName))
                _validationRules[propertyName] = new List<ValidationRule>();

            _validationRules[propertyName].Add(new ValidationRule(isValid, failErrorMessage));
        }

        //private virtual void Initialize()
        //{
        //}

        
        private class ValidationRule
        {
            public ValidationRule(Func<bool> isValid, string failErrorMessage)
            {
                IsValid = isValid;
                FailErrorMessage = failErrorMessage;
            }

            public Func<bool> IsValid { get; }
            public string FailErrorMessage { get; }
        }

    }
    
}
