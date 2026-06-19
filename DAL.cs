using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
// FIX for CS0234: Use Microsoft.Data.SqlClient or add System.Configuration.ConfigurationManager NuGet package if you want to use ConfigurationManager
// If you do not want to add the package, remove the usage of System.Configuration.ConfigurationManager below

namespace CRUDMahasiswaADO
{
    internal class DAL
    {
        // 1. Fungsi Connection String fleksibel menggunakan IP dinamis lokal
        public static string GetConnectionString()
        {
            // Sesuaikan User ID (sa) dan Password SQL Server kamu masing-masing
            return $"Data Source={GetLocalIPAddress()}; Initial Catalog=DBAkademikADO; User ID=sa; Password=123; TrustServerCertificate=True;";
        }

        // 2. Method Menghitung Jumlah Mahasiswa (Output Parameter)
        public int CountMhs()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter outputParam = new SqlParameter("@pCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outputParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outputParam.Value);
                }
            }
        }

        // 3. Method Mengambil Semua Data Mahasiswa
        public DataTable GetMhs()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                 
                    }
                }
            }
            return dt;
        }

        // 4. Method Insert Data Mahasiswa dengan Fitur Transaksi & BLOB Foto
        public void InsertMhs(string nim, string nama, string alamat, string jeniskelamin, DateTime tanggallahir, string kodeProdi, byte[] foto)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand command = new SqlCommand("sp_InsertMahasiswa", conn, trans))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@PNIM", nim);
                            command.Parameters.AddWithValue("@pNama", nama);
                            command.Parameters.AddWithValue("@pAlamat", alamat);
                            command.Parameters.AddWithValue("@pJenisKelamin", jeniskelamin);
                            command.Parameters.AddWithValue("@pTanggalLahir", tanggallahir);
                            command.Parameters.AddWithValue("@pKodeProdi", kodeProdi);
                            command.Parameters.AddWithValue("@pFoto", (object)foto ?? DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw; // Teruskan exception agar ditangkap MessageBox di Form UI
                    }
                }
            }
        }

        // 5. Method Update Data Mahasiswa (Termasuk Update BLOB Foto)
        public void UpdateMhs(string nim, string nama, string alamat, string jeniskelamin, DateTime tanggallahir, string kodeProdi, byte[] foto)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("sp_UpdateMahasiswa", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PNIM", nim);
                    command.Parameters.AddWithValue("@pNama", nama);
                    command.Parameters.AddWithValue("@pAlamat", alamat);
                    command.Parameters.AddWithValue("@pJenisKelamin", jeniskelamin);
                    command.Parameters.AddWithValue("@pTanggalLahir", tanggallahir);
                    command.Parameters.AddWithValue("@pKodeProdi", kodeProdi);
                    command.Parameters.AddWithValue("@pFoto", (object)foto ?? DBNull.Value);

                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        // 6. Method Delete Data Mahasiswa
        public void DeleteMhs(string nim)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PNIM", nim);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 7. Method Ambil Data untuk Chart Dashboard (Semua Data)
        public DataTable getAllDataChart()
        {
            DataTable dtMahasiswa = new DataTable();
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DashBoard", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtMahasiswa);
                    }
                }
            }
            return dtMahasiswa;
        }

        // 8. Method Ambil Data untuk Chart Dashboard Filter per Tahun
        public DataTable getDataChartByTahun(DateTime thMasuk)
        {
            DataTable dtMahasiswa = new DataTable();
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DashBoardByTahun", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTglMsuk", thMasuk.Year.ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtMahasiswa);
                    }
                }
            }
            return dtMahasiswa;
        }

        // 9. Method Log Message
        public void InsertLog(string message)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_LogMessage", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@psn", message);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 10. Helper mengambil IP Address lokal komputer secara dinamis
        private static string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new InvalidOperationException("Tidak ditemukan adapter jaringan dengan IPv4 aktif di sistem!");
        }
    }
}