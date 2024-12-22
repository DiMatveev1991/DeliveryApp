using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery.WPF.ViewModels
{
	internal class ViewModelLocator
	{
		public MainWindowViewModel MainWindowModel => App.Services
			.GetRequiredService<MainWindowViewModel>();
	}
}
