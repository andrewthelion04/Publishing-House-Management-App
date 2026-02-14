using System;
using System.Configuration;
using System.Data.SqlClient;

namespace EdituraWeb
{
    public partial class AdminPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // protectie rol
            if (Session["rol"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string rol = Session["rol"].ToString();
            if (rol != "admin")
            {
                if (rol == "editor") Response.Redirect("EditorPage.aspx");
                else Response.Redirect("ClientPage.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Salut, admin!";
                LoadStats(); // incarcam statisticile la prima afisare
            }
        }

        // metoda care incarca statisticile in labeluri
        private void LoadStats()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // total utilizatori
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Utilizatori", con))
                {
                    int totalUsers = Convert.ToInt32(cmd.ExecuteScalar());
                    lblTotalUsers.Text = totalUsers.ToString();
                }

                // total clienti
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Clienti", con))
                {
                    int totalClients = Convert.ToInt32(cmd.ExecuteScalar());
                    lblTotalClients.Text = totalClients.ToString();
                }

                // total carti
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Carti", con))
                {
                    int totalBooks = Convert.ToInt32(cmd.ExecuteScalar());
                    lblTotalBooks.Text = totalBooks.ToString();
                }

                // total comenzi
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Comenzi", con))
                {
                    int totalOrders = Convert.ToInt32(cmd.ExecuteScalar());
                    lblTotalOrders.Text = totalOrders.ToString();
                }

                // venit total (suma totalului comenzilor)
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(total), 0) FROM Comenzi", con))
                {
                    decimal revenue = Convert.ToDecimal(cmd.ExecuteScalar());
                    // format simplu, il poti ajusta cum vrei
                    lblRevenue.Text = revenue.ToString("0.00") + " RON";
                }
            }
        }

        // logout
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}
