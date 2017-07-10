using System;

namespace CommandPalette.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CreateCommandControl : Attribute
    {
        public Type ControlType { get; internal set; }

        public CreateCommandControl(Type controlType)
        {
            this.ControlType = controlType;
        }
    }
}
