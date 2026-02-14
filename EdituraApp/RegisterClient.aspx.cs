using System;
using System.Data.SqlClient;
using System.Configuration;

namespace EdituraWeb
{
    public partial class RegisterClient : System.Web.UI.Page
    {
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EdituraDB"].ConnectionString;

            string user = txtUser.Text.Trim();
            string pass = txtPass.Text.Trim();
            string nume = txtNume.Text.Trim();
            string prenume = txtPrenume.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefon = txtTelefon.Text.Trim();
            string adresa = txtAdresa.Text.Trim();

            // VALIDARE CAMPURI OBLIGATORII
            if (string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(pass) ||
                string.IsNullOrWhiteSpace(nume) ||
                string.IsNullOrWhiteSpace(prenume) ||
                string.IsNullOrWhiteSpace(email))
            {
                lblMsg.CssClass = "text-danger";
                lblMsg.Text = "Toate câmpurile obligatorii trebuie completate!";
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // VALIDARE USERNAME UNIC
                    SqlCommand checkUser = new SqlCommand(
                        "SELECT COUNT(*) FROM Utilizatori WHERE username=@u", con);
                    checkUser.Parameters.AddWithValue("@u", user);

                    if ((int)checkUser.ExecuteScalar() > 0)
                    {
                        lblMsg.CssClass = "text-danger";
                        lblMsg.Text = "Acest username este deja folosit!";
                        return;
                    }

                    // VALIDARE EMAIL UNIC
                    SqlCommand checkEmail = new SqlCommand(
                        "SELECT COUNT(*) FROM Clienti WHERE email=@e", con);
                    checkEmail.Parameters.AddWithValue("@e", email);

                    if ((int)checkEmail.ExecuteScalar() > 0)
                    {
                        lblMsg.CssClass = "text-danger";
                        lblMsg.Text = "Există deja un cont cu acest email!";
                        return;
                    }

                    // INSERT ÎN UTILIZATORI
                    string insertUser =
                        "INSERT INTO Utilizatori (username, parola, rol) OUTPUT INSERTED.id_utilizator VALUES (@u, @p, 'client')";

                    SqlCommand cmdUser = new SqlCommand(insertUser, con);
                    cmdUser.Parameters.AddWithValue("@u", user);
                    cmdUser.Parameters.AddWithValue("@p", pass);

                    int newUserId = (int)cmdUser.ExecuteScalar();

                    // INSERT ÎN CLIENTI
                    string insertClient =
                        "INSERT INTO Clienti (nume, prenume, email, telefon, adresa, id_utilizator) " +
                        "VALUES (@n, @pr, @e, @t, @a, @id)";

                    SqlCommand cmdClient = new SqlCommand(insertClient, con);
                    cmdClient.Parameters.AddWithValue("@n", nume);
                    cmdClient.Parameters.AddWithValue("@pr", prenume);
                    cmdClient.Parameters.AddWithValue("@e", email);
                    cmdClient.Parameters.AddWithValue("@t", telefon);
                    cmdClient.Parameters.AddWithValue("@a", adresa);
                    cmdClient.Parameters.AddWithValue("@id", newUserId);

                    cmdClient.ExecuteNonQuery();

                    lblMsg.CssClass = "text-success";
                    lblMsg.Text = "Cont creat cu succes! Redirecționare...";

                    // redirect după 2 secunde
                    Response.AddHeader("REFRESH", "2;URL=Login.aspx");
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
