<%@ Page Title="Chỉnh sửa thông tin" Language="C#" MasterPageFile="~/gdien.Master" AutoEventWireup="true" CodeBehind="EditUser.aspx.cs" Inherits="web_audio_music.EditUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="max-width:520px;margin:120px auto;padding:28px;background:#0f0f12;border-radius:8px;border:1px solid rgba(255,255,255,.03)">
        <h2>Chỉnh sửa thông tin</h2>
        <asp:Label ID="lblMsg" runat="server" ForeColor="#ff8080"></asp:Label>
        <div style="margin-top:12px">
            <asp:TextBox ID="txtUser" runat="server" CssClass="form-control" Placeholder="Tên đăng nhập"></asp:TextBox>
        </div>
        <div style="margin-top:12px">
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="Email"></asp:TextBox>
        </div>
        <div style="margin-top:12px">
            <asp:TextBox ID="txtPass" runat="server" TextMode="Password" CssClass="form-control" Placeholder="Mật khẩu mới (để trống nếu không đổi)"></asp:TextBox>
        </div>
        <div style="margin-top:18px">
            <asp:Button ID="btnSave" runat="server" Text="Lưu" OnClick="btnSave_Click" CssClass="btn-pill" />
            <a href="Default.aspx" style="margin-left:12px;color:var(--purple-2)">Hủy</a>
        </div>
    </div>
</asp:Content>
