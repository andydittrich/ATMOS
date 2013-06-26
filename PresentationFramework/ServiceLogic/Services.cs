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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using ServiceLogicCommon;

namespace ServiceLogic
{
    [System.ComponentModel.DataObject]
    public class Services
    {
        private readonly string serviceExtensionsPath = "..\\ServiceExtensions";

        // a composition container for our MEF parts
        private CompositionContainer _container;
        [ImportMany]
        IEnumerable<Lazy<IService, IServiceData>> services;

        public Services()
        {
            // An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();

            // Add parts from the current assembly, and from the assemblies found in the plugin directory.
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Services).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(serviceExtensionsPath));

            // Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            // Fill the imports of this object
            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                // TODO: handle this exception
                // Console.WriteLine(compositionException.ToString());
            }
        }

        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, true)]
        public IEnumerable<string> getServices()
        {
            return from service in services 
                   select service.Metadata.Type;
        }

        public IService getService(string type)
        {
            return (from service in services
                    where service.Metadata.Type == type 
                    select service.Value).FirstOrDefault();
        }

        public string getServiceHelp(string type)
        {
            return (from service in services
                    where service.Metadata.Type == type
                    select service.Metadata.HelpText).FirstOrDefault();
        }
    }
}
