using System;
using System.Web.UI;

namespace web_audio_music
{
    public partial class EditUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }
                int id = Convert.ToInt32(Session["UserId"]);
                var row = DbHelper.GetUserById(id);
                if (row != null)
                {
                    txtUser.Text = row["Username"].ToString();
                    txtEmail.Text = row["Email"].ToString();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("Login.aspx"); return; }
            int id = Convert.ToInt32(Session["UserId"]);
            var user = txtUser.Text.Trim();
            var email = txtEmail.Text.Trim();
            var pass = txtPass.Text;
            try
            {
                if (string.IsNullOrEmpty(pass))
                    DbHelper.UpdateUser(id, user, email);
                else
                    DbHelper.UpdateUser(id, user, email, pass);
                lblMsg.Text = "Cập nhật thành công.";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Lỗi khi cập nhật: " + ex.Message;
            }
        }
    }
}
