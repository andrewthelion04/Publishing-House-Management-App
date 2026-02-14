using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace EdituraApp
{
    public partial class ManageContracts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // verificare rol editor + protectie acces
            if (Session["rol"] == null || Session["rol"].ToString() != "editor")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // la prima incarcare, umplem dropdown-urile si lista de contracte
            if (!IsPostBack)
            {
                LoadAutoriEdituri();
                LoadContracts();
            }
        }

        // incarca listele de autori si edituri in dropdown-uri
        private void LoadAutoriEdituri()
        {
            ddlAutor.Items.Clear();
            ddlEditura.Items.Clear();

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // incarcare autori
                using (SqlCommand cmd = new SqlCommand("SELECT id_autor, nume, prenume FROM Autori ORDER BY nume, prenume", con))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string text = rdr["nume"].ToString() + " " + rdr["prenume"].ToString();
                        ddlAutor.Items.Add(new ListItem(text, rdr["id_autor"].ToString()));
                    }
                }

                // incarcare edituri
                using (SqlCommand cmd = new SqlCommand("SELECT id_editura, denumire FROM Edituri ORDER BY denumire", con))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ddlEditura.Items.Add(new ListItem(rdr["denumire"].ToString(), rdr["id_editura"].ToString()));
                    }
                }
            }
        }

        // incarca lista tuturor contractelor in grid
        private void LoadContracts()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            string sql = @"
SELECT
    cp.id_contract,
    (a.nume + ' ' + a.prenume) AS autor,
    e.denumire AS editura,
    cp.data_semnare,
    cp.data_expirare,
    cp.procent_royalty,
    CASE 
        WHEN cp.data_semnare <= CAST(GETDATE() AS date)
             AND (cp.data_expirare IS NULL OR cp.data_expirare >= CAST(GETDATE() AS date))
        THEN 'activ'
        ELSE 'inactiv'
    END AS status
FROM ContractePublicare cp
JOIN Autori a ON cp.id_autor = a.id_autor
JOIN Edituri e ON cp.id_editura = e.id_editura
ORDER BY cp.data_semnare DESC, cp.id_contract DESC";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvContracts.DataSource = dt;
                gvContracts.DataBind();

                lblMsg.Text = "";
                lblMsg.CssClass = "mt-3 d-block";
            }
        }

        // adauga un contract nou
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int idAutor = Convert.ToInt32(ddlAutor.SelectedValue);
            int idEditura = Convert.ToInt32(ddlEditura.SelectedValue);

            DateTime dataSemnare;
            DateTime dataExpirare;
            bool hasExp = DateTime.TryParse(txtExpirare.Text.Trim(), out dataExpirare);

            decimal royalty;

            // validare data semnare
            if (!DateTime.TryParse(txtSemnare.Text.Trim(), out dataSemnare))
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Data semnare invalida.";
                return;
            }

            // validare interval royalty
            if (!decimal.TryParse(txtRoyalty.Text.Trim(), out royalty) || royalty < 0 || royalty > 100)
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Royalty invalid (0..100).";
                return;
            }

            // validare relatie semnare - expirare
            if (hasExp && dataExpirare.Date < dataSemnare.Date)
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Data expirare nu poate fi mai mica decat data semnarii.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // nu permitem contract duplicat pentru aceeasi pereche autor-editura
                    SqlCommand check = new SqlCommand(
                        "SELECT COUNT(*) FROM ContractePublicare WHERE id_autor=@a AND id_editura=@e", con);
                    check.Parameters.AddWithValue("@a", idAutor);
                    check.Parameters.AddWithValue("@e", idEditura);

                    if ((int)check.ExecuteScalar() > 0)
                    {
                        lblAddMsg.CssClass = "text-danger mt-3 d-block";
                        lblAddMsg.Text = "Exista deja un contract intre acest autor si aceasta editura.";
                        return;
                    }

                    // insert contract nou
                    SqlCommand cmd = new SqlCommand(@"
INSERT INTO ContractePublicare (id_autor, id_editura, data_semnare, data_expirare, procent_royalty)
VALUES (@a, @e, @ds, @de, @r)", con);

                    cmd.Parameters.AddWithValue("@a", idAutor);
                    cmd.Parameters.AddWithValue("@e", idEditura);
                    cmd.Parameters.AddWithValue("@ds", dataSemnare.Date);
                    cmd.Parameters.AddWithValue("@de", hasExp ? (object)dataExpirare.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@r", royalty);

                    cmd.ExecuteNonQuery();
                }

                // reset campuri formular
                txtSemnare.Text = "";
                txtExpirare.Text = "";
                txtRoyalty.Text = "";

                lblAddMsg.CssClass = "text-success mt-3 d-block";
                lblAddMsg.Text = "Contract adaugat cu succes!";

                LoadContracts();
            }
            catch (Exception ex)
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Eroare insert: " + ex.Message;
            }
        }

        // gridview: intra in modul de editare
        protected void gvContracts_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvContracts.EditIndex = e.NewEditIndex;
            LoadContracts();
        }

        // gridview: anuleaza editarea
        protected void gvContracts_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvContracts.EditIndex = -1;
            LoadContracts();
        }

        // gridview: update contract existent
        protected void gvContracts_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int idContract = Convert.ToInt32(gvContracts.DataKeys[e.RowIndex].Value);

            // preluare controale din randul editat
            GridViewRow row = gvContracts.Rows[e.RowIndex];
            TextBox txtSemnare = (TextBox)row.FindControl("txtEditSemnare");
            TextBox txtExp = (TextBox)row.FindControl("txtEditExpirare");
            TextBox txtRoy = (TextBox)row.FindControl("txtEditRoyalty");

            DateTime ds;
            DateTime de;
            bool hasDe = DateTime.TryParse(txtExp.Text.Trim(), out de);

            decimal royalty;

            // validare data semnare
            if (!DateTime.TryParse(txtSemnare.Text.Trim(), out ds))
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Data semnare invalida.";
                return;
            }

            // validare interval royalty
            if (!decimal.TryParse(txtRoy.Text.Trim(), out royalty) || royalty < 0 || royalty > 100)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Royalty invalid (0..100).";
                return;
            }

            // validare relatie semnare - expirare
            if (hasDe && de.Date < ds.Date)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Data expirare nu poate fi mai mica decat data semnarii.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // update contract
                    SqlCommand cmd = new SqlCommand(@"
UPDATE ContractePublicare
SET data_semnare=@ds,
    data_expirare=@de,
    procent_royalty=@r
WHERE id_contract=@id", con);

                    cmd.Parameters.AddWithValue("@ds", ds.Date);
                    cmd.Parameters.AddWithValue("@de", hasDe ? (object)de.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@r", royalty);
                    cmd.Parameters.AddWithValue("@id", idContract);

                    cmd.ExecuteNonQuery();
                }

                gvContracts.EditIndex = -1;
                LoadContracts();

                lblMsg.CssClass = "text-success mt-3 d-block";
                lblMsg.Text = "Contract actualizat cu succes.";
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Eroare update: " + ex.Message;
            }
        }
    }
}
