
using AutoMapper;
using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace GoLoan.Infrastructure.Services
{
    public class sanctionletterservice : Isanctionletter
    {
        AppDbContext db;
        IMapper mapper;

        public sanctionletterservice(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public int generatesanction(int loandealid)
        {
            var loan = db.LoanDeals.FirstOrDefault(x => x.DealId == loandealid);
            if (loan == null || loan.CurrentStatus != "Approved")
            {
                return 0;
            }

            if (loan == null)
            {
                return 0;
            }

            double rate;

            if (loan.TenureMonths == 36)
            {
                rate = 11.5;
            }
            else if (loan.TenureMonths == 60 ||
                     loan.TenureMonths == 120)
            {
                rate = 11;
            }
            else
            {
                return 0;
            }

            decimal loanamount = loan.LoanAmount;

            decimal totalinterest = loanamount * (decimal)rate* loan.TenureMonths / (100 * 12);

            decimal emi = (loanamount + totalinterest)/ loan.TenureMonths;

            var data = new SanctionLetterDTO
            {
                DealId = loan.DealId,
                LoanType=loan.LoanType,
                LoanAmount = loan.LoanAmount,
                InterestRate = rate,
                TenureMonths = loan.TenureMonths,
                EmiAmount = Math.Round(emi, 2),
                CreatedAt = DateTime.Now
            };

            var sanction = mapper.Map<SanctionLetter>(data);
            db.SanctionLetters.Add(sanction);
            db.SaveChanges();
            return sanction.SanctionId;
        }

        public List<SanctionLetterDTO> fetchsanction()
        {
            var data = db.SanctionLetters.ToList();
            var mdata = mapper.Map<List<SanctionLetterDTO>>(data);
            return mdata;
        }

        public SanctionLetterDTO? fetchsanctionbyid(int sanctionid)
        {
            var data = db.SanctionLetters.FirstOrDefault(x => x.SanctionId == sanctionid);

            if (data != null)
            {
                var mdata = mapper.Map<SanctionLetterDTO>(data);
                return mdata;
            }
            return null;
        }




        public byte[]? downloadsanction(int sanctionid)
        {
            var data = db.SanctionLetters.Include(x => x.LoanDeal)
                .FirstOrDefault(x => x.SanctionId == sanctionid);

            if (data == null || data.LoanDeal == null ||
                data.LoanDeal.CurrentStatus != "Approved")
            {
                return null;
            }

            var customer = db.Customers
                .FirstOrDefault(x => x.CustomerId == data.LoanDeal.CustomerId);

            string customername = customer != null
                ? customer.FirstName + " " + customer.LastName
                : "Customer";

            QuestPDF.Settings.License =
                QuestPDF.Infrastructure.LicenseType.Community;

            var pdf = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(QuestPDF.Helpers.PageSizes.A4);
                    page.Margin(45);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(column =>
                    {
                        column.Item().AlignCenter()
                            .Text("GOLOAN")
                            .FontSize(24)
                            .Bold();

                        column.Item().AlignCenter()
                            .Text("LOAN AND FINANCIAL SERVICES")
                            .FontSize(10);

                        column.Item().PaddingTop(10)
                            .LineHorizontal(1);

                        column.Item().PaddingTop(15)
                            .AlignCenter()
                            .Text("LOAN SANCTION LETTER")
                            .FontSize(18)
                            .Bold();
                    });

                    page.Content().PaddingTop(25).Column(column =>
                    {
                        column.Spacing(12);

                        column.Item().AlignRight()
                            .Text("Date: " +
                                data.CreatedAt.ToString("dd-MM-yyyy"));

                        column.Item().Text("Sanction ID: " +
                            data.SanctionId);

                        column.Item().Text("Loan Deal ID: " +
                            data.DealId);

                        column.Item().PaddingTop(10)
                            .Text("To,");

                        column.Item().Text(customername)
                            .Bold()
                            .FontSize(13);

                        column.Item().Text(
                            "Subject: Loan Sanction Approval")
                            .Bold();

                        column.Item().Text("Dear " + customername + ",");

                        column.Item().Text(
                            "We are pleased to inform you that your loan " +
                            "application has been approved. The details " +
                            "of your sanctioned loan are provided below.");

                        column.Item().PaddingTop(10)
                            .Text("LOAN DETAILS")
                            .Bold()
                            .FontSize(13);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell()
                                    .Background("#D9EAF7")
                                    .Padding(8)
                                    .Text("Particulars")
                                    .Bold();

                                header.Cell()
                                    .Background("#D9EAF7")
                                    .Padding(8)
                                    .Text("Details")
                                    .Bold();
                            });

                            table.Cell().BorderBottom(1)
                                .Padding(8).Text("Loan Type");

                            table.Cell().BorderBottom(1)
                                .Padding(8).Text(data.LoanDeal.LoanType);

                            table.Cell().BorderBottom(1)
                                .Padding(8).Text("Loan Amount");

                            table.Cell().BorderBottom(1)
                                .Padding(8).Text(
                                    "INR " + data.LoanAmount.ToString("N2"));

                            table.Cell().BorderBottom(1)
                                .Padding(8).Text("Interest Rate");

                            table.Cell().BorderBottom(1)
                                .Padding(8).Text(
                                    data.InterestRate + "% per annum");

                            table.Cell().BorderBottom(1)
                                .Padding(8).Text("Loan Tenure");

                            table.Cell().BorderBottom(1)
                                .Padding(8).Text(
                                    data.TenureMonths + " months");

                            table.Cell().BorderBottom(1)
                                .Padding(8).Text("Monthly EMI");

                            table.Cell().BorderBottom(1)
                                .Padding(8).Text(
                                    "INR " + data.EmiAmount.ToString("N2"));
                        });

                        column.Item().PaddingTop(15).Text(
                            "Please complete all applicable documentation " +
                            "and formalities before loan disbursement.");

                        column.Item().Text(
                            "Thank you for choosing GoLoan.");

                        column.Item().PaddingTop(25)
                            .Text("Yours sincerely,");

                        column.Item().PaddingTop(25)
                            .Text("Authorized Signatory")
                            .Bold();

                        column.Item().Text("GoLoan");

                        column.Item().PaddingTop(15)
                            .LineHorizontal(1);

                        column.Item().AlignCenter()
                            .Text("This letter is generated electronically.")
                            .FontSize(9);
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("GoLoan | Loan Sanction Letter")
                            .FontSize(9);
                    });
                });
            }).GeneratePdf();

            return pdf;
        }


    }
}