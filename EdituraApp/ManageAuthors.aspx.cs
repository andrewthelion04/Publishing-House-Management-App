using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace EdituraApp
{
    public partial class ManageAuthors : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // verificare rol editor + protectie acces
            if (Session["rol"] == null || Session["rol"].ToString() != "editor")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // la prima incarcare, incarca autorii
            if (!IsPostBack)
            {
                LoadAuthors();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // scoate grid-ul din edit si aplica cautarea
            gvAuthors.EditIndex = -1;
            LoadAuthors();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            // reseteaza cautarea si reincarca lista
            txtSearch.Text = "";
            gvAuthors.EditIndex = -1;
            LoadAuthors();
        }

        // incarca autori + optional filtru de cautare
        private void LoadAuthors()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;
            string search = txtSearch.Text.Trim();

            // sql cu filtru optional
            string sql = @"
SELECT id_autor, nume, prenume, nationalitate, gen_literar, email
FROM Autori
WHERE 1=1
";

            if (!string.IsNullOrWhiteSpace(search))
                sql += " AND (nume LIKE @s OR prenume LIKE @s)";

            sql += " ORDER BY nume, prenume";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                // parametru filtru
                if (!string.IsNullOrWhiteSpace(search))
                    cmd.Parameters.AddWithValue("@s", "%" + search + "%");

                // umple gridview
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvAuthors.DataSource = dt;
                    gvAuthors.DataBind();

                    lblCount.Text = "Rezultate: " + dt.Rows.Count;
                    lblMsg.Text = "";
                }
            }
        }

        // intra in modul de editare pt un autor
        protected void gvAuthors_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvAuthors.EditIndex = e.NewEditIndex;
            LoadAuthors();
        }

        // anuleaza editarea
        protected void gvAuthors_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvAuthors.EditIndex = -1;
            LoadAuthors();
        }

        // update informatii autor
        protected void gvAuthors_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int idAutor = Convert.ToInt32(gvAuthors.DataKeys[e.RowIndex].Value);

            // extrage valori din rand
            GridViewRow row = gvAuthors.Rows[e.RowIndex];

            TextBox txtNume = (TextBox)row.FindControl("txtEditNume");
            TextBox txtPrenume = (TextBox)row.FindControl("txtEditPrenume");
            TextBox txtNat = (TextBox)row.FindControl("txtEditNat");
            TextBox txtGen = (TextBox)row.FindControl("txtEditGen");
            TextBox txtEmail = (TextBox)row.FindControl("txtEditEmail");

            string nume = txtNume.Text.Trim();
            string prenume = txtPrenume.Text.Trim();
            string nat = txtNat.Text.Trim();
            string gen = txtGen.Text.Trim();
            string email = txtEmail.Text.Trim();

            // validare de baza
            if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume))
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Nume si prenume sunt obligatorii.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // validare email unic
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        SqlCommand checkEmail = new SqlCommand(
                            "SELECT COUNT(*) FROM Autori WHERE email=@e AND id_autor<>@id", con);
                        checkEmail.Parameters.AddWithValue("@e", email);
                        checkEmail.Parameters.AddWithValue("@id", idAutor);

                        if ((int)checkEmail.ExecuteScalar() > 0)
                        {
                            lblMsg.CssClass = "text-danger mt-3 d-block";
                            lblMsg.Text = "Email-ul este deja folosit.";
                            return;
                        }
                    }

                    // update autor
                    SqlCommand cmd = new SqlCommand(@"
UPDATE Autori
SET nume=@n, prenume=@p, nationalitate=@nat, gen_literar=@g, email=@e
WHERE id_autor=@id", con);

                    cmd.Parameters.AddWithValue("@n", nume);
                    cmd.Parameters.AddWithValue("@p", prenume);
                    cmd.Parameters.AddWithValue("@nat", string.IsNullOrWhiteSpace(nat) ? (object)DBNull.Value : nat);
                    cmd.Parameters.AddWithValue("@g", string.IsNullOrWhiteSpace(gen) ? (object)DBNull.Value : gen);
                    cmd.Parameters.AddWithValue("@e", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);
                    cmd.Parameters.AddWithValue("@id", idAutor);

                    cmd.ExecuteNonQuery();
                }

                gvAuthors.EditIndex = -1;
                LoadAuthors();

                lblMsg.CssClass = "text-success mt-3 d-block";
                lblMsg.Text = "Autor actualizat.";
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "text-danger mt-3 d-block";
                lblMsg.Text = "Eroare update: " + ex.Message;
            }
        }

        // insert autor nou
        protected void btnAddAuthor_Click(object sender, EventArgs e)
        {
            // preluare date
            string nume = txtNewNume.Text.Trim();
            string prenume = txtNewPrenume.Text.Trim();
            string nat = txtNewNat.Text.Trim();
            string gen = txtNewGen.Text.Trim();
            string email = txtNewEmail.Text.Trim();

            // validare minima
            if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(prenume))
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Nume si prenume obligatorii.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // validare email unic
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        SqlCommand checkEmail = new SqlCommand(
                            "SELECT COUNT(*) FROM Autori WHERE email=@e", con);
                        checkEmail.Parameters.AddWithValue("@e", email);

                        if ((int)checkEmail.ExecuteScalar() > 0)
                        {
                            lblAddMsg.CssClass = "text-danger mt-3 d-block";
                            lblAddMsg.Text = "Email-ul este deja folosit.";
                            return;
                        }
                    }

                    // insert
                    SqlCommand cmd = new SqlCommand(@"
INSERT INTO Autori (nume, prenume, nationalitate, gen_literar, email)
VALUES (@n, @p, @nat, @g, @e)", con);

                    cmd.Parameters.AddWithValue("@n", nume);
                    cmd.Parameters.AddWithValue("@p", prenume);
                    cmd.Parameters.AddWithValue("@nat", string.IsNullOrWhiteSpace(nat) ? (object)DBNull.Value : nat);
                    cmd.Parameters.AddWithValue("@g", string.IsNullOrWhiteSpace(gen) ? (object)DBNull.Value : gen);
                    cmd.Parameters.AddWithValue("@e", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);

                    cmd.ExecuteNonQuery();
                }

                // reset campuri
                txtNewNume.Text = "";
                txtNewPrenume.Text = "";
                txtNewNat.Text = "";
                txtNewGen.Text = "";
                txtNewEmail.Text = "";

                lblAddMsg.CssClass = "text-success mt-3 d-block";
                lblAddMsg.Text = "Autor adaugat.";

                LoadAuthors();
            }
            catch (Exception ex)
            {
                lblAddMsg.CssClass = "text-danger mt-3 d-block";
                lblAddMsg.Text = "Eroare insert: " + ex.Message;
            }
        }
    }
}
