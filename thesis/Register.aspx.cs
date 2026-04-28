using System;
using System.Web.UI;

namespace web_audio_music
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            var user = txtUser.Text.Trim();
            var email = txtEmail.Text.Trim();
            var p1 = txtPass.Text;
            var p2 = txtPass2.Text;
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(p1))
            {
                lblMsg.Text = "Vui lòng điền đầy đủ thông tin.";
                return;
            }
            if (p1 != p2)
            {
                lblMsg.Text = "Mật khẩu không khớp.";
                return;
            }
            try
            {
                var id = DbHelper.CreateUser(user, email, p1);
                Session["UserId"] = id;
                Session["Username"] = user;
                Response.Redirect("Default.aspx");
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Lỗi khi tạo tài khoản: " + ex.Message;
            }
        }
    }
}
