using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace StajProje1
{
    public partial class Form1 : Form
    {
        SqlConnection baglanti;
        SqlDataAdapter da;
        public Form1()
        {
            InitializeComponent();
            baglanti = new SqlConnection("Data Source=DESKTOP-R0966DO;Initial Catalog=RFID;Integrated Security=True");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            dateTimePicker1.CustomFormat = "dd-MM-yyyy";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
        }

        public void clearTextBoxes()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            dateTimePicker1.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";

            radioButton1.Checked = false;
            radioButton2.Checked = false;
        }


        public void list2()
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();
            if (updateOperationIsInProcess())
            {

                DialogResult dr = MessageBox.Show("Güncelleme işlemi iptal edilecektir. Onaylıyor musunuz?", "",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.No)
                {
                    return;
                }
                else if (dr == DialogResult.Yes)
                {
                    clearTextBoxes();
                    button5.Enabled = false;
                    enableAllCheckBoxes();
                }

            }

            da = new SqlDataAdapter("Select * from PERSONEL", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            dataGridView1.Visible = true;
            baglanti.Close();
        }


        public int numberOfSelectedRows()
        {
            int countOfSelectedPersonels = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                bool isCellChecked = Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value);
                if (isCellChecked == true)
                {
                    countOfSelectedPersonels++;
                }
            }
            return countOfSelectedPersonels;
        }

        public int indexOfSelectedRow()
        {
            int i;
            for (i = 0; i < dataGridView1.Rows.Count; i++)
            {
                bool isCellChecked = Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value);
                if (isCellChecked == true)
                {
                    break;
                }
            }
            return i;
        }

        // seçimleri sıfırla butonu
        public void resetChoices()
        {
            button5.Enabled = false;
            button6.Enabled = false;
            list2();
            clearTextBoxes();

        }

        public void selectAll()
        {
            if (updateOperationIsInProcess())
            {
                DialogResult dr = MessageBox.Show("Güncelleme işlemi iptal edilecektir. Onaylıyor musunuz?", "",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                {
                    return;
                }

                else if (dr == DialogResult.Yes)
                {
                    clearTextBoxes();
                    button5.Enabled = false;
                    enableAllCheckBoxes();
                }
            }
   
            for (int i = 0; i < dataGridView1.Rows.Count ; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = true;
            }
        }


        // db'den tüm attributeları çekiyor
        public string[] getValues()
        {
            List<string> valueList = new List<string>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    valueList.Add(Convert.ToString(cell.Value));
                }
            }
            return valueList.ToArray();
        }

        // tüm alanlar dolduduldu mu kontrol ediyor
        public Boolean allTextBoxesAreFilled()
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox5.Text == "" || textBox6.Text == "" ||
                textBox7.Text == "" || (radioButton1.Checked == false && radioButton2.Checked == false))
            {

                return false;
            }
            return true;
        }


        public void disableAllCheckBoxes()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].ReadOnly = true;
            }

        }


        public void enableAllCheckBoxes()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].ReadOnly = false;
            }

        }

        public Boolean updateOperationIsInProcess()
        {
            if (button5.Enabled == true) return true;
            return false;
        }

        // tüm personel listele butonu
        private void button1_Click(object sender, EventArgs e)
        {
            list2();
            button7.Enabled = true;
        }

        // personel ekle butonu
        private void button2_Click(object sender, EventArgs e)
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();
            string tcKimlik = textBox1.Text;
            string ad = textBox2.Text;
            string soyad = textBox3.Text;
            string dTarihi = dateTimePicker1.Value.ToString("dd-MM-yyyy");
            string pozisyon = textBox5.Text;
            string telefon = textBox6.Text;
            string ePosta = textBox7.Text;
            string cinsiyet = "";
            if (radioButton1.Checked == true) cinsiyet = radioButton1.Text;
            else if (radioButton2.Checked == true) cinsiyet = radioButton2.Text;

            if (updateOperationIsInProcess())
            {
                DialogResult dr = MessageBox.Show("Güncelleme işlemi iptal edilecektir. Onaylıyor musunuz?", "",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                    return;
                else if (dr == DialogResult.Yes)
                {
                    button5.Enabled = false;
                    enableAllCheckBoxes();
                }
            }
           
            long number;
            if (!allTextBoxesAreFilled())
                MessageBox.Show("Tüm alanlar doldurulmalıdır.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!long.TryParse(tcKimlik, out number))
            {
                MessageBox.Show("Geçersiz bir TC Kimlik No girdiniz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (!long.TryParse(telefon, out number))
            {
                MessageBox.Show("Geçersiz bir telefon numarası girdiniz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (tcKimlik.Length < 11)
            {
                MessageBox.Show("TC Kimlik No 11 haneden az olamaz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (tcKimlik.Length > 11)
            {
                MessageBox.Show("TC Kimlik No 11 haneden fazla olamaz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (telefon.Length < 11)
            {
                MessageBox.Show("Telefon numarası 11 haneden az olamaz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (telefon.Length > 11)
            {
                MessageBox.Show("Telefon numarası 11 haneden fazla olamaz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (getValues().Contains(tcKimlik))
            {
                MessageBox.Show("Aynı TC Kimlik Numarasına sahip başka bir personel eklenemez.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else
            {
                SqlCommand cmd = new SqlCommand("Insert Into PERSONEL([TC Kimlik No],[Ad],[Soyad],[Doğum Tarihi]," +
                    "[Pozisyon],[Telefon],[E-posta],[Cinsiyet]) " +
                    "Values(@tckn,@ad,@soyad,@dt,@poz,@tel,@ePosta,@cins)", baglanti);

                cmd.Parameters.Add("@tckn", SqlDbType.VarChar, 50).Value = tcKimlik;
                cmd.Parameters.Add("@ad", SqlDbType.VarChar, 50).Value = ad;
                cmd.Parameters.Add("@soyad", SqlDbType.VarChar, 50).Value = soyad;
                cmd.Parameters.Add("@dt", SqlDbType.VarChar, 50).Value = dTarihi;
                cmd.Parameters.Add("@poz", SqlDbType.VarChar, 50).Value = pozisyon;
                cmd.Parameters.Add("@tel", SqlDbType.VarChar, 50).Value = telefon;
                cmd.Parameters.Add("@ePosta", SqlDbType.VarChar, 50).Value = ePosta;
                cmd.Parameters.Add("@cins", SqlDbType.VarChar, 50).Value = cinsiyet;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                MessageBox.Show("Personel başarıyla eklendi.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                resetChoices();
            }

            baglanti.Close();
        }

        // personel sil butonu
        private void button3_Click(object sender, EventArgs e)
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();

            if (updateOperationIsInProcess())
            {

                DialogResult dr = MessageBox.Show("Güncelleme işlemi iptal edilecektir. Onaylıyor musunuz?", "",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.No)
                {
                    return;
                }
                else if (dr == DialogResult.Yes)
                {
                    clearTextBoxes();
                    button5.Enabled = false;
                    enableAllCheckBoxes();
                }

            }
      
            if (numberOfSelectedRows() == 0)
                MessageBox.Show("Lütfen personel seçimi yapınız.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                DialogResult dr2 = MessageBox.Show(numberOfSelectedRows() + " personeli silmek istediğinize emin misiniz?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr2 == DialogResult.Yes)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        bool isCellChecked = Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value);
                        if (isCellChecked == true)
                        {
                            SqlCommand cmd = new SqlCommand("Delete From PERSONEL Where [TC Kimlik No]=@item", baglanti);
                            cmd.Parameters.AddWithValue("@item", dataGridView1.Rows[i].Cells[1].Value.ToString());
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show(numberOfSelectedRows() + " personel başarıyla silindi.", "", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    resetChoices();
                }
            }

            baglanti.Close();
        }

        // personel bilgisi getir butonu
        private void button4_Click(object sender, EventArgs e)
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();

            if (numberOfSelectedRows() > 1)
                MessageBox.Show("Aynı anda birden fazla personel bilgisi güncellenemez.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (numberOfSelectedRows() == 0)
                MessageBox.Show("Lütfen personel seçimi yapınız.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                textBox1.Text = Convert.ToString(dataGridView1.Rows[indexOfSelectedRow()].Cells[1].Value);
                textBox2.Text = Convert.ToString(dataGridView1.Rows[indexOfSelectedRow()].Cells[2].Value);
                textBox3.Text = Convert.ToString(dataGridView1.Rows[indexOfSelectedRow()].Cells[3].Value);
                dateTimePicker1.Text = Convert.ToString(dataGridView1.Rows[indexOfSelectedRow()].Cells[4].Value);
                textBox5.Text = Convert.ToString(dataGridView1.Rows[indexOfSelectedRow()].Cells[5].Value);
                textBox6.Text = Convert.ToString(dataGridView1.Rows[indexOfSelectedRow()].Cells[6].Value);
                textBox7.Text = Convert.ToString(dataGridView1.Rows[indexOfSelectedRow()].Cells[7].Value);
                if (Convert.ToString(dataGridView1.Rows[indexOfSelectedRow()].Cells[8].Value) == "Kadın")
                    radioButton1.Checked = true;
                else if (Convert.ToString(dataGridView1.Rows[indexOfSelectedRow()].Cells[8].Value) == "Erkek")
                    radioButton2.Checked = true;
                button5.Enabled = true;
                disableAllCheckBoxes();
            }
            baglanti.Close();
        }



        // kaydet butonu
        private void button5_Click(object sender, EventArgs e)
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();

            string tcKimlik = textBox1.Text;
            string ad = textBox2.Text;
            string soyad = textBox3.Text;
            string dTarihi = dateTimePicker1.Value.ToString("dd-MM-yyyy");
            string pozisyon = textBox5.Text;
            string telefon = textBox6.Text;
            string ePosta = textBox7.Text;
            string cinsiyet = "";
            if (radioButton1.Checked == true) cinsiyet = radioButton1.Text;
            else if (radioButton2.Checked == true) cinsiyet = radioButton2.Text;

            long number;
            string PKOfSelectedRow = Convert.ToString(dataGridView1.Rows[indexOfSelectedRow()].Cells[1].Value);

            if (!allTextBoxesAreFilled())
                MessageBox.Show("Tüm alanlar doldurulmalıdır.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!long.TryParse(tcKimlik, out number))
            {
                MessageBox.Show("Geçersiz bir TC Kimlik No girdiniz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (!long.TryParse(telefon, out number))
            {
                MessageBox.Show("Geçersiz bir telefon numarası girdiniz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (tcKimlik.Length < 11)
            {
                MessageBox.Show("TC Kimlik No 11 haneden az olamaz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (tcKimlik.Length > 11)
            {
                MessageBox.Show("TC Kimlik No 11 haneden fazla olamaz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (telefon.Length < 11)
            {
                MessageBox.Show("Telefon numarası 11 haneden az olamaz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (telefon.Length > 11)
            {
                MessageBox.Show("Telefon numarası 11 haneden fazla olamaz.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (getValues().Contains(tcKimlik) && !tcKimlik.Equals(PKOfSelectedRow))
            {
                MessageBox.Show("Aynı TC Kimlik Numarasına sahip başka bir personel eklenemez.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else

            {
                DialogResult dr = MessageBox.Show("Personel bilgisini güncellemek istediğinize emin misiniz?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {

                    SqlCommand cmd = new SqlCommand("Update PERSONEL Set [TC Kimlik No]=@tckn,[Ad]=@ad,[Soyad]=@soyad," +
                    "[Doğum Tarihi]=@dt,[Pozisyon]=@poz,[Telefon]=@tel,[E-posta]=@ePosta,[Cinsiyet]=@cins " +
                    "Where [TC Kimlik No]=@item", baglanti);

                    cmd.Parameters.AddWithValue("@item", Convert.ToString(PKOfSelectedRow));
                    cmd.Parameters.Add("@tckn", SqlDbType.VarChar, 50).Value = tcKimlik;
                    cmd.Parameters.Add("@ad", SqlDbType.VarChar, 50).Value = ad;
                    cmd.Parameters.Add("@soyad", SqlDbType.VarChar, 50).Value = soyad;
                    cmd.Parameters.Add("@dt", SqlDbType.VarChar, 50).Value = dTarihi;
                    cmd.Parameters.Add("@poz", SqlDbType.VarChar, 50).Value = pozisyon;
                    cmd.Parameters.Add("@tel", SqlDbType.VarChar, 50).Value = telefon;
                    cmd.Parameters.Add("@ePosta", SqlDbType.VarChar, 50).Value = ePosta;
                    cmd.Parameters.Add("@cins", SqlDbType.VarChar, 50).Value = cinsiyet;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Personel bilgisi başarıyla güncellendi.", "", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    resetChoices();
                }
            }

            baglanti.Close();
        }

        // seçimleri sıfırla butonu
        private void button6_Click(object sender, EventArgs e)
        {
            resetChoices();
        }

        // tümünü seç butonu
        private void button7_Click(object sender, EventArgs e)
        {
            button6.Enabled = true;
            selectAll();
        }

        // personele kart ata butonu
        private void button8_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.ShowDialog(); // Shows Form2
        }

        // bir tane bile checkbox checklenirse seçimleri sıfırla butonu aktif oluyor
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            bool flag = false;
            var senderGrid = (DataGridView)sender;
            senderGrid.EndEdit();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (e.RowIndex >= 0)
                {
                    var cbxCell = (DataGridViewCheckBoxCell)senderGrid.Rows[i].Cells[0];

                    if (Convert.ToBoolean(cbxCell.Value))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            button6.Enabled = flag;
        }


    }
}
