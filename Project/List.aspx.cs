﻿using Project.Models.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Project
{
    public partial class List : MyPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ShowData();
        }

        protected bool GotAdminPrivileges()
        {
            if (Session["user"] != null)
            {
                Person p = manager.GetPerson(Guid.Parse(Session["user"].ToString()));
                return p.Admin;
            }
            return false;
        }

        private void ShowData()
        {
            GwPersons.DataSource = manager.GetPersons();
            GwPersons.DataBind();

            RepeaterPerson.DataSource = manager.GetPersons();
            RepeaterPerson.DataBind();
        }

        protected void GwPersons_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GwPersons.EditIndex = e.NewEditIndex;
            ShowData();
        }

        protected void GwPersons_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GwPersons.EditIndex = -1;
            ShowData();
        }

        protected void GwPersons_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {            
            GridViewRow row = GwPersons.Rows[e.RowIndex];
            Guid id = Guid.Parse(GetControl<Label>(row.Cells[0].Controls).Text);
            string name = GetControl<TextBox>(row.Cells[1].Controls).Text;
            string surname = GetControl<TextBox>(row.Cells[2].Controls).Text;
            List<TextBox> tbEmails = GetControls<TextBox>(row.Cells[3].Controls);
            List<string> emails = new List<string>();
            foreach (var t in tbEmails)
            {
                emails.Add(t.Text);
            }
            string telephone = GetControl<TextBox>(row.Cells[4].Controls).Text;
            bool admin = bool.Parse(GetControl<DropDownList>(row.Cells[5].Controls).SelectedValue);

            Person p = manager.GetPerson(id);
            p.Name = name;
            for (int i = 0; i < emails.Count; i++)
            {
                p.Email[i] = emails[i];
            }
            p.Surname = surname;
            p.Telephone = telephone;
            p.Admin = admin;

            if (!manager.UpdatePerson(p))
                ShowToastr(Page, $"{p.Name} {p.Surname} not updated! ", "Error adding", Toastr.Warning);

            ShowToastr(Page, $"{p.Name} {p.Surname} updated!", "User updated", Toastr.Success);
            GwPersons.EditIndex = -1;
            ShowData();
        }

        private T GetControl<T>(ControlCollection parent)
        {
            foreach (var ctrl in parent)
            {
                if (ctrl is T)
                {
                    return (T)ctrl;
                }
            }
            return default(T);
        }

        private List<T> GetControls<T>(ControlCollection parent)
        {
            List<T> list = new List<T>();
            foreach (var ctrl in parent)
            {
                if (ctrl is T)
                    list.Add((T)ctrl);
            }
            return list;
        }

        protected void GwPersons_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            TemplateField tf = new TemplateField
            {
                HeaderText = "Email"
            };            
        }

        protected void RepeaterPerson_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string id = ((Label)e.Item.FindControl("RpId")).Text;
                Person p = manager.GetPerson(Guid.Parse(id));
                HtmlTableCell td = (HtmlTableCell)e.Item.FindControl("RpEmail");
                foreach (var em in p.Email)
                {
                    LiteralControl br = new LiteralControl("<br />");
                    HyperLink email = new HyperLink
                    {
                        Text = em,
                        NavigateUrl = "mailto:" + em
                    };
                    td.Controls.Add(email);
                    td.Controls.Add(br);
                }
            }
        }

        protected void GwPersons_RowCreated(object sender, GridViewRowEventArgs e)
        {
            int index = 0;
            if (e.Row.RowType == DataControlRowType.DataRow)
                index = e.Row.RowIndex;
            Person p = manager.GetPersons().ToList()[index];

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!GotAdminPrivileges())
                    e.Row.Cells[6].Controls[0].Visible = false;
            }

            foreach (var em in p.Email)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Normal)
                    {
                        e.Row.Cells[3].Controls.Add(
                            new HyperLink
                            {
                                NavigateUrl = "mailto:" + em,
                                Text = em,
                                ID = "LblEmailBox"
                            }
                        );
                        e.Row.Cells[3].Controls.Add(new LiteralControl("<br/>"));
                    }
                    else
                    {
                        e.Row.Cells[3].Controls.Add(new TextBox { Text = em, CssClass = "form-control input-sm" });
                    }
                }
            }
        }
    }
}