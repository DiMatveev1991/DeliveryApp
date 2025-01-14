using Delivery.DTOs;
using MathCore.WPF.ViewModels;

namespace Delivery.WPF.ViewModels
{
    internal class OrderEditorCancelViewModel : ViewModel
    {
        private OrderDto _order;
        public OrderDto Order { get => _order; set => Set(ref _order, value); }

        public OrderEditorCancelViewModel(OrderDto order)
        {
            _order = order;
        }
    }
}
