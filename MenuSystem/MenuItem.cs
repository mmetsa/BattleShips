using System;

namespace MenuSystem
{
    public class MenuItem
    {
        public MenuItem(string label, Action methodToExecute)
        {
            Label = label;
            MethodToExecute = methodToExecute;
        }

        public MenuItem(string label, bool isDisabled, Action methodToExecute)
        {
            Label = label.Trim();
            MethodToExecute = methodToExecute;
            IsDisabled = isDisabled;
        }
        public virtual string Label { get; set; }
        public virtual Action MethodToExecute { get; set; }

        public virtual bool IsDisabled { get; set; }

        public override string ToString()
        {
            return $"{Label}";
        }
    }
}