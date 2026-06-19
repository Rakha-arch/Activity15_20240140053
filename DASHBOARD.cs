using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; // Library wajib untuk Chart

namespace CRUDMahasiswaADO
{
    public partial class Dashboard : Form
    {
        // Panggil class logika database
        private readonly DAL dbLogic = new DAL();

        // Menyimpan data aktif agar saat ganti jenis grafik tidak perlu hit database lagi (Hemat Memory)
        private DataTable dtAktif;

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            // 1. Atur drop-down jenis grafik di UI Designer
            cmbJenis.DropDownStyle = ComboBoxStyle.DropDownList;

            // Set default pilihan ke indeks 0 (Column/Batang)
            cmbJenis.SelectedIndex = 0;

            // 2. Load semua data awal
            TampilGrafikSemua();
        }

        // Fungsi menampilkan grafik dari semua data
        private void TampilGrafikSemua()
        {
            try
            {
                dtAktif = dbLogic.getAllDataChart();
                BindDataKeChart(dtAktif);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat grafik: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Fungsi memasukkan data DataTable ke komponen Chart
        private void BindDataKeChart(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk ditampilkan pada periode ini.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                chartMhs.Series.Clear();
                return;
            }

            chartMhs.Series.Clear();

            // Membuat Series baru
            Series ser = new Series("Jumlah Mahasiswa");

            // Atur tipe grafik secara dinamis berdasarkan pilihan ComboBox 'cmbJenis'
            string tipeTerpilih = cmbJenis.SelectedItem?.ToString();
            if (tipeTerpilih == "Pie")
            {
                ser.ChartType = SeriesChartType.Pie;
                ser.IsValueShownAsLabel = true;
                ser.Label = "#VAL (#PERCENT)"; // Menampilkan jumlah dan persentase di grafik Pie
            }
            else if (tipeTerpilih == "Line")
            {
                ser.ChartType = SeriesChartType.Line;
                ser.IsValueShownAsLabel = true;
            }
            else
            {
                ser.ChartType = SeriesChartType.Column;
                ser.IsValueShownAsLabel = true;
            }

            // Looping memasukkan data ke dalam chart
            foreach (DataRow row in dt.Rows)
            {
                string namaProdi = row["NamaProdi"].ToString();

                // FIX: Gunakan Convert.ToInt32 langsung tanpa explicit casting (long) untuk menghindari crash tipe data
                int jumlah = Convert.ToInt32(row["JmlhMhs"]);

                ser.Points.AddXY(namaProdi, jumlah);
            }

            chartMhs.Series.Add(ser);
            chartMhs.Refresh();
        }

        // Event saat tombol Load diklik (Filter per tahun)
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                // dtpThn adalah DateTimePicker tahun Anda di UI Designer
                dtAktif = dbLogic.getDataChartByTahun(dtpThn.Value);
                BindDataKeChart(dtAktif);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memfilter grafik: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event saat pilihan ComboBox jenis grafik diubah oleh user
        private void cmbJenis_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cukup render ulang data yang sudah ada di memory, tidak perlu query ulang ke SQL Server
            if (dtAktif != null)
            {
                BindDataKeChart(dtAktif);
            }
        }

        // Event untuk pindah dari Dashboard ke Form Utama Mahasiswa
        private void btnKeFormMhs_Click(object sender, EventArgs e)
        {
            // Sesuaikan "FormMahasiswa" dengan nama class Form Utama kamu
            FormMahasiswa formUtama = new FormMahasiswa();
            formUtama.Show();
            this.Hide(); // Menyembunyikan dashboard sementara form utama terbuka
        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            try
            {
                // dtpThn adalah nama DateTimePicker tahun kamu
                DataTable dt = dbLogic.getDataChartByTahun(dtpThn.Value);

                // Panggil fungsi bind data untuk merender ulang isi grafik
                BindDataKeChart(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memfilter grafik: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKeFormMhs_Click_1(object sender, EventArgs e)
        {
            // Ganti 'FormMahasiswa' dengan nama class Form Utama tempat CRUD kamu berada
            FormMahasiswa formUtama = new FormMahasiswa();
            formUtama.Show();
            this.Hide(); // Menyembunyikan Dashboard agar fokus ke data mahasiswa
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dtpThn.Value = DateTime.Now;
                TampilGrafikSemua();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mereset grafik: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chartMhs_Click(object sender, EventArgs e)
        {
            
        }
    }
}