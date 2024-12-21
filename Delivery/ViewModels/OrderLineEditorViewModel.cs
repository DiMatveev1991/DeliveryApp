using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.DAL.Models;
using MathCore.ViewModels;

namespace Delivery.WPF.ViewModels
{
    public class OrderLineEditorViewModel : ViewModel
    {
        private OrderLine _orderLine;
        public OrderLine OrderLine { get => _orderLine; set => Set(ref _orderLine, value); }

        public OrderLineEditorViewModel(OrderLine orderLine)
        {
            _orderLine = orderLine;
        }
    }
}
