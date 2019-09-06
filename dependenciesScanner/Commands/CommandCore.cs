namespace dependenciesScanner.Commands
{
    public abstract class CommandCore
    {
        public void Execute()
        {
            if (this.CanExecute())
            {
                this.Action();
            }
        }

        protected virtual bool CanExecute()
        {
            return true;
        }

        protected abstract void Action();
    }
}