using Delivery.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Delivery.DTOs
{
    public class OrderLineDto : INotifyPropertyChanged
	{
		private string? _itemName;
		private Guid _id;
		private Guid? _orderId;
		private double _weight;
		private double _length;
		private double _width;
		public OrderLineDto() { }
		public OrderLineDto(OrderLine model)
		{
			Id = model.Id;
            OrderId = model.OrderId;
            _itemName = model.ItemName;
            _weight = model.Weight;
            Length = model.Length;
            Width = model.Width;
		}

		public Guid Id
		{
			get => _id;
			set => _id = value;
		}

		public Guid? OrderId
		{
			get => _orderId;
			set => _orderId = value;
		}

		public string? ItemName
        {
	        get => _itemName;
	        set
	        {
		        if (_itemName != value)
		        {
			        _itemName = value;
			        OnPropertyChanged(nameof(ItemName));
		        }

			}
		}

		public double Weight
		{
			get => _weight;
			set
			{
				if (_weight != value)
				{
					_weight = value;
					OnPropertyChanged(nameof(Weight));
				}

			}
		}

		public double Length
		{
			get => _length;
			set
			{
				if (_length != value)
				{
					_length = value;
					OnPropertyChanged(nameof(Length));
				}
			}
		}

		public double Width
		{
			get => _width;
			set
			{
				if (_width != value)
				{
					_width = value;
					OnPropertyChanged(nameof(Width));
				}
			}
		}

		public OrderLine ToModel()
        {
	        return new OrderLine()
	        {
		        Id = Id,
                OrderId = OrderId,
                ItemName = ItemName,
                Weight = Weight,
                Length = Length,
                Width = Width
	        };
        }


		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
