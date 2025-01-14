using System.Windows.Input;

namespace Delivery.WPF.Commands
{
    public class CheckedActionCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canexecute;

        public event EventHandler? CanExecuteChanged;


        public CheckedActionCommand(Action<object> execute, Predicate<object> canexecute)
        {
            this.execute = execute;
            this.canexecute = canexecute;
        }


        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());

        }


        public bool CanExecute(object? parameter)
        {

            return this.canexecute(parameter);
        }

        public void Execute(object? parameter)
        {
            this.execute(parameter);

        }
    }
}
