using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class FormMahasiswa : Form
    {
        DAL dbLogic = new DAL();

        private BindingSource bindingSource = new BindingSource();
        private DataTable dtMahasiswa = new DataTable();
        private readonly SqlConnection conn;
        private readonly string connectionString =
        "Data Source=GENZI\\RAKHAAAAAAAA;Initial Catalog=DBAkademikADO;User ID=sa;Password=123;TrustServerCertificate=True";

        public FormMahasiswa()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ComboBox JK manual
            cmbJK.DataSource = new string[] { "L", "p" };

            // Setting Grid
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // BindingNavigator
            bindingNavigator1.BindingSource = bindingSource;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dtMahasiswa = new DataTable();
                            da.Fill(dtMahasiswa);

                            bindingSource.DataSource = dtMahasiswa;
                            dataGridView1.DataSource = bindingSource;

                            BindingControls();
                        }
                    }
                }

                HitungTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void HitungTotal()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        ParameterDirection direction = ParameterDirection.Output;
                        SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int)
                        {
                            Direction = direction
                        };
                        cmd.Parameters.Add(outputParam);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        lblTotal.Text = "Total Mahasiswa: " + outputParam.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal hitung total: " + ex.Message);
            }
        }

        private void BindingControls()
        {
            txtNIM.DataBindings.Clear();
            txtNama.DataBindings.Clear();
            cmbJK.DataBindings.Clear();
            dtpTanggalLahir.DataBindings.Clear();
            txtAlamat.DataBindings.Clear();
            txtKodeProdi.DataBindings.Clear();

            txtNIM.DataBindings.Add("Text", bindingSource, "NIM");
            txtNama.DataBindings.Add("Text", bindingSource, "Nama");
            cmbJK.DataBindings.Add("Text", bindingSource, "JenisKelamin");
            dtpTanggalLahir.DataBindings.Add("Value", bindingSource, "TanggalLahir", true, DataSourceUpdateMode.OnPropertyChanged);
            txtAlamat.DataBindings.Add("Text", bindingSource, "Alamat");
            txtKodeProdi.DataBindings.Add("Text", bindingSource, "KodeProdi");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ConnectDatabase()
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                MessageBox.Show("Koneksi berhasil!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    MessageBox.Show("Koneksi berhasil");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }

        DAL db = new DAL();
        private void btnLoad_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = db.GetMhs();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtNIM.Text = row.Cells["NIM"].Value.ToString();
                txtNama.Text = row.Cells["Nama"].Value.ToString();
                cmbJK.Text = row.Cells["JenisKelamin"].Value.ToString();
                dtpTanggalLahir.Value = Convert.ToDateTime(row.Cells["TanggalLahir"].Value);
                txtAlamat.Text = row.Cells["Alamat"].Value.ToString();
                txtKodeProdi.Text = row.Cells["KodeProdi"].Value.ToString();
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                // === COMMAND 1: Insert data mahasiswa via Stored Procedure ===
                SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn, trans);
                cmd.CommandType = CommandType.StoredProcedure;

                // Menyesuaikan dengan parameter di SP (menggunakan @p...)
                cmd.Parameters.AddWithValue("@pNIM", txtNIM.Text);
                cmd.Parameters.AddWithValue("@pNama", txtNama.Text);
                cmd.Parameters.AddWithValue("@pAlamat", txtAlamat.Text);
                cmd.Parameters.AddWithValue("@pJenisKelamin", cmbJK.Text);
                cmd.Parameters.AddWithValue("@pTanggalLahir", dtpTanggalLahir.Value.Date);
                cmd.Parameters.AddWithValue("@pKodeProdi", txtKodeProdi.Text);

                // Mengubah gambar dari PictureBox menjadi byte array untuk @pFoto
                byte[] fotoBytes = null;
                if (pictureBox1.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Bitmap bmp = new Bitmap(pictureBox1.Image);
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        fotoBytes = ms.ToArray();
                    }
                }
                cmd.Parameters.Add("@pFoto", SqlDbType.VarBinary).Value = (object)fotoBytes ?? DBNull.Value;

                cmd.ExecuteNonQuery();

                // === COMMAND 2: Insert ke LogAktivitas ===
                SqlCommand cmdLog = new SqlCommand(
                    @"INSERT INTO LogAktivitas (aktivitas, waktu) 
              VALUES (@aktivitas, GETDATE())", conn, trans);

                cmdLog.Parameters.AddWithValue("@aktivitas", "INSERT MAHASISWA : " + txtNIM.Text);
                cmdLog.ExecuteNonQuery();

                trans.Commit();

                MessageBox.Show("Data berhasil ditambahkan");
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                SimpanLog("ERROR INSERT : " + ex.Message);
                MessageBox.Show("Gagal menyimpan: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Menyesuaikan dengan parameter di SP (menggunakan @p...)
                        cmd.Parameters.AddWithValue("@pNIM", txtNIM.Text);
                        cmd.Parameters.AddWithValue("@pNama", txtNama.Text);
                        cmd.Parameters.AddWithValue("@pAlamat", txtAlamat.Text);
                        cmd.Parameters.AddWithValue("@pJenisKelamin", cmbJK.Text);
                        cmd.Parameters.AddWithValue("@pTanggalLahir", dtpTanggalLahir.Value.Date);
                        cmd.Parameters.AddWithValue("@pKodeProdi", txtKodeProdi.Text);

                        // Mengubah gambar dari PictureBox menjadi byte array untuk @pFoto
                        byte[] fotoBytes = null;
                        if (pictureBox1.Image != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                Bitmap bmp = new Bitmap(pictureBox1.Image);
                                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                fotoBytes = ms.ToArray();
                            }
                        }
                        cmd.Parameters.Add("@pFoto", SqlDbType.VarBinary).Value = (object)fotoBytes ?? DBNull.Value;

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Data berhasil diupdate!");
                            ClearForm();
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Data tidak ditemukan.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal update: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult konfirmasi = MessageBox.Show(
                    "Yakin ingin menghapus data ini?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (konfirmasi != DialogResult.Yes) return;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NIM", SqlDbType.Char, 11).Value = txtNIM.Text;

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Data berhasil dihapus!");
                            ClearForm();
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Data tidak ditemukan.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal hapus: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtNIM.Clear();
            txtNama.Clear();
            cmbJK.SelectedIndex = -1;
            txtAlamat.Clear();
            txtKodeProdi.Clear();
            dtpTanggalLahir.Value = DateTime.Now;
            txtNIM.Focus();
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                    IF OBJECT_ID('dbo.Mahasiswa_Backup') IS NOT NULL
                    BEGIN
                        DELETE FROM dbo.Mahasiswa;
                        INSERT INTO dbo.Mahasiswa
                        SELECT * FROM dbo.Mahasiswa_Backup;
                    END";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil direset");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reset gagal: " + ex.Message);
            }
        }

        private void btnTestInjection_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Gunakan Parameter agar aman dari serangan
                    string query = "UPDATE Mahasiswa SET Nama = 'HACKED' WHERE NIM = @NIM";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@NIM", SqlDbType.Char, 11).Value = txtNIM.Text;
                        int result = cmd.ExecuteNonQuery();
                        MessageBox.Show(result + " baris terupdate");
                    }
                }
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void SimpanLog(string aktivitas)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO LogAktivitas (aktivitas, waktu) VALUES (@aktivitas, GETDATE())", conn))
                    {
                        cmd.Parameters.AddWithValue("@aktivitas", aktivitas);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Optional: handle error
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Dashboard fmDash = new Dashboard();
            fmDash.Show();
            this.Hide();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg; *.jpeg; *.png; *.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(ofd.FileName);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNIM.Text))
            {
                MessageBox.Show("Silahkan masukkan NIM terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM mahasiswa WHERE NIM = @NIM";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                txtNama.Text = dr["Nama"].ToString();
                                txtAlamat.Text = dr["Alamat"].ToString();
                                txtKodeProdi.Text = dr["KodeProdi"].ToString();
                                cmbJK.Text = dr["JenisKelamin"].ToString();
                                dtpTanggalLahir.Value = Convert.ToDateTime(dr["TanggalLahir"]);

                                if (dr["Foto"] != DBNull.Value)
                                {
                                    byte[] img = (byte[])dr["Foto"];
                                    using (MemoryStream ms = new MemoryStream(img))
                                    {
                                        pictureBox1.Image = Image.FromStream(ms);
                                    }
                                }
                                else
                                {
                                    pictureBox1.Image = null;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Data Mahasiswa tidak ditemukan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mencari data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnrefresh_Click(object sender, EventArgs e)
        {
            LoadData();
            MessageBox.Show("Data berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Workbook|*.xlsx;*.xls";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                using (var stream = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                        });

                        DataTable dt = result.Tables[0];
                        dataGridView1.DataSource = dt;
                    }
                }
            }
        }

        private void btnImportDb_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        using (SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Menyesuaikan dengan susunan parameter @p... dan menghapus @TanggalDaftar
                            cmd.Parameters.AddWithValue("@pNIM", row.Cells["NIM"].Value.ToString());
                            cmd.Parameters.AddWithValue("@pNama", row.Cells["Nama"].Value.ToString());
                            cmd.Parameters.AddWithValue("@pAlamat", row.Cells["Alamat"].Value.ToString());
                            cmd.Parameters.AddWithValue("@pJenisKelamin", row.Cells["JenisKelamin"].Value.ToString());
                            cmd.Parameters.AddWithValue("@pTanggalLahir", Convert.ToDateTime(row.Cells["TanggalLahir"].Value).Date);
                            cmd.Parameters.AddWithValue("@pKodeProdi", row.Cells["KodeProdi"].Value.ToString());
                            cmd.Parameters.AddWithValue("@pFoto", DBNull.Value); // Default kosong untuk import Excel

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                MessageBox.Show("Semua data Excel sukses di-import ke Database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data Excel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    }
}
