namespace PVM.Core.Data
{
    public class EmptyProcessData : ICopyable<EmptyProcessData>
    {
        public EmptyProcessData Copy()
        {
            return this;
        }
    }
}