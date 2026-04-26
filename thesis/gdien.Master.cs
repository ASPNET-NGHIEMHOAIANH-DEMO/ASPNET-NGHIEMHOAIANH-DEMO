using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace web_audio_music
{
    public partial class gdien : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // show/hide guest or user panel
            var isLogged = Session["UserId"] != null;
            if (pnlGuest != null) pnlGuest.Visible = !isLogged;
            if (pnlUser != null) pnlUser.Visible = isLogged;
            if (isLogged && lblUsername != null)
            {
                lblUsername.Text = Session["Username"] as string ?? "Tài khoản";
                // Ensure the manage link points to the correct application root URL
                try
                {
                    if (hlManage != null)
                        hlManage.NavigateUrl = ResolveUrl("~/SongsManage.aspx");
                }
                catch { }
            }
            // Ensure hero background uses resolved URL to project Content/bg.jpg
            try
            {
                if (heroSection != null)
                {
                    var bg = ResolveUrl("~/Content/bg.jpg");
                    heroSection.Style["background-image"] = $"url('{bg}')";
                }
            }
            catch { }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            try
            {
                // Abandon session and remove session cookie for safety
                Session.Abandon();
                var cookie = new System.Web.HttpCookie("ASP.NET_SessionId", "") { Expires = DateTime.Now.AddDays(-1) };
                Response.Cookies.Add(cookie);
            }
            catch { }
            Response.Redirect("Default.aspx");
        }

        protected void btnGlobalSearch_Click(object sender, EventArgs e)
        {
            var q = txtGlobalSearch.Text ?? string.Empty;
            q = q.Trim();
            if (!string.IsNullOrEmpty(q))
            {
                Response.Redirect("SearchResults.aspx?q=" + Server.UrlEncode(q));
            }
        }

        protected void btnSearchBox_Click(object sender, EventArgs e)
        {
            var q = txtSearchBox.Text ?? string.Empty;
            q = q.Trim();
            if (!string.IsNullOrEmpty(q))
            {
                Response.Redirect("SearchResults.aspx?q=" + Server.UrlEncode(q));
            }
        }
    }

   
    public static class DbHelper
    {
        private static string GetConnectionString()
        {
            var cs = System.Configuration.ConfigurationManager.ConnectionStrings["MusicDb"];
            if (cs == null)
                throw new InvalidOperationException("Connection string 'MusicDb' not found in Web.config");
            return cs.ConnectionString;
        }

        public static System.Data.SqlClient.SqlConnection GetConnection()
        {
            var conn = new System.Data.SqlClient.SqlConnection(GetConnectionString());
            return conn;
        }

        public static void EnsureSchema()
        {
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users](
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Username] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(200) NOT NULL,
        [PasswordHash] NVARCHAR(400) NOT NULL,
        [PasswordSalt] NVARCHAR(200) NOT NULL,
        [PasswordIterations] INT NOT NULL DEFAULT(10000),
        [Role] NVARCHAR(50) NOT NULL DEFAULT('user'),
        [CreatedAt] DATETIME NOT NULL DEFAULT(GETDATE())
    )
END";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Songs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Songs](
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] NVARCHAR(300) NOT NULL,
        [Artist] NVARCHAR(200) NULL,
        [Album] NVARCHAR(200) NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT(GETDATE())
    )
