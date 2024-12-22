using System;
using Delivery.DAL.Models;
using MathCore.WPF.ViewModels;

namespace Delivery.WPF.ViewModels
{
    internal class CourierEditorViewModel : ViewModel
    {


        #region данные курьера

        private string _firstName;
        public string FirstName { get => _firstName; set => Set(ref _firstName, value); }
       
        private string _secondName;
        public string SecondName { get => _secondName; set => Set(ref _secondName, value); }

        private string _phoneNumber;
        public string PhoneNumber { get => _phoneNumber; set => Set(ref _phoneNumber, value); }
        #endregion



        public CourierEditorViewModel (Courier courier)
        {
            _firstName = courier.FirstName;
            _secondName = courier.SecondName;
            _phoneNumber = courier.PhoneNumber;
        }
    }
}
