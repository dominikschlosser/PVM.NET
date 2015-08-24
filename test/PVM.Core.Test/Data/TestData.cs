using PVM.Core.Data.Attributes;

namespace PVM.Core.Test.Data
{
    public class TestData
    {
        [In]
        public string SomeInput { get; set; }

        [In("mappedInput")]
        public string SomeMappedInput { get; set; }

        [Out]
        public string SomeOutput { get; set; }

        [Out("mappedOutputName")]
        public string SomeMappedOutput { get; set; }
    }
}