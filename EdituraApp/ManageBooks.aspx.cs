using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace EdituraApp
{
    public partial class ManageBooks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // protectie: doar editor
            if (Session["rol"] == null || Session["rol"].ToString() != "editor")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadAuthorsAndPublishers();
                LoadPublishersForSelectedAuthor(); // important pentru workflow
                LoadBooks();
            }
        }

        // -------------------------
        // incarca dropdown-uri
        // -------------------------
        private void LoadAuthorsAndPublishers()
        {
            // filtre (lista)
            ddlAuthorFilter.Items.Clear();
            ddlAuthorFilter.Items.Add(new ListItem("Toti autorii", "0"));

            ddlPublisherFilter.Items.Clear();
            ddlPublisherFilter.Items.Add(new ListItem("Toate editurile", "0"));

            // add form
            ddlNewAuthor.Items.Clear();
            ddlNewPublisher.Items.Clear();

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // autori
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT id_autor, nume, prenume FROM Autori ORDER BY nume, prenume", con))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string text = rdr["nume"].ToString() + " " + rdr["prenume"].ToString();
                        string val = rdr["id_autor"].ToString();

                        ddlAuthorFilter.Items.Add(new ListItem(text, val));
                        ddlNewAuthor.Items.Add(new ListItem(text, val));
                    }
                }

                // edituri pentru FILTRE (lista carti) - aici pastram toate editurile
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT id_editura, denumire FROM Edituri ORDER BY denumire", con))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ddlPublisherFilter.Items.Add(new ListItem(
                            rdr["denumire"].ToString(),
                            rdr["id_editura"].ToString()
                        ));
                    }
                }
            }

            // dropdown-ul ddlNewPublisher NU se incarca cu toate editurile,
            // ci se incarca filtrat dupa contractele autorului selectat
            // => LoadPublishersForSelectedAuthor() va fi chemat separat.
        }

        // dropdown-ul ddlNewAuthor are AutoPostBack + OnSelectedIndexChanged in .aspx
        protected void ddlNewAuthor_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPublishersForSelectedAuthor();
        }

        private void LoadPublishersForSelectedAuthor()
        {
            ddlNewPublisher.Items.Clear();

            int idAutor = 0;
            int.TryParse(ddlNewAuthor.SelectedValue, out idAutor);

            if (idAutor == 0)
            {
                ddlNewPublisher.Items.Add(new ListItem("Selecteaza autorul intai", "0"));
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            // doar editurile cu contract ACTIV pentru autor
            string sql = @"
SELECT e.id_editura, e.denumire
FROM ContractePublicare cp
JOIN Edituri e ON cp.id_editura = e.id_editura
WHERE cp.id_autor = @idAutor
  AND cp.data_semnare <= CAST(GETDATE() AS date)
  AND (cp.data_expirare IS NULL OR cp.data_expirare >= CAST(GETDATE() AS date))
ORDER BY e.denumire";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@idAutor", idAutor);

                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ddlNewPublisher.Items.Add(new ListItem(
                            rdr["denumire"].ToString(),
                            rdr["id_editura"].ToString()
                        ));
                    }
                }
            }

            if (ddlNewPublisher.Items.Count == 0)
            {
                ddlNewPublisher.Items.Add(new ListItem("Nu exista contract activ pentru acest autor", "0"));
            }
        }

        // -------------------------
        // filtre lista
        // -------------------------
        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            gvBooks.EditIndex = -1;
            LoadBooks();
        }

        protected void btnResetFilters_Click(object sender, EventArgs e)
        {
            txtTitle.Text = "";
            txtGen.Text = "";
            ddlAuthorFilter.SelectedIndex = 0;
            ddlPublisherFilter.SelectedIndex = 0;

            gvBooks.EditIndex = -1;
            LoadBooks();
        }

        // -------------------------
        // listare carti (JOIN)
        // -------------------------
        private void LoadBooks()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            int idAutor = 0;
            int idEditura = 0;
            int.TryParse(ddlAuthorFilter.SelectedValue, out idAutor);
            int.TryParse(ddlPublisherFilter.SelectedValue, out idEditura);

            string title = txtTitle.Text.Trim();
            string gen = txtGen.Text.Trim();

            string sql = @"
SELECT
    c.id_carte,
    c.titlu,
    c.gen,
    c.pret,
    c.stoc,
    (a.nume + ' ' + a.prenume) AS autor,
    e.denumire AS editura
