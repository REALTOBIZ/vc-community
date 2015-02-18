﻿#region
using System.Collections.Generic;

#endregion

namespace VirtoCommerce.ApiClient.DataContracts.Contents
{
    #region
    
    #endregion

    public class DynamicContentItemGroup
    {
        #region Constructors and Destructors
        public DynamicContentItemGroup(string groupName)
        {
            this.GroupName = groupName;
            this.Items = new List<DynamicContentItem>();
        }
        #endregion

        #region Public Properties
        public string GroupName { get; set; }

        public List<DynamicContentItem> Items { get; private set; }
        #endregion
    }
}