END";
                cmd.ExecuteNonQuery();

                // create default admin if not exists (development convenience)
                cmd.CommandText = "SELECT COUNT(1) FROM Users WHERE Username = @adm";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@adm", "admin");
                var exists = Convert.ToInt32(cmd.ExecuteScalar());
                if (exists == 0)
                {
                    // create admin with default password 'admin123'
                    var password = "admin123";
                    var salt = GenerateSalt(16);
                    var iterations = 10000;
                    var hash = HashPasswordPBKDF2(password, salt, iterations);
                    cmd.Parameters.Clear();
                    cmd.CommandText = "INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, PasswordIterations, Role) VALUES (@u,@e,@h,@s,@it,@r)";
                    cmd.Parameters.AddWithValue("@u", "admin");
                    cmd.Parameters.AddWithValue("@e", "admin@example.com");
                    cmd.Parameters.AddWithValue("@h", hash);
                    cmd.Parameters.AddWithValue("@s", Convert.ToBase64String(salt));
                    cmd.Parameters.AddWithValue("@it", iterations);
                    cmd.Parameters.AddWithValue("@r", "admin");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static byte[] GenerateSalt(int size)
        {
            var salt = new byte[size];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static string HashPasswordPBKDF2(string password, byte[] salt, int iterations)
        {
            using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password ?? string.Empty, salt, iterations))
            {
                var key = pbkdf2.GetBytes(32); // 256-bit
                return Convert.ToBase64String(key);
            }
        }

        public static int CreateUser(string username, string email, string password, string role = "user")
        {
            EnsureSchema();
            var salt = GenerateSalt(16);
            var iterations = 10000;
            var hash = HashPasswordPBKDF2(password, salt, iterations);
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, PasswordIterations, Role) VALUES (@u,@e,@h,@s,@it,@r); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@h", hash);
                cmd.Parameters.AddWithValue("@s", Convert.ToBase64String(salt));
                cmd.Parameters.AddWithValue("@it", iterations);
                cmd.Parameters.AddWithValue("@r", role ?? "user");
                conn.Open();
                var id = cmd.ExecuteScalar();
                return Convert.ToInt32(id);
            }
        }

        public static System.Data.DataRow GetUserByUsername(string username)
        {
            EnsureSchema();
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Username=@u";
                cmd.Parameters.AddWithValue("@u", username);
                using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                {
                    var dt = new System.Data.DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0) return null;
                    return dt.Rows[0];
                }
            }
        }

        public static System.Data.DataRow GetUserById(int id)
        {
            EnsureSchema();
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                {
                    var dt = new System.Data.DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0) return null;
                    return dt.Rows[0];
                }
            }
        }

        public static bool ValidateUser(string username, string password, out int userId)
        {
            userId = -1;
            var row = GetUserByUsername(username);
            if (row == null) return false;
            var storedHash = row["PasswordHash"].ToString();
            var storedSalt = row.Table.Columns.Contains("PasswordSalt") ? row["PasswordSalt"].ToString() : string.Empty;
            var iters = row.Table.Columns.Contains("PasswordIterations") ? Convert.ToInt32(row["PasswordIterations"]) : 10000;
            if (string.IsNullOrEmpty(storedSalt)) return false;
            var saltBytes = Convert.FromBase64String(storedSalt);
            var computed = HashPasswordPBKDF2(password, saltBytes, iters);
            if (FixedTimeEquals(storedHash, computed))
            {
                userId = Convert.ToInt32(row["Id"]);
                return true;
            }
            return false;
        }

        public static void UpdateUser(int id, string username, string email, string password = null)
        {
            EnsureSchema();
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                if (string.IsNullOrEmpty(password))
                {
                    cmd.CommandText = "UPDATE Users SET Username=@u, Email=@e WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@id", id);
                }
                else
                {
                    var salt = GenerateSalt(16);
                    var iters = 10000;
                    var hash = HashPasswordPBKDF2(password, salt, iters);
                    cmd.CommandText = "UPDATE Users SET Username=@u, Email=@e, PasswordHash=@p, PasswordSalt=@s, PasswordIterations=@it WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", hash);
                    cmd.Parameters.AddWithValue("@s", Convert.ToBase64String(salt));
                    cmd.Parameters.AddWithValue("@it", iters);
                    cmd.Parameters.AddWithValue("@id", id);
                }
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            var aa = System.Text.Encoding.UTF8.GetBytes(a ?? string.Empty);
            var bb = System.Text.Encoding.UTF8.GetBytes(b ?? string.Empty);
            if (aa.Length != bb.Length) return false;
            int diff = 0;
            for (int i = 0; i < aa.Length; i++) diff |= aa[i] ^ bb[i];
            return diff == 0;
        }

        public static System.Data.DataTable SearchSongs(string q)
        {
            EnsureSchema();
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 100 * FROM Songs WHERE Title LIKE @q OR Artist LIKE @q ORDER BY CreatedAt DESC";
                cmd.Parameters.AddWithValue("@q", "%" + q + "%");
                using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                {
                    var dt = new System.Data.DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static System.Data.DataTable GetAllSongs()
        {
            EnsureSchema();
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Songs ORDER BY CreatedAt DESC";
                using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                {
                    var dt = new System.Data.DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static System.Data.DataRow GetSongById(int id)
        {
            EnsureSchema();
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Songs WHERE Id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                {
                    var dt = new System.Data.DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0) return null;
                    return dt.Rows[0];
                }
            }
        }

        public static int AddSong(string title, string artist, string album)
        {
            EnsureSchema();
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Songs (Title, Artist, Album) VALUES (@t,@a,@al); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@t", title ?? string.Empty);
                cmd.Parameters.AddWithValue("@a", artist ?? string.Empty);
                cmd.Parameters.AddWithValue("@al", album ?? string.Empty);
                conn.Open();
                var id = cmd.ExecuteScalar();
                return Convert.ToInt32(id);
            }
        }

        public static void UpdateSong(int id, string title, string artist, string album)
        {
            EnsureSchema();
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE Songs SET Title=@t, Artist=@a, Album=@al WHERE Id=@id";
                cmd.Parameters.AddWithValue("@t", title ?? string.Empty);
                cmd.Parameters.AddWithValue("@a", artist ?? string.Empty);
                cmd.Parameters.AddWithValue("@al", album ?? string.Empty);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteSong(int id)
        {
            EnsureSchema();
            using (var conn = GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Songs WHERE Id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
