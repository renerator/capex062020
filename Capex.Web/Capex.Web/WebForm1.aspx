<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Capex.Web.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
     <!--<link href="Content/bootstrap.min.css" rel="stylesheet" type="text/css"/> -->
    <link href="Content/site2.css" rel="stylesheet" type="text/css"/> 
    <script src="Scripts/sweetalert2.all.min.js"></script>
    <script src="Scripts/jquery-3.3.1.min.js"></script>
    <script src="Scripts/Modules/Gestion/GestionCallBack.js"></script>
    <title></title>
</head>
<body >
    <!-- <div class="card" style="background-color:#4a92a3"> -->
    <div class="card" >
        <div class="card-body" >
            <div class="container offset-md-0">
                <form id="form1" enctype="multipart/form-data" runat="server">
                    <%if (string.IsNullOrEmpty(Request.QueryString["type"]) || Request.QueryString["type"] == "1") { %>
                        <input id="File1" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button1" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button1_Click" onclientclick="return previosUpload();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label1" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                    <%} else if (Request.QueryString["type"] == "2") {  %>
                        <input id="File2" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button2" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button2_Click" onclientclick="return previosUploadImport();"   validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label2" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />    
                    <%} else if (Request.QueryString["type"] == "3") {  %>
                        <input id="File3" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button3" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button3_Click" onclientclick="return previosUploadGantt();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label3" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                    <%} else if (Request.QueryString["type"] == "4") {  %>
                        <input id="File4" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button4" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button4_Click" onclientclick="return previosUploadDescripcion();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label4" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                    <%}  else if (Request.QueryString["type"] == "5") {  %>
                        <input id="File5" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button5" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button5_Click" onclientclick="return previosUploadEvaluacionEconomica();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label5" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                    <%}  else if (Request.QueryString["type"] == "6") {  %>
                        <input id="File6" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button6" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button6_Click" onclientclick="return previosUploadEvaluacionRiesgo();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label6" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                    <%}  else if (Request.QueryString["type"] == "21") {  %>
                        <input id="File7" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button7" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button7_Click" onclientclick="return previosUploadCategorizacion();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label7" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                    <%} else if (Request.QueryString["type"] == "23") {  %>
                        <input id="File9" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button9" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button9_Click" onclientclick="return previosIngresoUploadGantt();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label9" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                    <%} else if (Request.QueryString["type"] == "24") {  %>
                        <input id="File10" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button10" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button10_Click" onclientclick="return previosIngresoUploadDescripcion();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label10" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                     <%} else if (Request.QueryString["type"] == "25") {  %>
                        <input id="File11" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button11" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button11_Click" onclientclick="return previosIngresoUploadEvaluacionEconomica();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label11" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                     <%} else if (Request.QueryString["type"] == "26") {  %>
                        <input id="File12" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button12" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button12_Click" onclientclick="return previosIngresoUploadEvaluacionRiesgo();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label12" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                    <%} else if (Request.QueryString["type"] == "27") {  %>
                        <input id="File13" type="file" class="inputfile inputfile-1" runat="server" />
                        <asp:button id="Button13" runat="server" text="Adjuntar" class="btn btn-success btn-sm" style="height:28px;" onclick="Button13_Click" onclientclick="return previosIngresoUploadImport();" validationgroup="A" xmlns:asp="#unknown" />
                        <asp:Label ID ="Label13" font-Bold="True" ForeColor="#FFFFFF" Runat="server" />
                    <%}  else if (Request.QueryString["type"] == "100") {%>
                        <input id="File100" type="file" class="inputfile inputfile-1" runat="server" />&nbsp;&nbsp;
                        <asp:HiddenField ID="hdnfldCategoria" runat="server" />
                        <asp:button id="Button100" runat="server" text="Subir" class="btn btn-success btn-sm" style="height:28px; width:73px" onclick="Button100_Click" onclientclick="return previosUploadDocument();" validationgroup="A" xmlns:asp="#unknown" />
                    <%}  %>
                </form>
            </div>
        </div>
    </div>
  </body>
</html>
