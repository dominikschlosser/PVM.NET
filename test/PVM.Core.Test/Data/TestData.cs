using PVM.Core.Data.Attributes;

namespace PVM.Core.Test.Data
{
    public class TestData
    {
        [In]
        public string Name { get; set; }

        [In("mappedName")]
        public string SomeProperty { get; set; }

        [Out]
        public string SomeOutput { get; set; }

        [Out("mappedOutputName")]
        public string SomeMappedOutput { get; set; }
    }
}