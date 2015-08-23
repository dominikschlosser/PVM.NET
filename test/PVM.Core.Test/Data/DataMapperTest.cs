using System.Collections.Generic;
using NUnit.Framework;
using PVM.Core.Data;

namespace PVM.Core.Test.Data
{
    [TestFixture]
    public class DataMapperTest
    {
        [Test]
        public void MapData_PropertyExists_SetsData()
        {
            var dataMapper = new DefaultDataMapper();
            var testData = new TestData();
            var processDataMap = new Dictionary<string, object>();
            processDataMap["name"] = "value";
            processDataMap["mappedname"] = "mappedValue";

            dataMapper.MapData(testData, processDataMap);

            Assert.That(testData.Name, Is.EqualTo("value"));
            Assert.That(testData.SomeProperty, Is.EqualTo("mappedValue"));
        }

        [Test]
        public void ExtractData_ExtractsData()
        {
            var dataMapper = new DefaultDataMapper();
            var testData = new TestData();
            testData.SomeOutput = "output";
            testData.SomeMappedOutput = "mappedOutput";

            var output = dataMapper.ExtractData(testData);

            Assert.That(output.Count, Is.EqualTo(2));
            Assert.That(output["someoutput"], Is.EqualTo("output"));
            Assert.That(output["mappedoutputname"], Is.EqualTo("mappedOutput"));
        }
    }
}