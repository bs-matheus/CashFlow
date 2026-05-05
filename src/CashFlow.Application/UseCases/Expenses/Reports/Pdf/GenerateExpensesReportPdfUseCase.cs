using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Colors;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Fonts;
using CashFlow.Domain.Extensions;
using CashFlow.Domain.Reports.Messages;
using CashFlow.Domain.Repositories.Expenses;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System.Reflection;

namespace CashFlow.Application.UseCases.Expenses.Reports.Pdf;

internal class GenerateExpensesReportPdfUseCase : IGenerateExpensesReportPdfUseCase
{
    private const string CURRENCY_SYMBOL = "R$";
    private const string USER_NAME = "Matheus Barbosa";

    private const int EXPENSE_TABLE_ROW_HEIGHT = 25;

    private readonly IExpensesReadOnlyRepository _repository;

    public GenerateExpensesReportPdfUseCase(IExpensesReadOnlyRepository repository)
    {
        _repository = repository;

        GlobalFontSettings.FontResolver = new ExpensesReportFontResolver();
    }

    public async Task<byte[]> ExecuteAsync(DateOnly month)
    {
        var expenses = await _repository.FilterByMonthAsync(month);
        if (expenses.Count == 0)
            return [];

        var document = CreateDocument(month);
        var page = CreateSection(document);

        CreateHeaderWithUserInformation(page);

        decimal totalExpenses = expenses.Sum(expense => expense.Amount);
        CreateTotalSpentSection(page, month, totalExpenses);

        foreach (var expense in expenses)
        {
            var table = CreateExpenseTable(page);

            var row = table.AddRow();
            row.Height = EXPENSE_TABLE_ROW_HEIGHT;

            AddExpenseTitle(row.Cells[0], expense.Title);
            AddAmountHeader(row.Cells[3]);

            row = table.AddRow();
            row.Height = EXPENSE_TABLE_ROW_HEIGHT;

            row.Cells[0].AddParagraph(expense.Date.ToString("D"));
            SetBaseStyleForExpenseInformation(row.Cells[0]);
            row.Cells[0].Format.LeftIndent = 20;

            row.Cells[1].AddParagraph(expense.Date.ToString("t"));
            SetBaseStyleForExpenseInformation(row.Cells[1]);

            row.Cells[2].AddParagraph(expense.PaymentType.PaymentTypeToText());
            SetBaseStyleForExpenseInformation(row.Cells[2]);

            AddExpenseAmount(row.Cells[3], expense.Amount);

            AddDescriptionRowIfNotEmpty(table, row.Cells[3], expense.Description);

            AddWhiteSpaceRow(table);
        }

        return RenderDocument(document);
    }

    private Document CreateDocument(DateOnly month)
    {
        var document = new Document();
        document.Info.Title = string.Format(ResourceReportGenerationMessages.EXPENSES_FOR, month.ToString("Y"));
        document.Info.Author = USER_NAME;

        var style = document.Styles.Normal; // Default document paragraph style
        style.Font.Name = FontHelper.RALEWAY_REGULAR;
        style.Font.Color = ColorHelper.BLACK;

        return document;
    }

    private Section CreateSection(Document document)
    {
        var section = document.AddSection();
        section.PageSetup = document.DefaultPageSetup.Clone();

        section.PageSetup.PageFormat = PageFormat.A4;

        section.PageSetup.LeftMargin = 40;
        section.PageSetup.RightMargin = 40;
        section.PageSetup.TopMargin = 80;
        section.PageSetup.BottomMargin = 80;

        return section;
    }

    private void CreateHeaderWithUserInformation(Section page)
    {
        var table = page.AddTable();
        table.AddColumn();
        table.AddColumn(300);

        var row = table.AddRow();

        var assembly = Assembly.GetExecutingAssembly();
        string dirName = Path.GetDirectoryName(assembly.Location)!;
        string filePath = Path.Combine(dirName, "Logo", "EmptyAvatar.png");
        row.Cells[0].AddImage(filePath);

        row.Cells[1].AddParagraph(string.Format(ResourceReportGenerationMessages.USER_GREETING, USER_NAME));
        row.Cells[1].VerticalAlignment = VerticalAlignment.Center;
        row.Cells[1].Format.Font = new Font
        {
            Name = FontHelper.RALEWAY_BLACK,
            Size = 16
        };
    }

