using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathCore.WPF.ViewModels;

namespace Delivery.ViewModels
{
	internal class MainWindowViewModel: ViewModel
	{
		private string _Title ="Главное окно";
		public string Title { get => _Title; set => Set(ref _Title, value);}
	}
}
