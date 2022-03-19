using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using NPOI.XWPF.UserModel;
using ProtectionOfInfo.WebApp.Data.Entities.CryptographyEntities;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToWordService
{

    public class ConvertToWord : IConvertToWord
    {
        private readonly IWebHostEnvironment _env;

        private readonly string templateName = "Temp.docx";
        private readonly string templatePath;

        private readonly string fileNameDOCX;
        private readonly string filePathDOCX;

        public ConvertToWord(IWebHostEnvironment env)
        {
            _env = env;
            fileNameDOCX = $"{Guid.NewGuid()}.docx";

            templatePath = Path.Combine(_env.ContentRootPath, "wwwroot", "documents", templateName);
            filePathDOCX = Path.Combine(_env.ContentRootPath, "wwwroot", "documents", fileNameDOCX);
        }

        public async Task<MyFileInfo?> CreateDocumentAsync(Check contract)
        {
            try
            {
                XWPFDocument doc;
                using (var rs = File.OpenRead(templatePath))
                {
                    doc = new XWPFDocument(rs);
                }

                foreach (var para in doc.Paragraphs)
                {
                    if (para.ParagraphText.Contains("{Header}"))
                    {
                        para.ReplaceText("{Header}", $"{contract.Type}{contract.Number} от {contract.Date.ToString("dd.MM.yyyy")}");
                        continue;
                    }
                    if (para.ParagraphText.Contains("{ResultInfoCheck}"))
                    {
                        para.ReplaceText("{ResultInfoCheck}", contract.ResultInfoCheck);
                        continue;
                    }
                }

                var tableWithDetails = doc.Tables[0];
                foreach(var row in tableWithDetails.Rows)
                {
                    var cell = row.GetCell(1);
                    foreach (var para in cell.Paragraphs)
                    {
                        if (para.ParagraphText.Contains("{Supplier}"))
                        {
                            para.ReplaceText("{Supplier}", contract.Supplier);
                            continue;
                        }
                        if (para.ParagraphText.Contains("{SupplierDetails}"))
                        {
                            para.ReplaceText("{SupplierDetails}", contract.SupplierDetails);
                            continue;
                        }
                        if (para.ParagraphText.Contains("{Сustomer}"))
                        {
                            para.ReplaceText("{Сustomer}", contract.Сustomer);
                            continue;
                        }
                        if (para.ParagraphText.Contains("{DeliveryAddress}"))
                        {
                            para.ReplaceText("{DeliveryAddress}", contract.DeliveryAddress);
                            continue;
                        }
                        if (para.ParagraphText.Contains("{DeliveryType}"))
                        {
                            para.ReplaceText("{DeliveryType}", contract.DeliveryType);
                            continue;
                        }
                        if (para.ParagraphText.Contains("{PaymentInformation}"))
                        {
                            para.ReplaceText("{PaymentInformation}", contract.PaymentInformation);
                            continue;
                        }
                    }
                }

                var tableWithProducts = doc.Tables[1];
                var tmpRow = new XWPFTableRow(tableWithProducts.Rows[1].GetCTRow(), tableWithProducts);
                for(int i = 0; i < contract.Products.Count; i++)
                {
                    var product = contract.Products[i];
                    var row = new XWPFTableRow(tmpRow.GetCTRow().Copy(), tableWithProducts);
                    
                    foreach (var cell in row.GetTableCells())
                    {
                        foreach(var para in cell.Paragraphs)
                        {
                            if (para.ParagraphText.Contains("{1}"))
                            {
                                para.ReplaceText("{1}", (i + 1).ToString());
                                continue;
                            }
                            if (para.ParagraphText.Contains("{2}"))
                            {
                                para.ReplaceText("{2}", product.VendorCode);
                                continue;
                            }
                            if (para.ParagraphText.Contains("{3}"))
                            {
                                para.ReplaceText("{3}", product.Name);
                                continue;
                            }
                            if (para.ParagraphText.Contains("{4}"))
                            {
                                para.ReplaceText("{4}", product.Amount.ToString());
                                continue;
                            }
                            if (para.ParagraphText.Contains("{5}"))
                            {
                                para.ReplaceText("{5}", product.Unit);
                                continue;
                            }
                            if (para.ParagraphText.Contains("{6}"))
                            {
                                para.ReplaceText("{6}", product.Price.ToString("C"));
                                continue;
                            }
                            if (para.ParagraphText.Contains("{7}"))
                            {
                                para.ReplaceText("{7}", product.SummaWithOutDiscount.ToString("C"));
                                continue;
                            }
                            if (para.ParagraphText.Contains("{8}"))
                            {
                                para.ReplaceText("{8}", product.Discount.ToString());
                                continue;
                            }
                            if (para.ParagraphText.Contains("{9}"))
                            {
                                para.ReplaceText("{9}", product.Total.ToString("C"));
                                continue;
                            }
                        }
                    }
                    tableWithProducts.AddRow(row, i + 1);
                }

                int rowsCount = tableWithProducts.Rows.Count;
                tableWithProducts.RemoveRow(rowsCount - 3);

                rowsCount = tableWithProducts.Rows.Count;
                var totalRow = tableWithProducts.Rows[rowsCount - 2];
                var totalCell = totalRow.GetCell(1);
                foreach (var par in totalCell.Paragraphs)
                {
                    if (par.ParagraphText.Contains("{Total}"))
                    {
                        par.ReplaceText("{Total}", contract.Total.ToString("C"));
                    }
                }

                var VATROW = tableWithProducts.Rows[rowsCount - 1];
                var VATCell = VATROW.GetCell(1);
                foreach (var par in VATCell.Paragraphs)
                {
                    if (par.ParagraphText.Contains("{VAT}"))
                    {
                        par.ReplaceText("{VAT}", contract.VAT.ToString("C"));
                    }
                }

                using (var ws = File.Create(filePathDOCX))
                {
                    doc.Write(ws);
                }
                doc.Close();
               
                var file = await File.ReadAllBytesAsync(filePathDOCX);
                File.Delete(filePathDOCX);

                return new MyFileInfo(file, "Receipt.docx", "docx");
            }
            catch
            {
                if (File.Exists(filePathDOCX)) File.Delete(filePathDOCX);
                return null;
            }
        }
    }
}
