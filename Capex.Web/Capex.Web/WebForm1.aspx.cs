using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CapexIdentity.Utilities;
using CapexInfraestructure.Bll.Business.Planificacion;
using CapexInfraestructure.Bll.Factory;
using ClosedXML.Excel;
using Dapper;
using Newtonsoft.Json;
using SHARED.AzureStorage;

namespace Capex.Web
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        public class DocumentUpload
        {
            public string ApiVersion { get; set; }
            public Data Data { get; set; }
        }

        public class Data
        {
            public string type { get; set; }
            public string nameFile { get; set; }
            public string initiative { get; set; }
            public string code { get; set; }
            public string error { get; set; }
            public string date { get; set; }
            public string response { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label1.Text = "";
                if (!string.IsNullOrEmpty(type) && File1.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        data.code = "-1";
                        Label1.Text = "ERROR";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Categorizacion\\" + token;
                            string nameFile = Path.GetFileName(File1.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Categorizacion";
                            string parCaso = "Desarrollo Ingenieria";
                            string urlAzure = String.Empty;

                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File1.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File1.PostedFile.FileName);
                            }
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File1))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);

                                Label1.Text = nameFile;
                            }
                            else
                            {
                                data.code = "1";
                                Label1.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            exc.ToString();
                            Label1.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label7.Text = "";
                if (!string.IsNullOrEmpty(type) && File7.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        data.code = "-1";
                        Label7.Text = "ERROR";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Categorizacion\\" + token;
                            string nameFile = Path.GetFileName(File7.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Categorizacion";
                            string parCaso = "Desarrollo Ingenieria";
                            string urlAzure = String.Empty;

                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File7.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File7.PostedFile.FileName);
                            }
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File7))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label7.Text = nameFile;
                            }
                            else
                            {
                                data.code = "1";
                                Label7.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            exc.ToString();
                            Label7.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label2.Text = "";
                if (!string.IsNullOrEmpty(type) && File2.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        data.code = "1";
                        Label2.Text = "ERROR";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Import\\" + token;
                            string nameFile = Path.GetFileName(File2.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Presupuesto";
                            string parCaso = "Template";
                            string urlAzure = String.Empty;

                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File2.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File2.PostedFile.FileName);
                            }
                            data.nameFile = nameFileFinal;
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File2))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label2.Text = nameFile;
                                string path = Server.MapPath("Scripts/Import/" + token);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                var donde = Path.Combine(Server.MapPath("Scripts/Import/" + token), nameFileFinal);
                                File2.PostedFile.SaveAs(donde);
                            }
                            else
                            {
                                data.code = "1";
                                Label2.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            Label2.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button13_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label13.Text = "";
                if (!string.IsNullOrEmpty(type) && File13.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        data.code = "1";
                        Label13.Text = "ERROR";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Import\\" + token;
                            string nameFile = Path.GetFileName(File13.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Presupuesto";
                            string parCaso = "Template";
                            string urlAzure = String.Empty;

                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File13.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File13.PostedFile.FileName);
                            }
                            data.nameFile = nameFileFinal;
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File13))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label13.Text = nameFile;
                                string path = Server.MapPath("Scripts/Import/" + token);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                var donde = Path.Combine(Server.MapPath("Scripts/Import/" + token), nameFileFinal);
                                File13.PostedFile.SaveAs(donde);
                            }
                            else
                            {
                                data.code = "1";
                                Label13.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            Label13.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label3.Text = "";
                if (!string.IsNullOrEmpty(type) && File3.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        data.code = "1";
                        Label3.Text = "ERROR";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Presupuesto\\" + token;
                            string nameFile = Path.GetFileName(File3.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Presupuesto-Gantt";
                            string parCaso = "Carta Gantt";
                            string urlAzure = String.Empty;
                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File3.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File3.PostedFile.FileName);
                            }
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File3))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label3.Text = nameFile;
                            }
                            else
                            {
                                data.code = "1";
                                Label3.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            Label3.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button9_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label9.Text = "";
                if (!string.IsNullOrEmpty(type) && File9.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        data.code = "1";
                        Label9.Text = "ERROR";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Presupuesto\\" + token;
                            string nameFile = Path.GetFileName(File9.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Presupuesto-Gantt";
                            string parCaso = "Carta Gantt";
                            string urlAzure = String.Empty;
                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File9.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File9.PostedFile.FileName);
                            }
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File9))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label9.Text = nameFile;
                            }
                            else
                            {
                                data.code = "1";
                                Label9.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            Label9.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label4.Text = "";
                if (!string.IsNullOrEmpty(type) && File4.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        data.code = "1";
                        Label4.Text = "ERROR";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Descripcion\\" + token;
                            string nameFile = Path.GetFileName(File4.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Descripcion-Detallada";
                            string parCaso = "Descipción Detallada";
                            string urlAzure = String.Empty;
                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File4.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File4.PostedFile.FileName);
                            }
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File4))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label4.Text = nameFile;
                            }
                            else
                            {
                                data.code = "1";
                                Label4.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            Label4.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button10_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label10.Text = "";
                if (!string.IsNullOrEmpty(type) && File10.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        data.code = "1";
                        Label10.Text = "ERROR";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Descripcion\\" + token;
                            string nameFile = Path.GetFileName(File10.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Descripcion-Detallada";
                            string parCaso = "Descipción Detallada";
                            string urlAzure = String.Empty;
                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File10.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File10.PostedFile.FileName);
                            }
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File10))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label10.Text = nameFile;
                            }
                            else
                            {
                                data.code = "1";
                                Label10.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            Label10.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label5.Text = "";
                if (!string.IsNullOrEmpty(type) && File5.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        data.code = "1";
                        Label5.Text = "ERROR";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\EvaluacionEconomica\\" + token;
                            string nameFile = Path.GetFileName(File5.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Evaluacion-Economica";
                            string parCaso = "Evaluacion Económica";
                            string urlAzure = String.Empty;
                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File5.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File5.PostedFile.FileName);
                            }
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File5))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label5.Text = nameFile;
                            }
                            else
                            {
                                data.code = "1";
                                Label5.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            Label5.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button11_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label11.Text = "";
                if (!string.IsNullOrEmpty(type) && File11.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        data.code = "1";
                        Label11.Text = "ERROR";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\EvaluacionEconomica\\" + token;
                            string nameFile = Path.GetFileName(File11.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Evaluacion-Economica";
                            string parCaso = "Evaluacion Económica";
                            string urlAzure = String.Empty;
                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File11.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File11.PostedFile.FileName);
                            }
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File11))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label11.Text = nameFile;
                            }
                            else
                            {
                                data.code = "1";
                                Label11.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            Label11.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label6.Text = "";
                if (!string.IsNullOrEmpty(type) && File6.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        Label6.Text = "ERROR";
                        data.code = "1";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\EvaluacionRiesgo\\" + token;
                            string nameFile = Path.GetFileName(File6.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Evaluacion-Riesgo";
                            string parCaso = "Evaluación Riesgo";
                            string urlAzure = String.Empty;
                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File6.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File6.PostedFile.FileName);
                            }
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File6))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label6.Text = nameFile;
                            }
                            else
                            {
                                data.code = "1";
                                Label6.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            Label6.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button12_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Label12.Text = "";
                if (!string.IsNullOrEmpty(type) && File12.PostedFile.ContentLength > 0)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                    {
                        Label12.Text = "ERROR";
                        data.code = "1";
                    }
                    else
                    {
                        try
                        {
                            string token = HttpContext.Current.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                            data.initiative = token;
                            string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                            string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();

                            //Parámetros:
                            //✓ string shareFile: recurso compartido de azure
                            //✓ string pathdirectory: directorio del archivo
                            //✓ string namefile: Nombre del archivo
                            //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                            string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                            string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\EvaluacionRiesgo\\" + token;
                            string nameFile = Path.GetFileName(File12.PostedFile.FileName);
                            string nameFileFinal = nameFile;
                            string parPaso = "Evaluacion-Riesgo";
                            string parCaso = "Evaluación Riesgo";
                            string urlAzure = String.Empty;
                            string totalVersionesArchivos = ValidarArchivoExiste(token, parUsuario, nameFile, parPaso, parCaso);
                            if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                            {
                                nameFileFinal = Path.GetFileNameWithoutExtension(File12.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File12.PostedFile.FileName);
                            }
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File12))
                            {
                                data.response = RegistrarArchivoFinal(token, parUsuario, nameFile, nameFileFinal, parPaso, parCaso, shareFile, pathDirectory, urlAzure);
                                Label12.Text = nameFile;
                            }
                            else
                            {
                                data.code = "1";
                                Label12.Text = "ERROR";
                            }
                        }
                        catch (Exception exc)
                        {
                            data.code = "1";
                            Label12.Text = exc.Message;
                        }
                    }
                }
                var jsonResponse = JsonConvert.SerializeObject(documentUpload, Formatting.None);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", "laterUpload(" + jsonResponse + ")", true);
            }
        }

        protected void Button100_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["CAPEX_SESS_USERNAME"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                // Save the file.
                string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
                string categoriaTokenSeleccionada = hdnfldCategoria.Value;
                string categoriaSeleccionada = SeleccionarCategoriaDocumento(categoriaTokenSeleccionada);
                DocumentUpload documentUpload = new DocumentUpload();
                Data data = new Data();
                documentUpload.Data = data;
                data.type = type;
                data.code = "0";
                data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                if (!string.IsNullOrEmpty(type) && File100.PostedFile.ContentLength > 0)
                {
                    try
                    {
                        string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                        string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();
                        //Parámetros:
                        //✓ string shareFile: recurso compartido de azure
                        //✓ string pathdirectory: directorio del archivo
                        //✓ string namefile: Nombre del archivo
                        //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                        string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                        string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Biblioteca\\" + categoriaSeleccionada;
                        string nameFile = Path.GetFileName(File100.PostedFile.FileName);
                        string extension = Path.GetExtension(File100.PostedFile.FileName);
                        int sizeFile = File100.PostedFile.ContentLength;
                        string tipo = "Desconocido";
                        if (!string.IsNullOrEmpty(extension))
                        {
                            extension = extension.Replace(".", "");
                            switch (extension)
                            {
                                case "xls": tipo = "Excel"; break;
                                case "xlsx": tipo = "Excel"; break;
                                case "ppt": tipo = "Power Point"; break;
                                case "pptx": tipo = "Power Point"; break;
                                case "pdf": tipo = "Acrobat Reader"; break;
                                case "doc": tipo = "Word"; break;
                                case "docx": tipo = "Word"; break;
                                case "mpp": tipo = "Project"; break;
                                case "mppx": tipo = "Project"; break;
                                case "txt": tipo = "Texto"; break;
                                case "zip": tipo = "Comprimido ZIP"; break;
                                case "rar": tipo = "Comprimido RAR"; break;
                            }
                        }
                        string nameFileFinal = nameFile;
                        string urlAzure = String.Empty;
                        string totalVersionesArchivos = ValidarDocumentoExiste(categoriaTokenSeleccionada, nameFile);
                        if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                        {
                            nameFileFinal = Path.GetFileNameWithoutExtension(File100.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(File100.PostedFile.FileName);
                        }
                        UploadDownload uploadDownload = new UploadDownload();
                        if (UploadDownload.UploadFile(shareFile, pathDirectory, nameFileFinal, File100))
                        {
                            data.response = RegistrarDocumentoFinal(nameFileFinal, sizeFile, extension, tipo, categoriaTokenSeleccionada);
                        }
                        else
                        {
                            data.code = "1";
                            data.response = "Error al subir documento a azure";
                        }
                    }
                    catch (Exception exc)
                    {
                        data.code = "1";
                        data.response = exc.Message;
                    }
                    var functionResponse = "laterUpload(" + JsonConvert.SerializeObject(documentUpload, Formatting.None) + ")";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", functionResponse, true);
                }
            }
        }

        protected void Button30_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["CAPEX_SESS_USERNAME"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                excelTemplate(HiddenField30, HiddenField31, File30);
            }
        }

        protected void Button31_Click(object sender, EventArgs e)
        {
            if (!@User.Identity.IsAuthenticated || HttpContext.Current.Session == null || HttpContext.Current.Session["CAPEX_SESS_USERNAME"] == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUploadLogout", "laterUploadLogout()", true);
            }
            else
            {
                excelTemplate(HiddenField32, HiddenField33, File31);
            }
        }


        private void excelTemplate(System.Web.UI.WebControls.HiddenField hiddenFieldUno, System.Web.UI.WebControls.HiddenField HiddenFieldDos, System.Web.UI.HtmlControls.HtmlInputFile htmlInputFile)
        {
            // Save the file.
            string type = Convert.ToString(HttpContext.Current.Request.QueryString.Get("type"));
            string TipoIniciativaSeleccionada = hiddenFieldUno.Value;
            string ITPEToken = HiddenFieldDos.Value;
            DocumentUpload documentUpload = new DocumentUpload();
            Data data = new Data();
            documentUpload.Data = data;
            data.type = type;
            data.code = "0";
            data.date = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            if (!string.IsNullOrEmpty(type) && htmlInputFile.PostedFile.ContentLength > 0)
            {
                try
                {
                    string parUsuario = HttpContext.Current.Session["CAPEX_SESS_USERNAME"].ToString();
                    string rol = HttpContext.Current.Session["CAPEX_SESS_ROLNOMBRE"].ToString();
                    //Parámetros:
                    //✓ string shareFile: recurso compartido de azure
                    //✓ string pathdirectory: directorio del archivo
                    //✓ string namefile: Nombre del archivo
                    //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile
                    string finalPath = String.Empty;
                    if (!string.IsNullOrEmpty(TipoIniciativaSeleccionada) && !string.IsNullOrEmpty(TipoIniciativaSeleccionada.Trim()))
                    {
                        if ("2".Equals(TipoIniciativaSeleccionada.Trim()))
                        {
                            finalPath = "ImportPresupuesto";
                        }
                        else if ("1".Equals(TipoIniciativaSeleccionada.Trim()))
                        {
                            finalPath = "ImportCasoBase";
                        }
                    }
                    DateTime todaysDate = DateTime.Now.Date;
                    int Periodo = todaysDate.Year;
                    string ShareFile = ConfigurationManager.AppSettings.Get("Shared");
                    string PathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\" + finalPath + "\\" + Periodo.ToString();
                    string NameFile = Path.GetFileName(htmlInputFile.PostedFile.FileName);
                    string Extension = Path.GetExtension(htmlInputFile.PostedFile.FileName);
                    int SizeFile = htmlInputFile.PostedFile.ContentLength;
                    string NameFileFinal = NameFile;

                    string totalVersionesArchivos = ValidarExcelTemplateExiste(Periodo, NameFile);
                    if (!string.IsNullOrEmpty(totalVersionesArchivos) && (string.Compare("0", totalVersionesArchivos) != 0))
                    {
                        NameFileFinal = Path.GetFileNameWithoutExtension(htmlInputFile.PostedFile.FileName) + "(" + (Convert.ToInt32(totalVersionesArchivos) + 1) + ")" + Path.GetExtension(htmlInputFile.PostedFile.FileName);
                    }
                    UploadDownload uploadDownload = new UploadDownload();
                    if (UploadDownload.UploadFile(ShareFile, PathDirectory, NameFileFinal, htmlInputFile))
                    {
                        data.response = RegistrarExcelTemplateFinal(TipoIniciativaSeleccionada, Periodo, SizeFile, Extension, NameFile, NameFileFinal, ShareFile, PathDirectory, ITPEToken);
                    }
                    else
                    {
                        data.code = "1";
                        data.response = "Error al subir documento a azure";
                    }
                }
                catch (Exception exc)
                {
                    data.code = "1";
                    data.response = exc.Message;
                }
                var functionResponse = "laterUpload(" + JsonConvert.SerializeObject(documentUpload, Formatting.None) + ")";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "laterUpload", functionResponse, true);
            }
        }

        public string ProcesarTemplatePresupuesto(string token, string usuario, string archivo, string tipo)
        {
            /*try
            {
                if (tipo == "CB" || tipo == "CD")
                {
                    string json = JsonConvert.SerializeObject(ImportarTemplateCasoBase(token, usuario, archivo), Formatting.None);
                    return Json(
                        json,
                        JsonRequestBehavior.AllowGet
                        );
                }
                else
                {
                    IPlanificacion = FactoryPlanificacion.delega(IT);
                    string json = JsonConvert.SerializeObject(ImportarTemplate(token, usuario, archivo), Formatting.None);
                    return Json(
                        json,
                        JsonRequestBehavior.AllowGet
                        );
                }
            }
            catch (Exception exc)
            {
                return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                FactoryPlanificacion = null;
                IPlanificacion = null;
            }*/
            return null;

        }

        private string RegistrarArchivo(string IniToken, string ParUsuario, string ParNombre, string ParPaso, string ParCaso)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    return SqlMapper.Query(objConnection, "CAPEX_INS_REGISTRAR_ARCHIVO", new { IniToken, ParUsuario, ParNombre, ParPaso, ParCaso }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                }
                catch (Exception err)
                {
                    err.ToString();
                    return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private string RegistrarArchivoFinal(string IniToken, string ParUsuario, string ParNombre, string ParNombreFinal, string ParPaso, string ParCaso, string ShareFile, string PathDirectory, string UrlAzure)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var parametos = new DynamicParameters();
                    parametos.Add("IniToken", IniToken);
                    parametos.Add("ParUsuario", ParUsuario);
                    parametos.Add("ParNombre", ParNombre);
                    parametos.Add("ParNombreFinal", ParNombreFinal);
                    parametos.Add("ParPaso", ParPaso);
                    parametos.Add("ParCaso", ParCaso);
                    parametos.Add("ShareFile", ShareFile);
                    parametos.Add("PathDirectory", PathDirectory);
                    parametos.Add("UrlAzure", UrlAzure);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                    SqlMapper.Query(objConnection, "CAPEX_INS_REGISTRAR_ARCHIVO_FINAL", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    string respuesta = parametos.Get<string>("Respuesta");
                    if (respuesta != null && !string.IsNullOrEmpty(respuesta.Trim()))
                    {
                        return respuesta.Trim();
                    }
                    return null;
                }
                catch (Exception err)
                {
                    err.ToString();
                    return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private string RegistrarExcelTemplateFinal(string Tipo, int Periodo, int Tamano, string Extension, string Nombre, string NombreFinal, string ShareFile, string PathDirectory, string ITPEToken)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    SqlMapper.Query(objConnection, "CAPEX_INS_EXCEL_TEMPLATE", new { ExcelTemplateTipo = Tipo, ExcelPeriodo = Periodo, ExcelTemplateTam = Tamano, ExcelTemplateExt = Extension, ExcelTemplateNombre = Nombre, ExcelTemplateNombreFinal = NombreFinal, ShareFile = ShareFile, PathDirectory = PathDirectory, ITPEToken = ITPEToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return "Registrado";
                }
                catch (Exception err)
                {
                    err.ToString();
                    return "Error";
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private string RegistrarDocumentoFinal(string Documento, int Tamano, string Extension, string Tipo, string Categoria)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    SqlMapper.Query(objConnection, "CAPEX_INS_DOCUMENTACION", new { Documento, Tamano, Extension, Tipo, Categoria }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return "Registrado";
                }
                catch (Exception err)
                {
                    err.ToString();
                    return "Error";
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private string ValidarExcelTemplateExiste(int Periodo, string NameFile)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    return SqlMapper.Query<String>(objConnection, "CAPEX_VAL_EXIST_EXCEL_TEMPLATE", new { Periodo, NameFile }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                }
                catch (Exception err)
                {
                    err.ToString();
                    return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }


        private string ValidarArchivoExiste(string IniToken, string ParUsuario, string ParNombre, string ParPaso, string ParCaso)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    return SqlMapper.Query<String>(objConnection, "CAPEX_VAL_EXIST_FILE", new { IniToken, ParUsuario, ParNombre, ParPaso, ParCaso }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                }
                catch (Exception err)
                {
                    err.ToString();
                    return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private string ValidarDocumentoExiste(string DocCatToken, string DocNombre)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    return SqlMapper.Query<String>(objConnection, "CAPEX_VAL_EXIST_DOC_CAT", new { DocCatToken, DocNombre }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                }
                catch (Exception err)
                {
                    err.ToString();
                    return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private string SeleccionarCategoriaDocumento(string DocCatToken)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    return SqlMapper.Query<String>(objConnection, "CAPEX_SEL_DOCUMENTACION_CATEGORIA_SUMMARY_BY_TOKEN", new { DocCatToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                }
                catch (Exception err)
                {
                    err.ToString();
                    return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

    }

}