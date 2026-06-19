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
    public partial class Form4 : Form
    {
        // Pindahkan string koneksi ke level class (Readonly)
        private readonly string connectionString = "Data Source=GENZI\\RAKHAAAAAAAA;Initial Catalog=DBAkademikADO;User ID=sa;Password=123;TrustServerCertificate=True";

        // Gunakan properti dengan penamaan PascalCase agar rapi (Best Practice C#)
        public string ProdiId { get; set; }
        public DateTime TglMasuk { get; set; }

        public Form4(string prodi, DateTime tglmasuk)
        {
            InitializeComponent();

            // 1. FIX: Menggunakan 'this.' agar nilai parameter masuk ke properti class
            this.ProdiId = prodi;
            this.TglMasuk = tglmasuk;

            // Daftarkan event FormClosing secara programmatically jika belum dipasang via Designer
            this.FormClosing += Form4_FormClosing;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            // Pindahkan logika load data ke Form_Load agar UI Form muncul terlebih dahulu baru mengambil data
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                DataTable dtMahasiswa = new DataTable();

                // 2. FIX: Menggunakan blok 'using' untuk mencegah kebocoran koneksi database
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Report", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Isi parameter sesuai dengan tipe data yang diminta Stored Procedure
                        cmd.Parameters.Add("@inProdi", SqlDbType.VarChar, 50).Value = this.ProdiId;

                        // FIX: Diubah menjadi .ToString() agar tipenya VarChar(4) sesuai rancangan awal
                        cmd.Parameters.Add("@inTglMsuk", SqlDbType.VarChar, 4).Value = this.TglMasuk.Year.ToString();

                        conn.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtMahasiswa);
                        }
                    }
                } // Koneksi otomatis ditutup di sini

                // Pasang data ke Crystal Report
                CrystalReport1 listMahasiswa = new CrystalReport1();
                listMahasiswa.SetDataSource(dtMahasiswa);

                crystalReportViewer1.ReportSource = listMahasiswa;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat cetakan laporan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. FIX: Menangani tombol Close (X) agar aplikasi tidak menggantung di background
        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Menutup seluruh aplikasi termasuk Form3 yang sedang di-hide
            Application.Exit();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            // Kosongkan jika tidak digunakan
        }
    }
}
