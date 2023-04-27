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

namespace StajProje1
{
    public partial class Form2 : Form
    {
        SqlConnection baglanti;
        SqlDataAdapter da;
        public Form2()
        {
            InitializeComponent();
            baglanti = new SqlConnection("Data Source=DESKTOP-R0966DO;Initial Catalog=RFID;Integrated Security=True");
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();
            dateTimePicker1.CustomFormat = "dd-MM-yyyy";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "dd-MM-yyyy";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            listPersonelCards();
            listPersonelCardInfo();
            listPersonnels();

            baglanti.Close();
        }

        public void listPersonelCards()
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();
            SqlCommand cmd = new SqlCommand("Select CardNumber From [PERSONNEL CARD] Where IsActive=0", baglanti);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr[0].ToString());
            }

            baglanti.Close();
        }

        public void listPersonelCardInfo()
        {
            da = new SqlDataAdapter("Select p.Ad, p.Soyad, pc.PersonnelID [TC Kimlik No], AC.numOfActiveCards [Aktif Kart Sayısı] " +
                "From [PERSONNEL CARD] pc inner join " +
    "PERSONEL p on pc.PersonnelID=p.[TC Kimlik No] inner join (Select PersonnelID, count(*) numOfactiveCards" +
    " From [PERSONNEL CARD] Where IsActive=1 Group By PersonnelID) AC on AC.PersonnelID=pc.PersonnelID Group By p.Ad, p.Soyad, " +
    "pc.PersonnelID, AC.numOfActiveCards", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            dataGridView1.Visible = true;
        }

        public void listPersonnels()
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();
            SqlCommand cmd = new SqlCommand("Select Ad+ ' '+Soyad From PERSONEL", baglanti);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comboBox2.Items.Add(dr[0].ToString());
            }

            baglanti.Close();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();
            string txt_cardNumber = comboBox1.Text;
            string txt_personnelNameSurname = comboBox2.Text;
            string txt_registerDate = dateTimePicker1.Text;
            string txt_cardValidityDate = dateTimePicker2.Text;

            TimeSpan ts = dateTimePicker2.Value - dateTimePicker1.Value;

            int dateDifference = ts.Days;

            if (txt_cardNumber == "" || txt_personnelNameSurname == "" || txt_registerDate == "" || txt_cardValidityDate == "")
            {
                MessageBox.Show("Tüm alanlar doldurulmalıdır.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (dateDifference < 0)
            {
                MessageBox.Show("Son kullanım tarihi kayıt tarihinden önce olamaz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                DialogResult dr2 = MessageBox.Show("Personele seçilen kartı atamak istediğinize emin misiniz?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr2 == DialogResult.Yes)
                {
                    SqlCommand cmd2 = new SqlCommand("Select [TC Kimlik No] From PERSONEL Where Ad+' '+Soyad=@item", baglanti);
                    cmd2.Parameters.AddWithValue("@item", txt_personnelNameSurname);
                    string personnelID = Convert.ToString(cmd2.ExecuteScalar());


                    SqlCommand cmd = new SqlCommand("Update [PERSONNEL CARD] Set PersonnelID=@persID, " +
                        "RegisterDate=@regDate, CardValidityDate=@ValidDate, IsActive=@isActive Where CardNumber=@cardNo", baglanti);

                    cmd.Parameters.Add("@cardNo", SqlDbType.VarChar, 250).Value = txt_cardNumber;
                    cmd.Parameters.Add("@persID", SqlDbType.VarChar, 250).Value = personnelID;
                    cmd.Parameters.Add("@regDate", SqlDbType.DateTime).Value = txt_registerDate;
                    cmd.Parameters.Add("@validDate", SqlDbType.DateTime).Value = txt_cardValidityDate;
                    cmd.Parameters.Add("@isActive", SqlDbType.Bit).Value = 1;

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Kart ataması başarıyla gerçekleştirildi.", "", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    listPersonelCardInfo();
                }
            }

            baglanti.Close();
        }

    }
}
