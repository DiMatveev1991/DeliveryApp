using MathCore.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Delivery.WPF.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrderLineEditorWindow.xaml
    /// </summary>
    public partial class OrderLineEditorWindow : Window
    {
        public OrderLineEditorWindow()
        {
            InitializeComponent();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextCompositionEventArgs e)
        {
            if (e.Text[0].IsDigit() || e.Text[0] == '.' || e.Text[0] == '-')
                e.Handled = false;
            base.OnPreviewTextInput(e);
            //Regex regex = new Regex(@"^-?[.][a-zA-Z][а-яА-Я]+$");
            //if (regex.IsMatch(e.Text))
            //    e.Handled = true;
            //base.OnPreviewTextInput(e);
        }
    }
}

