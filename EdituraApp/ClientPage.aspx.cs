using System;
using System.Configuration;
using System.Data.SqlClient;

namespace EdituraApp
{
    public partial class ClientPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // protectie: doar client
            if (Session["rol"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string rol = Session["rol"].ToString();
            if (rol != "client")
            {
                if (rol == "admin") Response.Redirect("AdminPage.aspx");
                else Response.Redirect("EditorPage.aspx");
                return;
            }

            if (Session["id_client"] == null)
            {
                // fara id_client nu putem incarca datele clientului
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Salut, client!";
                LoadClientStats();
            }
        }

        // statistici client
        private void LoadClientStats()
        {
            int idClient = Convert.ToInt32(Session["id_client"]);
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // numar comenzi
                    lblOrdersCount.Text = ExecuteScalarInt(con,
                        "SELECT COUNT(*) FROM Comenzi WHERE id_client=@id",
                        idClient).ToString();

                    // total cheltuit
                    decimal spent = ExecuteScalarDecimal(con,
                        "SELECT ISNULL(SUM(total), 0) FROM Comenzi WHERE id_client=@id",
                        idClient);

                    lblTotalSpent.Text = spent.ToString("0.00") + " lei";

                    // ultima comanda (data) - daca nu exista, afisam "-"
                    object last = ExecuteScalarObject(con,
                        "SELECT MAX(data_comanda) FROM Comenzi WHERE id_client=@id",
                        idClient);

                    if (last == null || last == DBNull.Value)
                        lblLastOrder.Text = "-";
                    else
                        lblLastOrder.Text = Convert.ToDateTime(last).ToString("yyyy-MM-dd");

                    lblMsg.CssClass = "text-success mt-3 d-block";
                    lblMsg.Text = "Datele contului tau au fost incarcate.";
                }
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Eroare la incarcare: " + ex.Message;
            }
        }
        // functii helper pentru statistici
        private int ExecuteScalarInt(SqlConnection con, string sql, int idClient)
        {
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", idClient);
                object val = cmd.ExecuteScalar();
                return Convert.ToInt32(val);
            }
        }

        private decimal ExecuteScalarDecimal(SqlConnection con, string sql, int idClient)
        {
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", idClient);
                object val = cmd.ExecuteScalar();
                return Convert.ToDecimal(val);
            }
        }

        private object ExecuteScalarObject(SqlConnection con, string sql, int idClient)
        {
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", idClient);
                return cmd.ExecuteScalar();
            }
        }

        //logout
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}
