<%@ Page Title="Đăng nhập" Language="C#" MasterPageFile="~/gdien.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="web_audio_music.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="max-width:420px;margin:120px auto;padding:28px;background:#0f0f12;border-radius:8px;border:1px solid rgba(255,255,255,.03)">
        <h2>Đăng nhập</h2>
        <asp:Label ID="lblMessage" runat="server" ForeColor="#ff8080"></asp:Label>
        <div style="margin-top:12px">
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" Placeholder="Tên đăng nhập"></asp:TextBox>
        </div>
        <div style="margin-top:12px">
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" Placeholder="Mật khẩu"></asp:TextBox>
        </div>
        <div style="margin-top:18px">
            <asp:Button ID="btnLogin" runat="server" Text="Đăng nhập" OnClick="btnLogin_Click" CssClass="btn-pill" />
            <a href="Register.aspx" style="margin-left:12px;color:var(--purple-2)">Đăng ký</a>
        </div>
    </div>
</asp:Content>
