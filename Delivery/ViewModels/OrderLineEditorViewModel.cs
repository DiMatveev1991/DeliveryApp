using Delivery.DTOs;
using Delivery.WPF.ValidationRules;
using MathCore.ViewModels;

namespace Delivery.WPF.ViewModels
{
    public class OrderLineEditorViewModel : ViewModel
    {
        private OrderLineDto _orderLine;
        private readonly OrderLineValidationRule _rule;
        public OrderLineValidationRule Rule => _rule;

        public OrderLineEditorViewModel Instance => this;

        public bool IsValid { get; set; } = false;
        //public bool IsValid => _rule.IsValid;
        //public bool IsValid => _orderLine.Length <= 0 || _orderLine.Width <= 0 || _orderLine.Weight <= 0 || string.IsNullOrWhiteSpace(_orderLine.ItemName);

        public OrderLineDto OrderLine
        {
	        get => _orderLine; 
	        set => Set(ref _orderLine, value);
        }

        public OrderLineEditorViewModel(OrderLineDto orderLine)
        {
            _orderLine = orderLine;
            _rule = new OrderLineValidationRule();
        }
    }
}
