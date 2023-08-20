using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace LibraryManagementSystem
{
    public partial class AddBooks : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\WINDOWS 10\Documents\library.mdf;Integrated Security=True;Connect Timeout=30");

        public AddBooks()
        {
            InitializeComponent();

            displayBooks();

        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }

            displayBooks();
        }

        private String imagePath;
        private void addBooks_importBtn_Click(object sender, EventArgs e)
        {
            
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png";

                if(dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    addBooks_picture.ImageLocation = imagePath;
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addBooks_addBtn_Click(object sender, EventArgs e)
        {
            if(addBooks_picture.Image == null
                || addBooks_bookTitle.Text == ""
                || addBooks_author.Text == ""
                || addBooks_published.Value == null
                || addBooks_status.Text == ""
                || addBooks_picture.Image == null)
            {
                MessageBox.Show("Please fill all blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if(connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        DateTime today = DateTime.Today;
                        connect.Open();
                        string insertData = "INSERT INTO books " +
                            "(book_title, author, published_date, status, image, date_insert) " +
                            "VALUES(@bookTitle, @author, @published_date, @status, @image, @dateInsert)";

                        string path = Path.Combine(@"C:\Users\WINDOWS 10\source\repos\LibraryManagementSystem\LibraryManagementSystem\Books_Directory\" +
                            addBooks_bookTitle.Text + addBooks_author.Text.Trim() + ".jpg");

                        string directoryPath = Path.GetDirectoryName(path);

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        File.Copy(addBooks_picture.ImageLocation, path, true);

                        using(SqlCommand cmd = new SqlCommand(insertData, connect))
                        {
                            cmd.Parameters.AddWithValue("@bookTitle", addBooks_bookTitle.Text.Trim());
                            cmd.Parameters.AddWithValue("@author", addBooks_author.Text.Trim());
                            cmd.Parameters.AddWithValue("@published_date", addBooks_published.Value);
                            cmd.Parameters.AddWithValue("@status", addBooks_status.Text.Trim());
                            cmd.Parameters.AddWithValue("@image", path);
                            cmd.Parameters.AddWithValue("@dateInsert", today);

                            cmd.ExecuteNonQuery();

                            displayBooks();

                            MessageBox.Show("Added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            clearFields();
                        }

                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        public void clearFields()
        {
            addBooks_bookTitle.Text = "";
            addBooks_author.Text = "";
            addBooks_picture.Image = null;
            addBooks_status.SelectedIndex = -1;
        }

        public void displayBooks()
        {
            DataAddBooks dab = new DataAddBooks();
            List<DataAddBooks> listData = dab.addBooksData();

            dataGridView1.DataSource = listData;

        }

        private int bookID = 0;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                bookID = (int)row.Cells[0].Value;
                addBooks_bookTitle.Text = row.Cells[1].Value.ToString();
                addBooks_author.Text = row.Cells[2].Value.ToString();
                addBooks_published.Text = row.Cells[3].Value.ToString();

                string imagePath = row.Cells[4].Value.ToString();


                if (imagePath != null || imagePath.Length >= 1)
                {
                    addBooks_picture.Image = Image.FromFile(imagePath);
                }
                else
                {
                    addBooks_picture.Image = null;
                }
                addBooks_status.Text = row.Cells[5].Value.ToString();
            }
        }

        private void addBooks_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void addBooks_updateBtn_Click(object sender, EventArgs e)
        {
            if (addBooks_picture.Image == null
                || addBooks_bookTitle.Text == ""
                || addBooks_author.Text == ""
                || addBooks_published.Value == null
                || addBooks_status.Text == ""
                || addBooks_picture.Image == null)
            {
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if(connect.State != ConnectionState.Open)
                {
                    DialogResult check = MessageBox.Show("Are you sure you want to UPDATE Book ID:"
                        + bookID + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if(check == DialogResult.Yes)
                    {
                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Today;
                            string updateData = "UPDATE books SET book_title = @bookTitle" +
                                ", author = @author, published_date = @published" +
                                ", status = @status, date_update = @dateUpdate WHERE id = @id";

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@bookTitle", addBooks_bookTitle.Text.Trim());
                                cmd.Parameters.AddWithValue("@author", addBooks_author.Text.Trim());
                                cmd.Parameters.AddWithValue("@published", addBooks_published.Value);
                                cmd.Parameters.AddWithValue("@status", addBooks_status.Text.Trim());
                                cmd.Parameters.AddWithValue("@dateUpdate", today);
                                cmd.Parameters.AddWithValue("@id", bookID);

                                cmd.ExecuteNonQuery();

                                displayBooks();

                                MessageBox.Show("Updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cancelled.", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    
                }
            }
        }

        private void addBooks_deleteBtn_Click(object sender, EventArgs e)
        {
            if (addBooks_picture.Image == null
                || addBooks_bookTitle.Text == ""
                || addBooks_author.Text == ""
                || addBooks_published.Value == null
                || addBooks_status.Text == ""
                || addBooks_picture.Image == null)
            {
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    DialogResult check = MessageBox.Show("Are you sure you want to DELETE Book ID:"
                        + bookID + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (check == DialogResult.Yes)
                    {
                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Today;
                            string updateData = "UPDATE books SET date_delete = @dateDelete WHERE id = @id";

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@dateDelete", today);
                                cmd.Parameters.AddWithValue("@id", bookID);

                                cmd.ExecuteNonQuery();

                                displayBooks();

                                MessageBox.Show("Deleted successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cancelled.", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
            }
        }
    }
}
