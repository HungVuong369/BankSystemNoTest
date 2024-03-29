using BankSystem.Models;
using BankSystem.Repository;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Signatures;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static iText.Signatures.PdfSigner;

namespace BankSystem.Service
{
    public class PdfService : IPdfService
    {
        private ISignatureService _signatureService;

        public PdfService(ISignatureService signatureService)
        {
            _signatureService = signatureService;
        }

        public byte[] GeneratePdf(List<Transaction> transactions)
        {
            byte[] pdfBytes;
            string userPassword = "123321";
            string ownerPassword = "321123";

            using (MemoryStream ms = new())
            {
                // Set encryption properties
                WriterProperties writerProperties = new WriterProperties()
                    .SetStandardEncryption(
                        Encoding.UTF8.GetBytes(userPassword),
                        Encoding.UTF8.GetBytes(ownerPassword),
                        EncryptionConstants.ALLOW_PRINTING,
                        EncryptionConstants.ENCRYPTION_AES_128 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA);

                // Create PDF document
                PdfWriter writer = new PdfWriter(ms, writerProperties);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Add title
                document.Add(new Paragraph("Transaction Report"));

                // Create a table with 7 columns
                Table table = new Table(7);

                // Add headers
                table.AddHeaderCell("Transaction ID");
                table.AddHeaderCell("Account No");
                table.AddHeaderCell("Direction");
                table.AddHeaderCell("Amount");
                table.AddHeaderCell("Status");
                table.AddHeaderCell("Description");
                table.AddHeaderCell("Transaction Date");

                // Add transaction data to the table
                foreach (var transaction in transactions)
                {
                    table.AddCell(transaction.TransactionId);
                    table.AddCell(transaction.AccountNo);
                    table.AddCell(transaction.Direction.ToString());
                    table.AddCell(transaction.Amount.ToString("N0") + " VND");
                    table.AddCell(transaction.Status.ToString());
                    table.AddCell(transaction.Description);
                    table.AddCell(transaction.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                // Add the table to the document
                document.Add(table);

                // Close document
                document.Close();
                pdf.Close();
                writer.Close();

                // Get PDF bytes
                
                var newMs = _signatureService.Signature(ms.ToArray(), ownerPassword);
                pdfBytes = newMs.ToArray();
            }
            return pdfBytes;
        }

    }
}
