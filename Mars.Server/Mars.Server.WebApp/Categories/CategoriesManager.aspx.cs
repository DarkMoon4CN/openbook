﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mars.Server.WebApp.Categories
{
    public partial class CategoriesManager : System.Web.UI.Page
    {
        public int fun = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                fun = Master.fun;
            }
        }
    }
}