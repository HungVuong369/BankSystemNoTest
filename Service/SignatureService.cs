using BankSystem.Repository;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Syncfusion.Pdf.Parsing;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf.Graphics;

namespace BankSystem.Service
{
    public class SignatureService : ISignatureService
    {
        public MemoryStream Signature(byte[] existingPdfStream, string passwordPdf = null)
        {
            PdfLoadedDocument loadedDocument;
            if(passwordPdf == null)
                loadedDocument = new PdfLoadedDocument(existingPdfStream);
            else
                loadedDocument = new PdfLoadedDocument(existingPdfStream, passwordPdf);

            PdfSignature signature = new PdfSignature(loadedDocument, loadedDocument.Pages[0], null, "Signed");

            signature.Bounds = new RectangleF(new PointF(0, 0), new SizeF(40, 40));
            FileStream imageStream = new FileStream("Assets/Img/MicrosoftTeams-image.png", FileMode.Open, FileAccess.Read);
            PdfBitmap signatureImage = new PdfBitmap(imageStream);
            signature.Appearance.Normal.Graphics.DrawImage(signatureImage, new RectangleF(0, 0, 40, 40));

            signature.ContactInfo = "VuongNH12@fpt.com";
            signature.LocationInfo = "Phước Hải, Nha Trang";
            signature.Reason = "I have signature this pdf";
            string filePath = Path.Combine("Assets/sautinsoft.pfx");
            X509Certificate2 digitalId = new X509Certificate2(filePath, "123456789");

            X509Chain chain = new X509Chain();
            chain.Build(digitalId);
            List<X509Certificate2> certificates = new List<X509Certificate2>();
            for (int i = 0; i < chain.ChainElements.Count; i++)
            {
                certificates.Add(chain.ChainElements[i].Certificate);
            }

            signature.EnableLtv = true;
            signature.AddExternalSigner(new ExternalSigner(digitalId), certificates, null);
            signature.TimeStampServer = new TimeStampServer(new Uri("http://timestamp.entrust.net/TSS/RFC3161sha2TS"));
            signature.EnableLtv = true;
            signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
            signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA256;

            MemoryStream signedMemoryStream = new MemoryStream();
            loadedDocument.Save(signedMemoryStream);
            signedMemoryStream.Position = 0;

            return signedMemoryStream;
        }
    }

    internal class ExternalSigner : IPdfExternalSigner
    {
        public string HashAlgorithm => "SHA256";

        X509Certificate2 digitalID;

        public ExternalSigner(X509Certificate2 certificate)
        {
            digitalID = certificate;
        }

        public byte[] Sign(byte[] message, out byte[] timeStampResponse)
        {
            byte[] signedBytes = null;

            //This condition is based on the provided certificate private key

            if (digitalID.PrivateKey is RSACng)

            {
                Console.WriteLine("RSACng");

                RSACng rsa = (RSACng)digitalID.PrivateKey;

                signedBytes = rsa.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            else if (digitalID.PrivateKey is RSACryptoServiceProvider)
            {

                Console.WriteLine("RSACryptoServiceProvider");

                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)digitalID.PrivateKey;

                signedBytes = rsa.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            }
            else if (digitalID.PrivateKey is RSAOpenSsl)
            {
                Console.WriteLine("RSAOpenSsl");
                RSAOpenSsl rsa = (RSAOpenSsl)digitalID.PrivateKey;
                signedBytes = rsa.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            timeStampResponse = null;
            return signedBytes;
        }
    }
}
