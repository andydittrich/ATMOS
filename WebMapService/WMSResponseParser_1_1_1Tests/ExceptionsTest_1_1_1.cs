// Copyright 2012 Andrew Dittrich
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.using System;

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WMS.Exceptions_1_1_1;
using System.Xml.Serialization;
using System.IO;

namespace WMSResponseParser_1_1_1Tests
{
    [TestClass]
    public class ExceptionsTest_1_1_1
    {
        [TestMethod]
        public void TestDeserializeExampleException()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ServiceExceptionReport));
            StringReader stringReader = new StringReader(TestResources.example_exception_1_1_1);
            ServiceExceptionReport exception = (ServiceExceptionReport)serializer.Deserialize(stringReader);
            Assert.AreEqual("1.1.1", exception.version);
            Assert.IsNull(exception.ServiceException[0].code);
            Assert.AreEqual("Plain text message about an error.", exception.ServiceException[0].Value.Trim());
            Assert.AreEqual("InvalidUpdateSequence", exception.ServiceException[1].code);
            Assert.AreEqual("Another message, this one with a Service Exception code supplied.", exception.ServiceException[1].Value.Trim());
        }
    }
}
