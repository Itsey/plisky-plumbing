namespace Plisky.Plumbing {
    using Plisky.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;

    /// <summary>
    /// Taken directly from Josh Smiths article on MSDN.
    /// http://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
    /// </summary>
    public class RelayCommand : ICommand {

        #region Fields

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        #endregion Fields

        #region Constructors

        public RelayCommand(Action<object> execute)
            : this(execute, null) {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute) {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion Constructors

        #region ICommand Members

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter) {
            _execute(parameter);
        }

        #endregion ICommand Members
    }

    /// <summary>
    /// Default boolean <> Visibility where true = Visibility.Visible and false = Visibility.Collapsed.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((Visibility)value) == Visibility.Visible;
        }
    }

    public class StringNullOrEmptyToVisibilityConverter : MarkupExtension, IValueConverter {
        private static StringNullOrEmptyToVisibilityConverter instance;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var s = value as string;
            return string.IsNullOrEmpty(s) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (instance == null) {
                instance = new StringNullOrEmptyToVisibilityConverter();
            }
            return instance;
        }
    }

    public class AutoBindVM : BaseVM {
        public static TraceSwitch PliskyBindingSwitch = new TraceSwitch("PliskyBindingSwitch", "Trace out plisky auto binding debug info", "Verbose");

        public AutoBindVM() {
            InitialiseRequired = true;
            InitaliseOnBind = true;
        }

        protected bool InitialiseRequired { get; set; }

        protected bool InitaliseOnBind { get; set; }

        #region autobindsupport

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected Dictionary<Type, List<string>> controlPrefixIdentifiers = new Dictionary<Type, List<string>>();

        private static BooleanToVisibilityConverter boolVisConverter = new BooleanToVisibilityConverter();

        public override void BindMe(DependencyObject view) {
            b.Info.Log(PliskyBindingSwitch, "BindMe called, binding VM to View");

            if (InitialiseRequired && InitaliseOnBind) {
                PerformInitialLoad();
            }

            FrameworkElement fe = (FrameworkElement)view;

            var allProperties = this.GetType().GetProperties();

            foreach (var p in allProperties) {
                string searchName;
                string propName;
                string propNameL;

                propName = p.Name;
                propNameL = propName.ToLower();

                b.Verbose.Log(PliskyBindingSwitch, "Attempting to bind against Property :" + propName);

                if (propNameL.EndsWith("enabled")) {
                    searchName = propName.Substring(0, propName.Length - 7);
                } else {
                    searchName = propName;
                }

                DependencyObject viewElement = TryToFindUIElementForProperty(fe, searchName, p.PropertyType);
                if (viewElement == null) {
                    b.Verbose.Log(PliskyBindingSwitch, "No view element could be located, skipping this property");
                    continue;
                }

                DependencyProperty autoBind = GetDependencyProperty(viewElement.GetType(), p.PropertyType);
                if (autoBind == null) {
                    b.Verbose.Log(PliskyBindingSwitch, "There is no binding set up for this propertyType/viewElement combo", viewElement.GetType().ToString() + ">>" + p.PropertyType.ToString());
                    continue;
                }

                FrameworkElement el = (FrameworkElement)viewElement;
                // It already has a binding skip it.
                if (el.GetBindingExpression(autoBind) != null) {
                    b.Verbose.Log(PliskyBindingSwitch, "There is already a binding in palce for this element, skipping this property");
                    continue;
                }

                var binding = new Binding(propName) {
                    Mode = p.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                    ValidatesOnDataErrors = Attribute.GetCustomAttributes(p, typeof(ValidationAttribute), true).Any()
                };

                // Binding rules such as converters formatting etc.
                if (autoBind == UIElement.VisibilityProperty && (typeof(bool)).IsAssignableFrom(p.PropertyType)) {
                    binding.Converter = boolVisConverter;
                }
                if (typeof(DateTime).IsAssignableFrom(p.PropertyType)) {
                    binding.StringFormat = "{dd/MM/yyyy}";
                }

                BindingOperations.SetBinding(viewElement, autoBind, binding);

                TextBox tb = viewElement as TextBox;
                if (tb != null) {
                    tb.TextChanged += delegate { tb.GetBindingExpression(TextBox.TextProperty).UpdateSource(); };
                }
            }

            fe.DataContext = this;
        }

        protected void LoadTypesIntoControlPrefix() {
            if (!controlPrefixIdentifiers.ContainsKey(typeof(string))) { controlPrefixIdentifiers.Add(typeof(string), new List<string>()); }
            if (!controlPrefixIdentifiers.ContainsKey(typeof(bool))) { controlPrefixIdentifiers.Add(typeof(bool), new List<string>()); }
        }

        /// <summary>
        /// ControlPrefixIdentifiers are a list of prefixes that are applied to the property names to try and identify the correct controls.  Therefore
        /// Name can be prefixed with txt, for a textbox, lbl for a label etc.
        /// </summary>
        protected void LoadControlPrefixIdentifiers() {
            b.Verbose.Log(PliskyBindingSwitch, "Loading default ControlPrefixes");
            Type next = typeof(string);
            controlPrefixIdentifiers[next].Add("txt");
            controlPrefixIdentifiers[next].Add("lbl");

            next = typeof(bool);
            controlPrefixIdentifiers[next].Add("btn");
            controlPrefixIdentifiers[next].Add("chk");
            controlPrefixIdentifiers[next].Add("rdo");
        }

        private void PerformInitialLoad() {
            LoadTypesIntoControlPrefix();
            LoadControlPrefixIdentifiers();
            InitialiseRequired = false;
        }

        private DependencyObject TryToFindUIElementForProperty(FrameworkElement viewToSearch, string propertyName, Type propertyType) {
            // Takes the name of the property (e.g. Search) tries it with different prefixes (e.g. btnSearch, txtSearch) and tries to find matching UI element.
            DependencyObject viewElement = null;

            string prefixToSearchFor = string.Empty;

            viewElement = (DependencyObject)viewToSearch.FindName(propertyName);
            if (viewElement != null) {
                b.Verbose.Log(PliskyBindingSwitch, "Match made with no prefix.");
            } else if (controlPrefixIdentifiers.ContainsKey(propertyType)) {
                foreach (var v in controlPrefixIdentifiers[propertyType]) {
                    string actualSearchName = v + propertyName;
                    b.Verbose.Log(PliskyBindingSwitch, "Looking for a UI element with the name " + actualSearchName);

                    viewElement = (DependencyObject)viewToSearch.FindName(actualSearchName);
                    if (viewElement != null) {
                        b.Verbose.Log(PliskyBindingSwitch, "Match made.");
                        break;
                    }
                }
            }

            return viewElement;
        }

        private DependencyProperty GetDependencyProperty(Type screenControlType, Type vmBindToType) {
            if (screenControlType == typeof(TextBox)) {
                if (vmBindToType == typeof(string)) {
                    return TextBox.TextProperty;
                }
                if (vmBindToType == typeof(bool)) {
                    return TextBox.IsEnabledProperty;
                }
            }

            if (screenControlType == typeof(DatePicker)) {
                if (vmBindToType == typeof(DateTime)) {
                    return DatePicker.SelectedDateProperty;
                }
                if (vmBindToType == typeof(bool)) {
                    return DatePicker.IsEnabledProperty;
                }
            }

            if (screenControlType == typeof(ItemsControl)) {
                return ItemsControl.ItemsSourceProperty;
            }

            if (screenControlType == typeof(Button)) {
                if (vmBindToType == typeof(bool)) {
                    return Button.IsEnabledProperty;
                }
                if (vmBindToType == typeof(string)) {
                    return Button.ContentProperty;
                }
            }

            if (screenControlType == typeof(TextBlock)) {
                if (vmBindToType == typeof(string)) {
                    return TextBlock.TextProperty;
                }
                if (vmBindToType == (typeof(bool))) {
                    return TextBlock.IsEnabledProperty;
                }
            }

            if (screenControlType == typeof(Label)) {
                if (vmBindToType == typeof(string)) {
                    return Label.ContentProperty;
                }
                if (vmBindToType == typeof(bool)) {
                    return TextBlock.IsVisibleProperty;
                }
            }

            if (screenControlType == typeof(Border)) {
                if (vmBindToType == typeof(bool)) {
                    return Border.VisibilityProperty;
                }
            }

            if (screenControlType == typeof(CheckBox)) {
                if (vmBindToType == typeof(bool)) {
                    return CheckBox.IsCheckedProperty;
                }
            }

            b.Info.Log(PliskyBindingSwitch, "No binding setup for this screenControlType of control, unable to autobind");
            return null;
        }

        #endregion autobindsupport
    }

    /// <summary>
    /// Holds a base ViewModel for use within different solutions, Provides INotifyPropertyChanged implementations
    /// </summary>
    public class BaseVM : INotifyPropertyChanged {
        protected Hub hub;
        protected Bilge b;

        public BaseVM() {
            hub = Hub.Current;
        }

        public virtual void BindMe(DependencyObject view) {
            FrameworkElement fe = (FrameworkElement)view;
            fe.DataContext = this;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(params string[] properties) {
            if (this.PropertyChanged == null) {
                return;
            }

            foreach (string s in properties) {
                OnPropertyChanged(s);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName) {
            b.Assert.False(string.IsNullOrEmpty(propertyName), "Missing property name in notification");
            b.Assert.True(TypeDescriptor.GetProperties(this)[propertyName] != null, "Property not found - " + propertyName);
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged Implementation

        public void InjectHub(Hub h) {
            hub = h;
        }
    }
}