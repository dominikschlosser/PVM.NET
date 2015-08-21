namespace PVM.Core.Data
{
    public class EmptyProcessData : IProcessData<EmptyProcessData>
    {
        public EmptyProcessData Copy()
        {
            return this;
        }
    }
}