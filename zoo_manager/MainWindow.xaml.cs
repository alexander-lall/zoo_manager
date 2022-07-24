using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace zoo_manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection SqlConnection { get; set; }

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["zoo_manager.Properties.Settings.contentDBConnectionString"].ConnectionString;
            SqlConnection = new SqlConnection(connectionString);

            ShowZoos();
            ShowAnimals();
        }

        #endregion

        #region Private Methods

        private void ShowZoos()
        {
            try
            {
                string query = "SELECT * FROM Zoo;";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, SqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);

                    listZoos.DisplayMemberPath = "location";
                    listZoos.SelectedValuePath = "id";
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ShowAnimals()
        {
            try
            {
                string query = "SELECT * FROM Animal;";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, SqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    listAnimals.DisplayMemberPath = "name";
                    listAnimals.SelectedValuePath = "id";
                    listAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ShowAssociatedAnimals()
        {
            try
            {
                string query = "SELECT * FROM Animal a INNER JOIN Zoo_Animal za " +
                    "ON a.id = za.animal_id WHERE za.zoo_id = @zoo_id;";
                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@zoo_id", listZoos.SelectedValue);

                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    listAssociatedAnimals.DisplayMemberPath = "name";
                    listAssociatedAnimals.SelectedValuePath = "id";
                    listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ShowSelectedZooInTextBox()
        {
            try
            {
                string query = "SELECT location FROM Zoo WHERE id = @zoo_id";
                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@zoo_id", listZoos.SelectedValue);

                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);

                    textBox.Text = zooTable.Rows[0]["location"].ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ShowSelectedAnimalInTextBox()
        {
            try
            {
                string query = "SELECT name FROM Animal WHERE id = @animal_id";
                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@animal_id", listAnimals.SelectedValue);

                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    textBox.Text = animalTable.Rows[0]["name"].ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        #endregion

        #region Event Handlers

        private void AddZoo_Click(object sender, RoutedEventArgs ea)
        {
            try
            {
                string query = "INSERT INTO Zoo (location) VALUES (@location)";

                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);

                SqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@location", textBox.Text.Trim());
                sqlCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                SqlConnection.Close();
                ShowZoos();
            }
        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs ea)
        {
            try
            {
                string query = "DELETE FROM Zoo WHERE id = @zoo_id;";

                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);

                SqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@zoo_id", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                SqlConnection.Close();
                ShowZoos();
                ShowAssociatedAnimals();
            }
        }

        private void UpdateZoo_Click(object sender, RoutedEventArgs ea)
        {
            try
            {
                string query = "UPDATE Zoo SET location = @location WHERE id = @zoo_id;";

                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);

                SqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@location", textBox.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@zoo_id", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                SqlConnection.Close();
                ShowZoos();
            }
        }

        private void AddAnimalToZoo_Click(object sender, RoutedEventArgs ea)
        {
            try
            {
                string query = "INSERT INTO Zoo_Animal (zoo_id, animal_id) VALUES (@zoo_id, @animal_id);";

                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);

                SqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@zoo_id", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@animal_id", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                SqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }

        private void RemoveAnimalFromZoo_Click(object sender, RoutedEventArgs ea)
        {
            try
            {
                string query = "DELETE FROM Zoo_Animal WHERE animal_id = @animal_id;";

                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);

                SqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@animal_id", listAssociatedAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                SqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }

        private void AddAnimal_Click(object sender, RoutedEventArgs ea)
        {
            try
            {
                string query = "INSERT INTO Animal (name) VALUES (@name)";

                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);

                SqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@name", textBox.Text.Trim());
                sqlCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                SqlConnection.Close();
                ShowAnimals();
            }
        }

        private void DeleteAnimal_Click(object sender, RoutedEventArgs ea)
        {
            try
            {
                string query = "DELETE FROM Animal WHERE id = @animal_id;";

                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);

                SqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@animal_id", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                SqlConnection.Close();
                ShowAnimals();
                ShowAssociatedAnimals();
            }
        }

        private void UpdateAnimal_Click(object sender, RoutedEventArgs ea)
        {
            try
            {
                string query = "UPDATE Animal SET name = @animal_name WHERE id = @animal_id;";

                SqlCommand sqlCommand = new SqlCommand(query, SqlConnection);

                SqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@animal_name", textBox.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@animal_id", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                SqlConnection.Close();
                ShowAnimals();
                ShowAssociatedAnimals();
            }
        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
            ShowSelectedZooInTextBox();
        }

        private void listAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedAnimalInTextBox();
        }

        #endregion


    }
}
