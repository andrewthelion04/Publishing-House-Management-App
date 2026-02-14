using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace EdituraWeb
{
    public partial class DeleteOrders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // protectie: doar admin
            if (Session["rol"] == null || Session["rol"].ToString() != "admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Salut, admin!";
                LoadOrders();
                ResetUI();
            }
        }

        //logout
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        //refresh
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
            ResetUI();

            lblMsg.CssClass = "text-success mt-3 d-block";
            lblMsg.Text = "Lista a fost reîncărcată.";
        }

        private void ResetUI()
        {
            btnDelete.Enabled = false;
            lblSelected.Text = "Nicio comandă selectată.";
            lblMsg.Text = "";
            lblMsg.CssClass = "mt-3 d-block";
        }

        //incarcare comenzi
        private void LoadOrders()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            string sql = @"
SELECT
    c.id_comanda,
    (cl.nume + ' ' + cl.prenume) AS client,
    c.data_comanda,
    c.total
FROM Comenzi c
JOIN Clienti cl ON c.id_client = cl.id_client
ORDER BY c.data_comanda DESC, c.id_comanda DESC";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvOrders.DataSource = dt;
                gvOrders.DataBind();

                lblCount.Text = "Comenzi: " + dt.Rows.Count;
            }
        }

        // handler pentru evenimentul SelectedIndexChanged
        protected void gvOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idComanda = Convert.ToInt32(gvOrders.SelectedDataKey.Value);
            btnDelete.Enabled = true;

            string client = gvOrders.SelectedRow.Cells[1].Text;

            lblSelected.Text = $"Selectată comanda ID <b>{idComanda}</b> (client: <b>{client}</b>)";
            lblMsg.CssClass = "text-primary mt-3 d-block";
            lblMsg.Text = "Delete = anulare comandă (se face și restocare automat).";
        }

        //stergere comanda

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (gvOrders.SelectedDataKey == null)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Selectează mai întâi o comandă.";
                return;
            }

            int idComanda = Convert.ToInt32(gvOrders.SelectedDataKey.Value);
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    using (SqlTransaction tr = con.BeginTransaction())
                    {
                        // 0) verificam ca exista comanda
                        using (SqlCommand chk = new SqlCommand(
                            "SELECT COUNT(*) FROM Comenzi WHERE id_comanda=@id", con, tr))
                        {
                            chk.Parameters.AddWithValue("@id", idComanda);
                            int exists = Convert.ToInt32(chk.ExecuteScalar());
                            if (exists == 0)
                            {
                                tr.Rollback();
                                lblMsg.CssClass = "text-danger mt-3 d-block";
                                lblMsg.Text = "Comanda nu mai există.";
                                LoadOrders();
                                ResetUI();
                                return;
                            }
                        }

                        // 1) luam detaliile comenzii (id_carte, cantitate)
                        DataTable dtDetails = new DataTable();
                        using (SqlCommand getDetails = new SqlCommand(@"
SELECT id_carte, cantitate
FROM DetaliiComanda
WHERE id_comanda=@id", con, tr))
                        {
                            getDetails.Parameters.AddWithValue("@id", idComanda);
                            using (SqlDataAdapter da = new SqlDataAdapter(getDetails))
                            {
                                da.Fill(dtDetails);
                            }
                        }

                        // 2) restocare pentru fiecare carte
                        int restockedLines = 0;

                        foreach (DataRow r in dtDetails.Rows)
                        {
                            int idCarte = Convert.ToInt32(r["id_carte"]);
                            int qty = Convert.ToInt32(r["cantitate"]);

                            using (SqlCommand restock = new SqlCommand(@"
UPDATE Carti
SET stoc = stoc + @q
WHERE id_carte = @cid", con, tr))
                            {
                                restock.Parameters.AddWithValue("@q", qty);
                                restock.Parameters.AddWithValue("@cid", idCarte);

                                int affected = restock.ExecuteNonQuery();
                                if (affected == 0)
                                {
                                    // daca o carte din comanda nu mai exista, facem rollback ca sa nu corupem datele
                                    throw new Exception("Restocare eșuată: nu există cartea cu ID " + idCarte + ".");
                                }

                                restockedLines++;
                            }
                        }

                        // 3) stergem detalii comanda
                        using (SqlCommand delDetails = new SqlCommand(
                            "DELETE FROM DetaliiComanda WHERE id_comanda=@id", con, tr))
                        {
                            delDetails.Parameters.AddWithValue("@id", idComanda);
                            delDetails.ExecuteNonQuery();
                        }

                        // 4) stergem comanda
                        using (SqlCommand delOrder = new SqlCommand(
                            "DELETE FROM Comenzi WHERE id_comanda=@id", con, tr))
                        {
                            delOrder.Parameters.AddWithValue("@id", idComanda);
                            delOrder.ExecuteNonQuery();
                        }

                        tr.Commit();

                        LoadOrders();
                        ResetUI();

                        lblMsg.CssClass = "text-success mt-3 d-block";
                        lblMsg.Text = $"Comanda a fost anulată cu succes. Restocare realizată pentru {restockedLines} poziții.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Eroare la anulare/restocare: " + ex.Message;
            }
        }
    }
}
