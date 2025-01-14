using System.Collections;
using Delivery.DTOs;
using Delivery.WPF.ValidationRules;
using MathCore.ViewModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows;
using Microsoft.Xaml.Behaviors.Core;
using Delivery.WPF.Commands;
using Delivery.WPF.Services.Services.Interfaces;

namespace Delivery.WPF.ViewModels
{
	public class OrderLineEditorViewModel : ViewModel, INotifyDataErrorInfo
    {
        private readonly IUserDialogOrderLine _userDialog;

        public bool WasChanged { get; set; }

        private OrderLineDto _orderLine;
        public CheckedActionCommand SubmitCommand { get; set; }

        Dictionary<string, List<string>> Errors = new Dictionary<string, List<string>>();

        public IEnumerable GetErrors(string? propertyName)
        {
            return Errors.ContainsKey(propertyName) ? Errors[propertyName] : Enumerable.Empty<string>();
        }

        public bool HasErrors => Errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public OrderLineDto OrderLine
		{
			get => _orderLine;
			set
            {
                if (!Set(ref _orderLine, value)) return;
            }
		}

        public void Validate(string propertyName, object propertyValue)
        {
            var results = new List<ValidationResult>();

            Validator.TryValidateProperty(propertyValue, new ValidationContext(this) { MemberName = propertyName }, results);

            if (propertyName is "Length" or "Width" or "Weight")
            {
                var doubleValidator = new DoubleValidationRules();
                var doubleRes = doubleValidator.Validate(propertyValue, CultureInfo.CurrentCulture);

                if (!doubleRes.IsValid)
                {
                    Errors.Add(propertyName, new List<string>() { $"Значение {propertyName} < 0" });
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                }
                else
                {
                    Errors.Remove(propertyName);
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                }

                SubmitCommand.RaiseCanExecuteChanged();
                return;
            }

            if (results.Any())
            {
                Errors.Add(propertyName, results.Select(r => r.ErrorMessage).ToList());
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            else
            {
                Errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            SubmitCommand.RaiseCanExecuteChanged();

        }

        private string _itemName;

        [Required(ErrorMessage = "Item Name is Required")]
        public string ItemName
        {
            get => _itemName;
            set
            {
                _itemName = value;
				OrderLine.ItemName = value;
                Validate(nameof(ItemName), value);
            }
        }

        private double? _length;

        [Required]
        public double? Length
        {
            get => _length;
            set
            {
                _length = value;
                OrderLine.Length = (double)value;
                Validate(nameof(Length), value);
            }
        }

        private double? _width;

        [Required]
        public double? Width
        {
            get => _width;
            set
            {
                _width = value;
                OrderLine.Length = (double)value;
                Validate(nameof(Width), value);
            }
        }

        private double? _weight;

        [Required]
        public double? Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                OrderLine.Length = (double)value;
                Validate(nameof(Weight), value);
            }
        }

		public OrderLineEditorViewModel(OrderLineDto orderLine, IUserDialogOrderLine userDialog)
		{
			OrderLine = orderLine;
            _userDialog = userDialog;
            _orderLine = orderLine;
            SubmitCommand = new CheckedActionCommand(Submit, CanSubmit);
        }

        private bool CanSubmit(object obj)
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null) && !Errors.Any();

        }

        private void Submit(object obj)
        {
            WasChanged = true;
            _userDialog.Close();
        }
    }
}