﻿using Project.Models.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Project
{
    public partial class Setup  : MyPage
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            SetSelectedValues();
        }

        protected void DdlTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateCookie("theme", DdlTheme.SelectedValue);
            Response.Redirect(Request.Url.AbsolutePath);
        }

        protected void DdlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateCookie("language", DdlLanguage.SelectedValue);
            Response.Redirect(Request.Url.AbsolutePath);
        }

        private void SetSelectedValues()
        {
            if (Request.Cookies["theme"] != null)
                DdlTheme.SelectedValue = Request.Cookies["theme"].Value;
            else
                DdlTheme.SelectedIndex = 1;

            if (Request.Cookies["language"] != null)
                DdlLanguage.SelectedValue = Request.Cookies["language"].Value;
            else
                DdlLanguage.SelectedIndex = 2;

            if (Request.Cookies["repo"] != null)
                DdlRepository.SelectedValue = Request.Cookies["repo"].Value;
            else
                DdlRepository.SelectedIndex = 1;
        }

        protected void DdlRepository_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateCookie("repo", DdlRepository.SelectedValue);
            Session.Abandon();
            Response.Cookies["user"].Expires = DateTime.Now.AddDays(-1);
            Response.Redirect(Request.Url.AbsolutePath);
        }
    }
}