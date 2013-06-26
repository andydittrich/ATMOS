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
using WMS.Exceptions_1_3_0;
using System.Xml.Serialization;
using System.IO;

namespace WMSResponseParser_1_3_0Tests
{
    [TestClass]
    public class ExceptionsTest_1_3_0
    {
        [TestMethod]
        public void TestDeserializeExampleException()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ServiceExceptionReport));
            StringReader stringReader = new StringReader(TestResources.example_exception_1_3_0);
            ServiceExceptionReport exception_1_3_0 = (ServiceExceptionReport)serializer.Deserialize(stringReader);
            Assert.AreEqual("1.3.0", exception_1_3_0.version);
            Assert.IsNull(exception_1_3_0.ServiceException[0].code);
            Assert.AreEqual("Plain text message about an error.", exception_1_3_0.ServiceException[0].Value.Trim ());
            Assert.AreEqual("InvalidUpdateSequence", exception_1_3_0.ServiceException[1].code);
            Assert.AreEqual("Another error message, this one with a service exception code supplied.", exception_1_3_0.ServiceException[1].Value.Trim());
        }
    }
}
