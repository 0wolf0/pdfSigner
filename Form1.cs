using System;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using System.Windows.Forms;

namespace pdfSigner
{
    public partial class Form1 : Form
    {
        private Pkcs12Store rightPfxKeyStore;
        private Pkcs12Store leftPfxKeyStore;
        private PdfReader pdfReader;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //LOGIC FOR SIGNING
            // open pdf
            //open signature
            //call signPDF() function
            
        }

        private void openSignature()
        {
        }

        private void signPDF()
        {
            
            // Step 4: Initialize the PDF Stamper And Creating the Signature Appearance
            PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, new FileStream("C:\\Users\\Admin\\Documents\\MyPDF_Signed.pdf", FileMode.Create), '\0', null, true);
            PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
            signatureAppearance.Reason = "Digital Signature Reason";
            signatureAppearance.Location = "Digital Signature Location";

            // Set the signature appearance location (in points)
            float x = 360;
            float y = 130;
            signatureAppearance.Acro6Layers = false;
            signatureAppearance.Layer4Text = PdfSignatureAppearance.questionMark;
            signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(x, y, x + 150, y + 50), 1, "signature");

            // Step 5: Sign the Document
            string aliasRight = rightPfxKeyStore.Aliases.Cast<string>().FirstOrDefault(entryAlias => rightPfxKeyStore.IsKeyEntry(entryAlias));
            string aliasLeft = rightPfxKeyStore.Aliases.Cast<string>().FirstOrDefault(entryAlias => rightPfxKeyStore.IsKeyEntry(entryAlias));

            if (aliasRight != null)
            {
                ICipherParameters privateKey = rightPfxKeyStore.GetKey(aliasRight).Key;
                IExternalSignature pks = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);
                MakeSignature.SignDetached(signatureAppearance, pks, new Org.BouncyCastle.X509.X509Certificate[] { rightPfxKeyStore.GetCertificate(aliasRight).Certificate }, null, null, null, 0, CryptoStandard.CMS);
            }
            else
            {
                Console.WriteLine("Private key not found in the right side PFX certificate.");
            }

            if (aliasLeft != null)
            {
                ICipherParameters privateKey = rightPfxKeyStore.GetKey(aliasRight).Key;
                IExternalSignature pks = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);
                MakeSignature.SignDetached(signatureAppearance, pks, new Org.BouncyCastle.X509.X509Certificate[] { rightPfxKeyStore.GetCertificate(aliasRight).Certificate }, null, null, null, 0, CryptoStandard.CMS);
            }
            else
            {
                Console.WriteLine("Private key not found in the left side PFX certificate.");
            }

            // Step 6: Save the Signed PDF
            pdfStamper.Close();

            Console.WriteLine("PDF signed successfully!");
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            //LEFT THE CODE FOR FUTURE MULTIPLE FILE OPEN
            //using (var fbd = new FolderBrowserDialog())
            //{
            //    DialogResult result = fbd.ShowDialog();

            //    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            //    {
            //        string pdfFilePath = fbd.SelectedPath;
            //        textBox1.Text = pdfFilePath;
            //        pdfReader = new PdfReader(pdfFilePath);
            //    }
            //}

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "PDF files (*.PDF)|*.pdf|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pdfFilePath = openFileDialog.FileName;
                    textBox1.Text = pdfFilePath;
                    //MUST DO A LOOP FOR READING ALL FILES, NOT JUST ONE
                    //pdfReader = new PdfReader(pdfFilePath);
                }
            }
        }

        private void textBox2_DoubleClick(object sender, EventArgs e)//' RIGHT SIDE SIGNATURE
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "pfx files (*.pfx)|*.pfx|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pfxFilePath = openFileDialog.FileName;
                    string pfxPassword = "";// IF THERE IS A PASSWORD, PLACE IT HERE
                    rightPfxKeyStore = new Pkcs12Store(new FileStream(pfxFilePath, FileMode.Open, FileAccess.Read), pfxPassword.ToCharArray());
                }
            }
        }

        private void textBox3_DoubleClick(object sender, EventArgs e)//' LEFT SIDE SIGNATURE
        {

            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "pfx files (*.pfx)|*.pfx|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    string pfxFilePath = openFileDialog.FileName;
                    string pfxPassword = "";
                    leftPfxKeyStore = new Pkcs12Store(new FileStream(pfxFilePath, FileMode.Open, FileAccess.Read), pfxPassword.ToCharArray());
                }
            }

        }
    }
}
