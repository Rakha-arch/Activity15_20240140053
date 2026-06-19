using CRUDMahasiswaADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Form3 : Form
    {
        private readonly string connectionString = "Data Source=GENZI\\RAKHAAAAAAAA;Initial Catalog=DBAkademikADO;User ID=sa;Password=123;TrustServerCertificate=True";
        private DataTable dtMahasiswa;
        private DataTable dtProdi;
        public Form3()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Pengaturan UI picker tahun
            dtpTanggalMasuk.Format = DateTimePickerFormat.Custom;
            dtpTanggalMasuk.CustomFormat = "yyyy";
            dtpTanggalMasuk.ShowUpDown = true;
            dtpTanggalMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTanggalMasuk.MaxDate = DateTime.Now;

            cmbProdi.DropDownStyle = ComboBoxStyle.DropDownList;
            btnCetak.Enabled = false;

            try
            {
                // Ambil kodeprodi juga untuk berjaga-jaga jika SP butuh kode, bukan nama teks
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT kodeprodi, namaprodi FROM programstudi";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        dtProdi = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtProdi);
                        }
                    }
                }

                cmbProdi.DataSource = dtProdi;
                cmbProdi.DisplayMember = "namaprodi";

                // ISI SESUAI KEBUTUHAN SP: 
                // Gunakan "kodeprodi" jika SP memfilter pakai Kode, gunakan "namaprodi" jika SP memfilter pakai Nama teks full.
                cmbProdi.ValueMember = "kodeprodi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data Prodi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void btnload_Click_1(object sender, EventArgs e)
        {
            if (cmbProdi.SelectedValue == null)
            {
                MessageBox.Show("Silahkan pilih Program Studi terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Report", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Proteksi parameter dari nilai null
                        cmd.Parameters.Add("@inProdi", SqlDbType.VarChar, 50).Value = cmbProdi.SelectedValue.ToString();
                        cmd.Parameters.Add("@inTglMsuk", SqlDbType.VarChar, 4).Value = dtpTanggalMasuk.Value.Year.ToString();

                        dtMahasiswa = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtMahasiswa);
                        }
                    }
                }

                dataGridView1.DataSource = dtMahasiswa;

                // Validasi data untuk tombol cetak
                if (dtMahasiswa.Rows.Count > 0)
                {
                    btnCetak.Enabled = true;
                }
                else
                {
                    btnCetak.Enabled = false;
                    MessageBox.Show("Data mahasiswa tidak ditemukan untuk kriteria ini.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void btncetak_Click(object sender, EventArgs e)
        {
            if (cmbProdi.SelectedValue != null)
            {
                // Oper data filter ke Form4 (Form Crystal Report / ReportViewer Anda)
                Form4 frm2 = new Form4(cmbProdi.SelectedValue.ToString(), dtpTanggalMasuk.Value);
                frm2.Show();
                this.Hide();
            }


        }
    }
}
