using System;
using Delivery.DAL.Models;
using MathCore.WPF.ViewModels;

namespace Delivery.WPF.ViewModels
{
    internal class CourierEditorViewModel : ViewModel
    {
        #region CourierId : - Идентификатор Courier

      //  public Guid CourierId { get; }
      //  public Guid? CourierStatusId{ get; }

        #endregion

        #region Name : string - Название книги

        private string _FirstName;
        public string FirstName { get => _FirstName; set => Set(ref _FirstName, value); }
       
        private string _SecondName;
        public string SecondName { get => _SecondName; set => Set(ref _SecondName, value); }

        private string _PhoneNumber;
        public string PhoneNumber { get => _PhoneNumber; set => Set(ref _PhoneNumber, value); }

     //   private CourierStatus _CourierStatus;
      //  public CourierStatus CourierStatus { get => _CourierStatus; set => Set(ref _CourierStatus, value); }
        #endregion



        public CourierEditorViewModel (Courier courier)
        {
           // CourierId = courier.Id;
          //  CourierStatusId = courier.CourierStatusId;
            _FirstName = courier.FirstName;
            _SecondName = courier.SecondName;
            _PhoneNumber = courier.PhoneNumber;
            //_CourierStatus = courier.CourierStatus;
        }
    }
}
