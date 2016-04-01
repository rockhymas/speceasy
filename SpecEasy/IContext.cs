namespace SpecEasy
{
    public interface IContext
    {
        void Verify(Action addSpecs);
        IContext Or(string description, Action setup);
    }
}