    private void CreateTotalSpentSection(Section page, DateOnly month, decimal totalExpenses)
    {
        var paragraph = page.AddParagraph();
        paragraph.Format.SpaceBefore = 40;
        paragraph.Format.SpaceAfter = 40;

        string paragraphTitle = string.Format(ResourceReportGenerationMessages.TOTAL_SPENT_IN, month.ToString("Y"));

        paragraph.AddFormattedText(paragraphTitle, new Font
        {
            Name = FontHelper.RALEWAY_REGULAR,
            Size = 15
        });
        paragraph.AddLineBreak();

        paragraph.AddFormattedText($"{CURRENCY_SYMBOL} {totalExpenses}", new Font
        {
            Name = FontHelper.WORKSANS_BLACK,
            Size = 50
        });
    }

    private Table CreateExpenseTable(Section page)
    {
        var table = page.AddTable();

        table.AddColumn(195).Format.Alignment = ParagraphAlignment.Left;
        table.AddColumn(80).Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn(120).Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn(120).Format.Alignment = ParagraphAlignment.Right;

        return table;
    }

    private void AddExpenseTitle(Cell cell, string expenseTitle)
    {
        cell.AddParagraph(expenseTitle);
        cell.MergeRight = 2;
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Shading.Color = ColorHelper.RED_LIGHT;
        cell.Format.LeftIndent = 20;
        cell.Format.Font = new Font
        {
            Name = FontHelper.RALEWAY_BLACK,
            Size = 14
        };
    }

    private void AddAmountHeader(Cell cell)
    {
        cell.AddParagraph(ResourceReportGenerationMessages.AMOUNT);
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Shading.Color = ColorHelper.RED_DARK;
        cell.Format.Font = new Font
        {
            Name = FontHelper.RALEWAY_BLACK,
            Size = 14,
            Color = ColorHelper.WHITE
        };
    }

    private void SetBaseStyleForExpenseInformation(Cell cell)
    {
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Shading.Color = ColorHelper.GREEN_DARK;
        cell.Format.Font = new Font
        {
            Name = FontHelper.WORKSANS_REGULAR,
            Size = 12
        };
    }

    private void AddExpenseAmount(Cell cell, decimal expenseAmount)
    {
        cell.AddParagraph($"-{CURRENCY_SYMBOL} {expenseAmount}");
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Shading.Color = ColorHelper.WHITE;
        cell.Format.Font = new Font
        {
            Name = FontHelper.WORKSANS_REGULAR,
            Size = 14
        };
    }

    private void AddWhiteSpaceRow(Table table)
    {
        var row = table.AddRow();
        row.Height = 30;
        row.Borders.Visible = false;
    }

    private void AddDescriptionRowIfNotEmpty(Table table, Cell lastCell, string? expenseDescription)
    {
        if (!string.IsNullOrWhiteSpace(expenseDescription))
        {
            var descriptionRow = table.AddRow();
            descriptionRow.Height = EXPENSE_TABLE_ROW_HEIGHT;

            descriptionRow.Cells[0].AddParagraph(expenseDescription);
            descriptionRow.Cells[0].MergeRight = 2;
            descriptionRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            descriptionRow.Cells[0].Shading.Color = ColorHelper.GREEN_LIGHT;
            descriptionRow.Cells[0].Format.LeftIndent = 20;
            descriptionRow.Cells[0].Format.Font = new Font
            {
                Name = FontHelper.WORKSANS_REGULAR,
                Size = 10
            };

            lastCell.MergeDown = 1;
        }
    }

    private byte[] RenderDocument(Document document)
    {
        var renderer = new PdfDocumentRenderer
        {
            Document = document
        };

        renderer.RenderDocument();

        using var file = new MemoryStream();
        renderer.PdfDocument.Save(file);

        return file.ToArray();
    }
}