FROM Carti c
JOIN Autori a ON c.id_autor = a.id_autor
JOIN Edituri e ON c.id_editura = e.id_editura
WHERE 1=1
";

            // parametri variabili pentru JOIN (bifeaza cerinta)
            if (!string.IsNullOrWhiteSpace(title)) sql += " AND c.titlu LIKE @title";
            if (!string.IsNullOrWhiteSpace(gen)) sql += " AND c.gen LIKE @gen";
            if (idAutor != 0) sql += " AND c.id_autor = @idAutor";
            if (idEditura != 0) sql += " AND c.id_editura = @idEditura";

            sql += " ORDER BY c.titlu";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                if (!string.IsNullOrWhiteSpace(title)) cmd.Parameters.AddWithValue("@title", "%" + title + "%");
                if (!string.IsNullOrWhiteSpace(gen)) cmd.Parameters.AddWithValue("@gen", "%" + gen + "%");
                if (idAutor != 0) cmd.Parameters.AddWithValue("@idAutor", idAutor);
                if (idEditura != 0) cmd.Parameters.AddWithValue("@idEditura", idEditura);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvBooks.DataSource = dt;
                    gvBooks.DataBind();

                    lblCount.Text = "Rezultate: " + dt.Rows.Count;
                    lblMsg.Text = "";
                    lblMsg.CssClass = "mt-3 d-block";
                }
            }
        }

        // -------------------------
        // gridview update pret/stoc
        // -------------------------
        protected void gvBooks_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvBooks.EditIndex = e.NewEditIndex;
            LoadBooks();
        }

        protected void gvBooks_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvBooks.EditIndex = -1;
            LoadBooks();
        }

        protected void gvBooks_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int idCarte = Convert.ToInt32(gvBooks.DataKeys[e.RowIndex].Value);

            GridViewRow row = gvBooks.Rows[e.RowIndex];
            TextBox txtPret = (TextBox)row.FindControl("txtEditPret");
            TextBox txtStoc = (TextBox)row.FindControl("txtEditStoc");

            decimal pret;
            int stoc;

            if (!decimal.TryParse(txtPret.Text.Trim(), out pret) || pret < 0)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Pret invalid. Introdu un numar >= 0.";
                return;
            }

            if (!int.TryParse(txtStoc.Text.Trim(), out stoc) || stoc < 0)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Stoc invalid. Introdu un numar intreg >= 0.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(@"
UPDATE Carti
SET pret = @pret, stoc = @stoc
WHERE id_carte = @id", con);

                    cmd.Parameters.AddWithValue("@pret", pret);
                    cmd.Parameters.AddWithValue("@stoc", stoc);
                    cmd.Parameters.AddWithValue("@id", idCarte);

                    cmd.ExecuteNonQuery();
                }

                gvBooks.EditIndex = -1;
                LoadBooks();

                lblMsg.CssClass = "text-success mt-3 d-block";
                lblMsg.Text = "Cartea a fost actualizata cu succes.";
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Eroare update: " + ex.Message;
            }
        }

        // -------------------------
        // insert carte noua (cu editura filtrata din contracte)
        // -------------------------
        protected void btnAddBook_Click(object sender, EventArgs e)
        {
            string title = txtNewTitle.Text.Trim();
            string gen = txtNewGen.Text.Trim();

            decimal pret;
            int stoc;

            if (string.IsNullOrWhiteSpace(title))
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Titlul este obligatoriu.";
                return;
            }

            if (!decimal.TryParse(txtNewPrice.Text.Trim(), out pret) || pret < 0)
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Pret invalid (>= 0).";
                return;
            }

            if (!int.TryParse(txtNewStock.Text.Trim(), out stoc) || stoc < 0)
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Stoc invalid (intreg >= 0).";
                return;
            }

            int idAutor = Convert.ToInt32(ddlNewAuthor.SelectedValue);
            int idEditura = Convert.ToInt32(ddlNewPublisher.SelectedValue);

            // regula workflow: editura trebuie sa fie una contractata (altfel ddlNewPublisher va avea 0)
            if (idEditura == 0)
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Nu poti adauga cartea: autorul nu are contract activ cu nicio editura (sau nu ai selectat o editura).";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // verificare suplimentara (server-side) ca exista contract activ autor-editura
                    SqlCommand checkContract = new SqlCommand(@"
SELECT COUNT(*)
FROM ContractePublicare
WHERE id_autor=@a AND id_editura=@e
  AND data_semnare <= CAST(GETDATE() AS date)
  AND (data_expirare IS NULL OR data_expirare >= CAST(GETDATE() AS date))", con);

                    checkContract.Parameters.AddWithValue("@a", idAutor);
                    checkContract.Parameters.AddWithValue("@e", idEditura);

                    if ((int)checkContract.ExecuteScalar() == 0)
                    {
                        lblAddMsg.CssClass = "text-danger mt-3 d-block";
                        lblAddMsg.Text = "Nu exista contract ACTIV intre autor si editura selectata.";
                        return;
                    }

                    SqlCommand cmd = new SqlCommand(@"
INSERT INTO Carti (titlu, gen, pret, stoc, id_autor, id_editura)
VALUES (@t, @g, @p, @s, @a, @e)", con);

                    cmd.Parameters.AddWithValue("@t", title);
                    cmd.Parameters.AddWithValue("@g", string.IsNullOrWhiteSpace(gen) ? (object)DBNull.Value : gen);
                    cmd.Parameters.AddWithValue("@p", pret);
                    cmd.Parameters.AddWithValue("@s", stoc);
                    cmd.Parameters.AddWithValue("@a", idAutor);
                    cmd.Parameters.AddWithValue("@e", idEditura);

                    cmd.ExecuteNonQuery();
                }

                // clear form
                txtNewTitle.Text = "";
                txtNewGen.Text = "";
                txtNewPrice.Text = "";
                txtNewStock.Text = "";

                // refresh edituri pentru autorul selectat (ramane filtrat)
                LoadPublishersForSelectedAuthor();

                lblAddMsg.CssClass = "text-success mt-3 d-block";
                lblAddMsg.Text = "Carte adaugata cu succes!";

                // refresh list
                LoadBooks();
            }
            catch (Exception ex)
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Eroare insert: " + ex.Message;
            }
        }
    }
}
