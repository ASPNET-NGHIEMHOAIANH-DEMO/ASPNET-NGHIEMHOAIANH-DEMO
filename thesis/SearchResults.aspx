<%@ Page Title="Kết quả tìm kiếm" Language="C#" MasterPageFile="~/gdien.Master" AutoEventWireup="true" CodeBehind="SearchResults.aspx.cs" Inherits="web_audio_music.SearchResults" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="max-width:1000px;margin:120px auto;color:#fff;padding:18px">
        <h2>Kết quả tìm kiếm</h2>
        <asp:Label ID="lblQuery" runat="server" />
        <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false" CssClass="table" GridLines="None">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="#" />
                <asp:BoundField DataField="Title" HeaderText="Bài hát" />
                <asp:BoundField DataField="Artist" HeaderText="Nghệ sĩ" />
                <asp:BoundField DataField="Album" HeaderText="Album" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
