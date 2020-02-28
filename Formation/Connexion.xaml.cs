using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Formation
{
    /// <summary>
    /// Logique d'interaction pour Connexion.xaml
    /// </summary>
    public partial class Connexion : Window
    {
        private SqlConnection _con;
        private SqlCommand _command;
        private SqlDataReader _reader;
        private int _result;
        private string _query;

        public Connexion()
        {
            _con = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={System.IO.Directory.GetCurrentDirectory()}\db-formation.mdf;Integrated Security=True");
            InitializeComponent();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bye Fellas ! ;D");
            Application.Current.Shutdown();
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            _query = "SELECT * FROM Administrateur WHERE username = @user and password = @pass";
            _con.Open();
            using (_command = new SqlCommand(_query, _con))
            {
                _command.CommandType = CommandType.Text;
                _command.Parameters.AddWithValue("@user", username.Text);
                _command.Parameters.AddWithValue("@pass", password.Password);
                _reader = _command.ExecuteReader();
                _reader.Close();
            }
            _con.Close();
            if (_reader.HasRows)
            {
                Main main = new Main(username.Text, _con);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect, dégage !");
            }
        }
    }
}
