using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace EdituraWeb
{
    public partial class OrderHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // doar clientii au voie aici
            if (Session["rol"] == null || Session["rol"].ToString() != "client")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["id_client"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadOrders();
            }
        }

        private void LoadOrders()
        {
            int idClient = Convert.ToInt32(Session["id_client"]);
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            // JOIN (Comenzi + Clienti) - are sens si bifeaza join
            string sql = @"
SELECT co.id_comanda, co.data_comanda, co.total
FROM Comenzi co
JOIN Clienti cl ON co.id_client = cl.id_client
WHERE cl.id_client = @idClient
ORDER BY co.data_comanda DESC, co.id_comanda DESC";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@idClient", idClient);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvOrders.DataSource = dt;
                    gvOrders.DataBind();

                    lblOrdersInfo.Text = "Total comenzi: " + dt.Rows.Count;

                    // reset detalii
                    gvOrderDetails.DataSource = null;
                    gvOrderDetails.DataBind();
                    lblSelectedOrder.Text = "";
                    lblDetailsTotal.Text = "";
                    lblMsg.Text = "";
                }
            }
        }

        protected void gvOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idComanda = Convert.ToInt32(gvOrders.SelectedDataKey.Value);
            LoadOrderDetails(idComanda);
        }

        private void LoadOrderDetails(int idComanda)
        {
            int idClient = Convert.ToInt32(Session["id_client"]);
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // securitate: verificam ca aceasta comanda apartine clientului logat
                    SqlCommand checkOwner = new SqlCommand(@"
SELECT COUNT(*)
FROM Comenzi
WHERE id_comanda = @idComanda AND id_client = @idClient", con);

                    checkOwner.Parameters.AddWithValue("@idComanda", idComanda);
                    checkOwner.Parameters.AddWithValue("@idClient", idClient);

                    if ((int)checkOwner.ExecuteScalar() == 0)
                    {
                        lblMsg.CssClass = "text-danger";
                        lblMsg.Text = "Nu ai acces la aceasta comanda.";
                        return;
                    }

                    // JOIN (DetaliiComanda + Carti) + parametru variabil @idComanda
                    SqlCommand cmd = new SqlCommand(@"
SELECT 
    c.titlu,
    dc.cantitate,
    dc.pret_unitar,
    (dc.cantitate * dc.pret_unitar) AS subtotal
FROM DetaliiComanda dc
JOIN Carti c ON dc.id_carte = c.id_carte
WHERE dc.id_comanda = @idComanda
ORDER BY c.titlu", con);

                    cmd.Parameters.AddWithValue("@idComanda", idComanda);

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    gvOrderDetails.DataSource = dt;
                    gvOrderDetails.DataBind();

                    lblSelectedOrder.Text = "Comanda selectata: #" + idComanda;

                    // total calculat din detalii (agregare pe dt)
                    decimal total = 0m;
                    foreach (DataRow row in dt.Rows)
                        total += Convert.ToDecimal(row["subtotal"]);

                    lblDetailsTotal.CssClass = "mt-2 d-block fw-semibold";
                    lblDetailsTotal.Text = "Total calculat din detalii: " + total.ToString("0.00") + " lei";

                    lblMsg.CssClass = "text-success";
                    lblMsg.Text = "Detalii incarcate cu succes.";
                }
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "text-danger";
                lblMsg.Text = "Eroare: " + ex.Message;
            }
        }
    }
}
