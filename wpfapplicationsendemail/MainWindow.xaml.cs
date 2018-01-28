using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpfapplicationsendemail
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {//First change
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker bgw = new BackgroundWorker();

         
            bgw.WorkerReportsProgress = true;

            // background thread
            bgw.DoWork += new DoWorkEventHandler(
            delegate (object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;

               
			    SqlDataReader rdEmail = null;

               
                SqlConnection conn = new SqlConnection(
     "Data Source=rosebloom.arvixe.com;Initial Catalog=HowtoDB;User ID=bbgreendragon;Password=yuelong");

                
                SqlCommand cmd = new SqlCommand(
                    "select * from bbgreendragon.EmailNotification where sent=0", conn);

                try
                {
                    
                    conn.Open();

                   
                    rdEmail = cmd.ExecuteReader();

                   

                    int i = 1;
                    while (rdEmail.Read())
                    {
                        
                        string contact = (string)rdEmail["Email"];
                        int Id = (int)rdEmail["Id"];
                     

                       
                        sendemailnotifier(contact,Id);
                            b.ReportProgress(i * 1);
                            Thread.Sleep(1000);
                        i++;
                    }
                }
                finally
                {
                    
                    if (rdEmail != null)
                    {
                        rdEmail.Close();
                    }

                 
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            });
            // when progress changed -update lblMain content 
            bgw.ProgressChanged += new ProgressChangedEventHandler(
            delegate (object o, ProgressChangedEventArgs args)
            {
                lblMain.Content = string.Format("Processing emails ...{0} ", args.ProgressPercentage);
            });

           //when bk worker completes its task - update lblMain 
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate (object o, RunWorkerCompletedEventArgs args)
            {
                lblMain.Content = "Mass Email Sending Finished!";
            });

            bgw.RunWorkerAsync();
        }

        private void sendemailnotifier(string contact,int emailSentID)
        {
         
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                message.From = new MailAddress(contact);
                message.To.Add(new MailAddress(contact));
                message.Subject = "Mass mail";
                message.Body  = "<U>Test Mass Mail</U><br>with <b>Sample content goes here</b>." + "URL : <a href='http://www.google.com'>Sample link</a>";
                message.IsBodyHtml = true;

                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(contact, "moon1son");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                updatestatus(contact, emailSentID);
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        
    }

        private void updatestatus(string strEmail,int emailSentIDtoupdate)
        {
            try
            {
                string connectionString = "Data Source=rosebloom.arvixe.com;Initial Catalog=HowtoDB;User ID=bbgreendragon;Password=yuelong";

                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(("UPDATE bbgreendragon.EmailNotification SET sent=1" + " WHERE Id=@Id and Email=@NewEmail"), conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", emailSentIDtoupdate);
                        cmd.Parameters.AddWithValue("@NewEmail", strEmail);
                        //  cmd.Parameters.AddWithValue("@Address", "Kerala");

                        int rows = cmd.ExecuteNonQuery();

                        //rows number of record got updated
                    }
                }
            }
            catch (SqlException ex)
            {
                
                Console.WriteLine(String.Format("update email status for ({0}) - Failed ,ID of email is {1}. Reason {2}", strEmail, emailSentIDtoupdate, ex.Message));
            }
        }

        private void sendemailnow(string strEmail,int id)
        {
            
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                message.From = new MailAddress("jcdesign2000@gmail.com");
                message.To.Add(new MailAddress(strEmail));
                message.Subject = "wpf app email sending Test id= " + id.ToString();
                message.Body = "Content";

                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("jcdesign2000@gmail.com", "moon1son");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        
    }
    }
}
