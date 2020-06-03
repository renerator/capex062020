using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CapexInfraestructure.Utilities
{
    public class Utils
    {
        /* ------------------------------------------------------------------------------------
         * 
         * PMO360
         * Av. Nueva Tajamar 481 Of 1403 - Vitacura, Santiago
         * http://www.pmo360.cl
         * 
         * -----------------------------------------------------------------------------------
         * 
         * CLIENTE          : AMSA - ANTOFAGASTA MINERALS
         * PRODUCTO         : CAPEX
         * RESPONABILIDAD   : PROVEER METODOS GENERALES REUTILIZABLES
         * TIPO             : LOGICA DE NEGOCIO
         * DESARROLLADO POR : PMO360
         * FECHA            : 2018
         * VERSION          : 0.0.1
         * PROPOSITO        : ADMINISTRAR CONECTORES Y PROVEER FUNCIONALIDADES COMUNES
         * 
         * 
         */
        #region "PROPIEDADES"
        public static string ExceptionResult { get; set; }
        public static string AppModule { get; set; }
        public static string Status { get; set; }
        public static string Messaje { get; set; }
        public static string Response { get; set; }
        #endregion

        #region "METODOS"
        /// <summary>
        /// CONECTOR SINGLETON
        /// </summary>
        private static string CadenaConexion;
        private static SqlConnection ObjConector;
        public static SqlConnection Conectar()
        {
            if (!string.IsNullOrEmpty(CadenaConexion))
            {
                return ObjConector;
            }
            else
            {
                CadenaConexion = System.Configuration.ConfigurationManager.AppSettings.Get("CapexRepository");
                ObjConector = new SqlConnection(CadenaConexion);
                ObjConector.Open();
                return ObjConector;
            }
        }

        public static String ConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["CapexRepository"].ConnectionString;
        }

        /// <summary>
        /// METODO ENVIO CORREO
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="cc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string SendMail(string to, string from, string cc, string subject, string body)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            string msg = string.Empty;
            try
            {
                MailAddress fromAddress = new MailAddress(from);
                message.From = fromAddress;
                message.To.Add(to);
                if (cc != null && cc != string.Empty)
                {
                    message.CC.Add(cc);
                }
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;

                string _USR_ = ConfigurationManager.AppSettings.Get("CAPEX_MAIL_USR");
                string _PSW_ = ConfigurationManager.AppSettings.Get("CAPEX_MAIL_USR");
                string _HOST_ = ConfigurationManager.AppSettings.Get("CAPEX_MAIL_HOST");
                string _PORT_ = ConfigurationManager.AppSettings.Get("CAPEX_MAIL_PORT");
                string _SSL_ = ConfigurationManager.AppSettings.Get("CAPEX_MAIL_SSL");
                bool ssl = false;
                if (_SSL_.Contains("YES"))
                {
                    ssl = true;
                }
                else
                {
                    ssl = false;
                }

                smtpClient.Host = _HOST_;
                smtpClient.Port = Convert.ToInt32(_PORT_);
                smtpClient.EnableSsl = true;// true for GMAIL;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(_USR_, _PSW_);

                smtpClient.Send(message);
                msg = "OK";
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Envio de Correo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
            return msg;
        }
        /// <summary>
        /// METODO LOG ERROR
        /// </summary>
        /// <param name="Mensaje"></param>
        /// <returns></returns>
        public static string LogError(string Mensaje)
        {
            //string path_error = "C:\\HostingSpaces\\appqacl\\wwwroot\\capex\\Files\\Logs\\log_error.txt";
            string path_error = ConfigurationManager.AppSettings.Get("CAPEX_ERROR_PATH");
            Directory.SetCurrentDirectory(Environment.CurrentDirectory);
            FileStream fss = new FileStream(path_error, FileMode.Append, FileAccess.Write, FileShare.Write);
            fss.Close();

            StreamWriter ssw = new StreamWriter(path_error, true, Encoding.ASCII);
            string descripcion_err = Mensaje;
            ssw.Write(descripcion_err);
            ssw.WriteLine("\n\n");
            ssw.WriteLine("------------------------------------------------------------------------------------\n\n");
            string domain = Environment.UserDomainName;
            string user = Environment.UserName;
            ssw.WriteLine("Date   :" + DateTime.Today.ToString() + "\n\n");
            ssw.WriteLine("Domain :" + domain + "\n");
            ssw.WriteLine("User   :" + user + "\n\n");
            ssw.WriteLine("-----------------------------------------------------------------------------------\n\n");
            ssw.Close();
            return null;
        }
        #endregion

    }
}
