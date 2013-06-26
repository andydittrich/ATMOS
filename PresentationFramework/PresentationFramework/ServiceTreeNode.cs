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
using System.Web;
using System.Web.UI.WebControls;

namespace PresentationFramework
{
    public class ServiceTreeNode : TreeNode
    {
        public long ServiceID { get; set; }
        public object SerializableData { get; set; }

        public ServiceTreeNode()
            : base()
        {
        }

        public ServiceTreeNode(TreeView owner, bool isRoot)
            : base(owner, isRoot)
        {
        }

        public ServiceTreeNode(string name, long serviceID)
            : base(name)
        {
            ServiceID = serviceID;
        }

        public ServiceTreeNode(string title, string name, long serviceID)
            : base(title, name)
        {
            ServiceID = serviceID;
        }

        protected override object SaveViewState()
        {
            object[] viewState = new object[3];
            viewState[0] = base.SaveViewState();
            viewState[1] = ServiceID;
            viewState[2] = SerializableData;

            return viewState;
        }

        protected override void LoadViewState(object state)
        {
            object[] viewState = state as object[];
            ServiceID = (long) viewState[1];
            SerializableData = viewState[2];
            base.LoadViewState(viewState[0]);
        }
    }
}