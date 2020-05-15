using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace CryptIt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        byte[] saltbytes = Encoding.Unicode.GetBytes("cryptthisfamoust");
        string content,path;

        public string Encrypt(string text, string secretKey)
        {
            byte[] textbytes = Encoding.Unicode.GetBytes(text);
            
            using (Aes Encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pw = new Rfc2898DeriveBytes(secretKey, saltbytes);
                Encryptor.Key = pw.GetBytes(32);
                Encryptor.IV = pw.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, Encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(textbytes, 0, textbytes.Length);
                    }
                    text = Convert.ToBase64String(ms.ToArray());
                }
            }
            return text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            path = openFileDialog1.FileName;
            content = File.ReadAllText(path);

        }
        
        public string Decrypt(string ciphertext, string secretKey)
        {
            byte[] cipherBytes = Convert.FromBase64String(ciphertext);
            using (Aes decryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pw = new Rfc2898DeriveBytes(secretKey, saltbytes);
                decryptor.Key = pw.GetBytes(32);
                decryptor.IV = pw.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                    }
                    ciphertext = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return ciphertext;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(content))
                MessageBox.Show("Select a file!");
            else
            {
                if (textBox1.Text == "")
                    MessageBox.Show("Add a key!");
                else
                {
                    content=Encrypt(content, textBox1.Text);
                    File.WriteAllText(path+".crypted", content);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(content))
                MessageBox.Show("Select a file!");
            else
            {
                if (textBox1.Text == "")
                    MessageBox.Show("Add a key!");
                else
                {
                    content = Decrypt(content, textBox1.Text);
                    File.WriteAllText(path + ".decrypted", content);
                }
            }
        }
    }
}
