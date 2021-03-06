﻿using log4net;
using Mars.Server.BLL;
using Mars.Server.Entity;
using Mars.Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mars.Server.App.Modules
{
    public class SignBookModule : ModuleBase
    {
        private static ILog marsLog = log4net.LogManager.GetLogger(typeof(SignBookModule));

        public SignBookModule() : base("SignBook")
        {
            #region 判定是否报到客户  2015/11/12
            Get["IsAllow"] = _ =>
            {
                dynamic data = FecthQueryData();
                string customerKey = data.CustomerKey;
                bool flag=BCtrl_SignBook.Instance.IsAllowCustomerKey(customerKey);
                if (!flag)
                {
                    marsLog.InfoFormat("IsAllow 未找到的用户Key："+customerKey);
                }
                return JsonObj<JsonMessageBase>.ToJson(new JsonMessageBase<string>() { Status = 1, Msg = "客户已签到", Value = flag.ToString() });
            };
            #endregion

            #region 返回客户的二维码地址 2015/11/12
            Get["GetQRCode"] = _ =>
            {
                dynamic data = FecthQueryData();
                string customerKey = data.CustomerKey;
                IList<SignBookEntity> entity=BCtrl_SignBook.Instance.SignBook_Get(customerKey);
                if (entity==null ||  entity.Count == 0) 
                {
                    return JsonObj<JsonMessageBase>.ToJson(new JsonMessageBase() { Status = 0, Msg = "数据接收成功,但没有实际改变" });
                }
                string url = System.Configuration.ConfigurationManager.AppSettings["WebRoot"].ToString()+ "/SignBook/Details.aspx?sid=" + entity[0].SignID;
                //string url = "http://www.kjrili.com/SignBook/Details.aspx?sid=" + entity[0].SignID;
                return JsonObj<JsonMessageBase>.ToJson(new JsonMessageBase<string>() { Status = 1, Msg = "数据接收成功", Value = url });
            };
            #endregion 


            #region 发送邮件
            Get["/SendEmail"] = _ =>
            {
                dynamic data = FecthQueryData();
                ExhibitionEmailEntity entity = new ExhibitionEmailEntity();
                string eID = data.Eid;        //展场ID
                string cID = data.UserID;     //用户ID
                string cName = data.NickName; //用户名
                string email = data.Email;    //email

                int exhibitionID = 0;
                int customerID = 0;
                int.TryParse(eID, out exhibitionID);
                int.TryParse(cID, out customerID);
                entity.ExhibitionID = exhibitionID;
                entity.CustomerID = customerID.ToString();
                entity.CustomerName = cName;
                entity.CustomerEmail = email;
                entity.CreateTime = DateTime.Now;
                entity.SendTypeID = 0;
                //展场邮件时间设定
                DateTime? sendTime = DateTime.Parse("2016-01-07 08:20:00");

                if (entity.CustomerName == null || string.IsNullOrEmpty(entity.CustomerName))
                {
                    entity.CustomerName = "开卷日历大客户";
                }

                if (entity.CreateTime < sendTime) // 测试 求反
                {
                    entity.SendTime = sendTime;
                    BCtrl_ExhibitionEmail.Instance.ExhibitionEmail_Insert(entity);
                }
                else
                {
                    entity.SendTime = entity.CreateTime;
                    SendEmailEntity sendEntity = new SendEmailEntity();
                    BCtrl_UserMail bllMail = new BCtrl_UserMail();
                    List<CompanyEmailAccountEntity> mailList = bllMail.QueryCoEmailAccountList();
                    Random rd = new Random();
                    if (mailList == null || mailList.Count == 0)
                    {
                        return JsonObj<JsonMessageBase<bool>>.ToJson(new JsonMessageBase<bool>() { Status = 0, Msg = "邮箱服务器地址库为空！！", Value = false });
                    }
                    //随机找出一个服务器地址
                    int num = rd.Next(0, mailList.Count - 1);
                    #region 发送邮件固定参数
                    CompanyEmailAccountEntity cea = mailList[num];
                    sendEntity.FromEmail = cea.EmailName;
                    sendEntity.FromSendName = "开卷网络";
                    sendEntity.FromPassword = cea.EmailPassword;
                    sendEntity.ToSendName = entity.CustomerName;
                    sendEntity.ToEmail = entity.CustomerEmail;
                    sendEntity.Subject = "2016年北京图书订货会电子书目——由北京开卷收集整理";
                    sendEntity.Body = "基于2016年图书订货会，这是开卷搜集并整理订货会参展商的电子书目。";
                    sendEntity.Body += "<span style='font-family: Arial; line-height: 23.8px; color: rgb(255, 255, 255); background-color: rgb(255, 0, 0);'>【此文件大小超过3G，请务必使用电脑进行下载】</span> <br />";
                    sendEntity.Body += "下载链接: http://pan.baidu.com/s/1bpjF5c ";
                    sendEntity.Body += "密码: ijex   <br />";
                    sendEntity.Body += "开卷日历APP——网络书业人和事。<br />";
                    sendEntity.Body += "<img src='http://7xkwie.com2.z0.glb.qiniucdn.com/20151201/e3834448-e386-45b8-93cc-01a92744e5e8.jpg' alt='扫一扫' ></img>";
                    #endregion
                    bool state = SendEmail.Send(sendEntity);
                    if (state)
                    {
                        //单条发送时，发送后改变发送状态
                        entity.SendTypeID = 1;
                        BCtrl_ExhibitionEmail.Instance.ExhibitionEmail_Insert(entity);
                    }
                }
                return JsonObj<JsonMessageBase<bool>>.ToJson(new JsonMessageBase<bool>() { Status = 1, Msg = "数据传输完成！！", Value = true });
            };
            #endregion
        }
    }
}
