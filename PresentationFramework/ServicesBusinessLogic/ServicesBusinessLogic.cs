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
using System.Linq;
using System.Text;
using ServicesDataAccess;
using ServicesDataAccess.ServicesDataSetTableAdapters;
using ServiceLogic;
using ServiceLogicCommon;

namespace ServicesBusinessLogic
{
    [System.ComponentModel.DataObject]
    public class ServicesBusinessLogic
    {
        // singleton services table adapter used by this class
        private ServicesTableAdapter _servicesAdapter = null;
        protected ServicesTableAdapter Adapter
        {
            get
            {
                if (_servicesAdapter == null)
                    _servicesAdapter = new ServicesTableAdapter();

                return _servicesAdapter;
            }
        }

        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, true)]
        public ServicesDataSet.ServicesDataTable GetServices()
        {
            return Adapter.GetServices();
        }

        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
        public ServicesDataSet.ServicesDataTable GetServiceById(int serviceId)
        {
            return Adapter.GetServiceById(serviceId);
        }

        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
        public ServicesDataSet.ServicesDataTable GetServicesByUrl(string url)
        {
            return Adapter.GetServicesByUrl(url);
        }

        [System.ComponentModel.DataObjectMethodAttribute (System.ComponentModel.DataObjectMethodType.Insert, true)]
        public bool AddService (string title, string url, string type, string version)
        {
            // before adding the service to the database, give the service a chance to correct 
            // information, normalize urls, or add missing fields.  This will also verify that
            // the service is valid.
            Service service = new Service(title, url, type, version);
            var services = new Services();
            // TODO: check for a null service here
            var dataService = services.getService(type);
            Service correctedService = dataService.requestServiceInformation(service);

            // Create a new ProductRow instance
            ServicesDataSet.ServicesDataTable servicesTable = new ServicesDataSet.ServicesDataTable ();
            ServicesDataSet.ServicesRow serviceRow = servicesTable.NewServicesRow ();

            serviceRow.Title = correctedService.Title;
            serviceRow.Url = correctedService.Url;
            serviceRow.Type = correctedService.Type;
            serviceRow.Version = correctedService.Version;

            // Add the new service
            servicesTable.AddServicesRow(serviceRow);
            int rowsAffected = Adapter.Update(servicesTable);

            // Return true if precisely one row was inserted,
            // otherwise false
            return rowsAffected == 1;
        }

        [System.ComponentModel.DataObjectMethodAttribute (System.ComponentModel.DataObjectMethodType.Update, true)]
        public bool UpdateService (int serviceId, string title, string url, string type, string version)
        {
            ServicesDataSet.ServicesDataTable services = Adapter.GetServiceById (serviceId);
            if (services.Count == 0)
                // no matching record found, return false
                return false;

            // TODO: call requestServiceInformation to verify updated parameters, especailly if the Url is changing.
            ServicesDataSet.ServicesRow service = services[0];

            service.Title = title;
            service.Url = url;
            service.Type = type;
            service.Version = version;

            // Update the service
            int rowsAffected = Adapter.Update(service);

            // Return true if precisely one row was updated,
            // otherwise false
            return rowsAffected == 1;
        }

        [System.ComponentModel.DataObjectMethodAttribute (System.ComponentModel.DataObjectMethodType.Delete, true)]
        public bool DeleteService (int serviceId)
        {
            int rowsAffected = Adapter.Delete(serviceId);

            // Return true if precisely one row was deleted,
            // otherwise false
            return rowsAffected == 1;
        }

    }
}
