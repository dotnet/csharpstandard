interface IControl
{
    void Paint();
}

class Control : IControl
{
    public virtual void Paint() {}
}

class TextBox : Control
{
    public override void Paint() {}
}
