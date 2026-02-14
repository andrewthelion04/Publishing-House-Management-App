using System;
using System.Configuration;
using System.Data.SqlClient;

namespace EdituraApp
{
    public partial class EditorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // protectie acces
            if (Session["rol"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string rol = Session["rol"].ToString();

            if (rol != "editor")
            {
                if (rol == "admin") Response.Redirect("AdminPage.aspx");
                else Response.Redirect("ClientPage.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = $"Salut, {rol}!";
                LoadStats();
            }
        }

        private void LoadStats()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                lblBooks.Text = ExecuteScalarInt(con, "SELECT COUNT(*) FROM Carti").ToString();
                lblAuthors.Text = ExecuteScalarInt(con, "SELECT COUNT(*) FROM Autori").ToString();
                lblPublishers.Text = ExecuteScalarInt(con, "SELECT COUNT(*) FROM Edituri").ToString();
                lblContracts.Text = ExecuteScalarInt(con, "SELECT COUNT(*) FROM ContractePublicare").ToString();
            }
        }

        private int ExecuteScalarInt(SqlConnection con, string sql)
        {
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}
