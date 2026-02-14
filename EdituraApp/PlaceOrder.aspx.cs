using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace EdituraWeb
{
    public partial class PlaceOrder : System.Web.UI.Page
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
                LoadCarti();
                LoadSelectedCarteInfo();
            }
        }

        private void LoadCarti()
        {
            ddlCarti.Items.Clear();

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // afisam doar cartile care au stoc > 0
                SqlCommand cmd = new SqlCommand(
                    "SELECT id_carte, titlu FROM Carti WHERE stoc > 0 ORDER BY titlu", con);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    ddlCarti.Items.Add(new ListItem("Alege o carte...", "0"));

                    while (rdr.Read())
                    {
                        ddlCarti.Items.Add(new ListItem(
                            rdr["titlu"].ToString(),
                            rdr["id_carte"].ToString()
                        ));
                    }
                }
            }
        }

        protected void ddlCarti_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedCarteInfo();
        }

        // informatii despre carti
        private void LoadSelectedCarteInfo()
        {
            txtPret.Text = "";
            txtStoc.Text = "";
            lblMsg.Text = "";
            lblMsg.CssClass = "mt-3 d-block text-center";

            int idCarte;
            if (!int.TryParse(ddlCarti.SelectedValue, out idCarte) || idCarte == 0)
                return;

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT pret, stoc FROM Carti WHERE id_carte=@id", con);
                cmd.Parameters.AddWithValue("@id", idCarte);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        txtPret.Text = Convert.ToDecimal(rdr["pret"]).ToString("0.00");
                        txtStoc.Text = rdr["stoc"].ToString();
                    }
                }
            }
        }

        //plasare comanda

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            int idClient = Convert.ToInt32(Session["id_client"]);

            int idCarte;
            if (!int.TryParse(ddlCarti.SelectedValue, out idCarte) || idCarte == 0)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block text-center";
                lblMsg.Text = "Selecteaza o carte!";
                return;
            }

            int cantitate;
            if (!int.TryParse(txtCantitate.Text.Trim(), out cantitate) || cantitate <= 0)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block text-center";
                lblMsg.Text = "Cantitatea trebuie sa fie un numar pozitiv!";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    using (SqlTransaction tr = con.BeginTransaction())
                    {
                        // 1) luam pret + stoc curent (cu lock ca sa fie safe)
                        SqlCommand cmdInfo = new SqlCommand(@"
SELECT pret, stoc
FROM Carti WITH (UPDLOCK, ROWLOCK)
WHERE id_carte=@id", con, tr);

                        cmdInfo.Parameters.AddWithValue("@id", idCarte);

                        decimal pret;
                        int stoc;

                        using (SqlDataReader rdr = cmdInfo.ExecuteReader())
                        {
                            if (!rdr.Read())
                            {
                                tr.Rollback();
                                lblMsg.CssClass = "text-danger mt-3 d-block text-center";
                                lblMsg.Text = "Cartea nu exista!";
                                return;
                            }

                            pret = Convert.ToDecimal(rdr["pret"]);
                            stoc = Convert.ToInt32(rdr["stoc"]);
                        }

                        if (cantitate > stoc)
                        {
                            tr.Rollback();
                            lblMsg.CssClass = "text-danger mt-3 d-block text-center";
                            lblMsg.Text = $"Stoc insuficient! Disponibil: {stoc}";
                            return;
                        }

                        decimal total = pret * cantitate;

                        // 2) INSERT in Comenzi
                        SqlCommand cmdComanda = new SqlCommand(@"
INSERT INTO Comenzi (id_client, data_comanda, total)
OUTPUT INSERTED.id_comanda
VALUES (@idClient, CAST(GETDATE() AS date), @total)", con, tr);

                        cmdComanda.Parameters.AddWithValue("@idClient", idClient);
                        cmdComanda.Parameters.AddWithValue("@total", total);

                        int idComanda = Convert.ToInt32(cmdComanda.ExecuteScalar());

                        // 3) INSERT in DetaliiComanda
                        SqlCommand cmdDetalii = new SqlCommand(@"
INSERT INTO DetaliiComanda (id_comanda, id_carte, cantitate, pret_unitar)
VALUES (@idComanda, @idCarte, @cant, @pret)", con, tr);

                        cmdDetalii.Parameters.AddWithValue("@idComanda", idComanda);
                        cmdDetalii.Parameters.AddWithValue("@idCarte", idCarte);
                        cmdDetalii.Parameters.AddWithValue("@cant", cantitate);
                        cmdDetalii.Parameters.AddWithValue("@pret", pret);

                        cmdDetalii.ExecuteNonQuery();

                        // 4) UPDATE stoc carte
                        SqlCommand cmdUpdateStoc = new SqlCommand(@"
UPDATE Carti
SET stoc = stoc - @cant
WHERE id_carte=@idCarte", con, tr);

                        cmdUpdateStoc.Parameters.AddWithValue("@cant", cantitate);
                        cmdUpdateStoc.Parameters.AddWithValue("@idCarte", idCarte);

                        cmdUpdateStoc.ExecuteNonQuery();

                        tr.Commit();

                        lblMsg.CssClass = "text-success mt-3 d-block text-center";
                        lblMsg.Text = $"Comanda plasata cu succes! ID Comanda: {idComanda} | Total: {total:0.00} lei";

                        // refresh UI
                        LoadCarti();
                        ddlCarti.SelectedValue = "0";
                        txtCantitate.Text = "1";
                        txtPret.Text = "";
                        txtStoc.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block text-center";
                lblMsg.Text = "Eroare: " + ex.Message;
            }
        }
    }
}
