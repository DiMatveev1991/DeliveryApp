using System.ComponentModel;
using System.Globalization;
using Delivery.DTOs;
using Delivery.WPF.ValidationRules;
using MathCore.ViewModels;

namespace Delivery.WPF.ViewModels
{
    public class OrderLineEditorViewModel : ViewModel
    {
        private OrderLineDto _orderLine;
        private bool _isValid;

        public OrderLineDto OrderLine
        {
            get => _orderLine;
            set
            {
                if (Set(ref _orderLine, value))
                {
                    // Подписка на события изменения свойств
                    if (_orderLine != null)
                    {
                        _orderLine.PropertyChanged += OrderLine_PropertyChanged;
                    }
                    CheckAndUpdateValidity();
                }
            }
        }

        public bool IsValid
        {
            get => _isValid;
            private set => Set(ref _isValid, value);
        }

        public OrderLineEditorViewModel(OrderLineDto orderLine)
        {
            OrderLine = orderLine;
        }

        private void OrderLine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Проверка валидности при изменении любого свойства
            CheckAndUpdateValidity();
        }

        private void CheckAndUpdateValidity()
        {
            var stringRule = new StringValidationRule();
            var doubleRule = new DoubleValidationRules();
            var nameValidationResult = stringRule.Validate(OrderLine.ItemName, CultureInfo.CurrentCulture);
            var weightValidationResult = doubleRule.Validate(OrderLine.Weight.ToString(), CultureInfo.CurrentCulture);
            var lengthValidationResult = doubleRule.Validate(OrderLine.Length.ToString(), CultureInfo.CurrentCulture);
            var widthValidationResult = doubleRule.Validate(OrderLine.Width.ToString(), CultureInfo.CurrentCulture);

            IsValid = nameValidationResult.IsValid &&
                      weightValidationResult.IsValid &&
                      lengthValidationResult.IsValid &&
                      widthValidationResult.IsValid;
        }
    }
}