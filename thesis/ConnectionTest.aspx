<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Connection Test</title>
    <style>body{background:#0b0b0d;color:#fff;font-family:Segoe UI, Roboto, Arial;padding:24px}</style>
</head>
<body>
    <h2>Test kết nối đến MusicDb</h2>
    <div id="result">
        <% 
            try {
                var cs = System.Configuration.ConfigurationManager.ConnectionStrings["MusicDb"]?.ConnectionString;
                if (string.IsNullOrEmpty(cs)) {
                    Response.Write("Connection string 'MusicDb' không được tìm thấy trong Web.config.<br/>Vui lòng thêm connectionString vào Web.config.");
                } else {
                    using (var conn = new System.Data.SqlClient.SqlConnection(cs)) {
                        conn.Open();
                        Response.Write("Kết nối thành công đến database.<br/>Server version: " + conn.ServerVersion);
                        conn.Close();
                    }
                }
            } catch (Exception ex) {
                Response.Write("Lỗi khi kết nối: " + Server.HtmlEncode(ex.Message) + "<br/><pre>" + Server.HtmlEncode(ex.ToString()) + "</pre>");
            }
        %>
    </div>
    <p>Hướng dẫn nhanh:</p>
    <ul>
        <li>Mở Web.config và đảm bảo connectionStrings/MusicDb đúng (Data Source, Initial Catalog, User/Password nếu cần).</li>
        <li>Ví dụ LocalDB: Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MusicDb;Integrated Security=True</li>
        <li>Ví dụ SQLEXPRESS: Data Source=.\\SQLEXPRESS;Initial Catalog=MusicDb;Integrated Security=True</li>
        <li>Nếu dùng SQL Auth: thêm User ID và Password và bật Mixed Mode Authentication.</li>
    </ul>
</body>
</html>
