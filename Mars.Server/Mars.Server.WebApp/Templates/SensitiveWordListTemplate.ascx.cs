﻿using Mars.Server.BLL.Comments;
using Mars.Server.Entity.Comments;
using Mars.Server.Controls;
using Mars.Server.Entity;
using Mars.Server.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mars.Server.WebApp.Templates
{
    public partial class SensitiveWordListTemplate : TemplateBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override System.Data.DataTable QueryDataPerPage()
        {
            return GetQueryData(false);
        }

        protected override System.Data.DataTable QueryDataAllPages()
        {
            return GetQueryData(true);
        }

        SensitiveWordSearchEntity entity = new SensitiveWordSearchEntity();
        BCtrl_SensitiveWordSearch bll = new BCtrl_SensitiveWordSearch();
        private DataTable GetQueryData(bool isDownload)
        {
            int totalcnt = 0;

            entity.PageSize = base.PageSize;
            entity.PageIndex = base.PageIndex;
            entity.UseDBPagination = !isDownload;

            DataTable table = bll.QueryData(entity, out totalcnt);
            base.TotalRecords = totalcnt;

            return table;
        }

        #region 重写

        /// <summary>
        /// 重写
        /// </summary>
        protected override void RenderJsonData()
        {
            if (ValidQueryCondition())
            {
                this.Visible = false;
                Response.Clear();
                DataTable dt = QueryDataPerPage();

                var data = new
                {
                    cnt = TotalRecords,
                    list = dt
                };
                Response.Write(StringUti.ToUnicode(JsonObj<object>.ToJsonString(data)));
            }
            else
            {
                var data = new
                {
                    cnt = 0,
                    list = new DataTable()
                };
                Response.Write(StringUti.ToUnicode(JsonObj<object>.ToJsonString(data)));
            }
        }

        #endregion
    }
}