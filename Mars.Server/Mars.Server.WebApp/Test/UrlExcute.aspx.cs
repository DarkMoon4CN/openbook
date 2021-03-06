﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mars.Server.BLL;
namespace Mars.Server.WebApp.Test
{
    public partial class UrlExcute : System.Web.UI.Page
    {
        UploadImageHelper uploadBll = new UploadImageHelper();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
                {
                    content.InnerHtml = "<b>这是一个重测试，您传的ID:</b> + <span style='color:red; font-size:15px;'>" + Request.QueryString["tid"] + "</span>";
                }
                //else
                //{
                //    Response.StatusCode = 404;
                //    Response.End();
                //}
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string key = DateTime.Now.ToString("yyyyMMdd") + "/" + Guid.NewGuid().ToString() + ".jpg";
                string savePath = Server.MapPath("~/UploadImages") + "/";

                String filename = FileUpload1.FileName;
                savePath += filename;
                FileUpload1.SaveAs(savePath);

                bool result = uploadBll.UpLoadImage(key, savePath);

                if (result)
                {
                    string imgUrl = "<a href='http://7xkwie.com2.z0.glb.qiniucdn.com/" + key + "'>打开图片</a>";
                    Response.Write("<b>图片上传成功<b><br/>" + imgUrl);
                }
                else
                {
                    Response.Write("<b>图片上传失败<b><br/>");
                }
            }
            else
            {
                Response.Write("<b>图片不存在<b><br/>");
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (txtKey.Text.Trim() != "")
            {
                if (uploadBll.DeleteImage(txtKey.Text.Trim()))
                {
                    Response.Write("<b>图片删除成功<b><br/>");
                }
                else
                {
                    Response.Write("<b>图片删除失败<b><br/>");
                }
            }
        }

        public void Button3_Click(object sender, EventArgs e)
        {
            //string key = txtWaterKey.Text;
            //string url = uploadBll.GetImageWater(key);
            //imgWater.ImageUrl = url;
        }

        public void Button4_Click(object sender, EventArgs e)
        {
            //string key = txtMogrify.Text;
            //string url = uploadBll.GetImageMogrify(key);
            //imgMogrify.ImageUrl = url;
        }

        MailManager bllMail = new MailManager();
        protected void Button5_Click(object sender, EventArgs e)
        {
            bllMail.Start();
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            bllMail.Stop();
        }
    }
}