using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UniversityAssestManagement.Models.ViewModels;

namespace UniversityAssetsManagement.Documents
{
    public class ReportsPdfDocument : IDocument
    {
        private readonly ReportsViewModel _model;

        // ── Design tokens ────────────────────────────────────────────
        private static readonly string NavyHex = "#0e1525";
        private static readonly string GoldHex = "#c9a84c";
        private static readonly string TealHex = "#2fb8a0";
        private static readonly string RoseHex = "#e05c6b";
        private static readonly string CreamHex = "#f5f0e8";
        private static readonly string MutedHex = "#7a8a9e";

        public ReportsPdfDocument(ReportsViewModel model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => new DocumentMetadata
        {
            Title = "University Asset Management Report",
            Author = "UAM System",
            Subject = "Asset Report",
        };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        // ── Header ───────────────────────────────────────────────────
        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                // Top stripe
                col.Item().Height(5).Background(NavyHex);

                col.Item().Padding(16).Row(row =>
                {

                    // Logo box
                    row.ConstantItem(60).Height(60)
                       .Background(NavyHex)
                       .AlignCenter().AlignMiddle()
                       .Text("UAM")
                       .FontSize(14).Bold().FontColor(GoldHex);
                    //// Logo box
                    //row.ConstantItem(60).Height(30)
                    //   .Background(NavyHex)
                    //   .Element(container => { 
                    //    container.Image("wwwroot/images/knust_seal.jpg");
                    //   });

                    // .Image("wwwroot/images/knust_seal.jpg");
                    //.Text("UAM")
                    //.FontSize(14).Bold().FontColor(GoldHex);

                    row.ConstantItem(12); // spacer

                    row.RelativeItem().Column(inner =>
                    {
                        inner.Item().Text("Kwame Nkrumah University of Science and Technology")
                             .FontSize(15).Bold().FontColor(NavyHex);

                        inner.Item().Text("Asset Management Report")
                             .FontSize(11).FontColor(MutedHex);

                        inner.Item().PaddingTop(4)
                             .Text($"Generated: {DateTime.Now:dd MMMM yyyy}")
                             .FontSize(9).FontColor(MutedHex).Italic();
                    });
                });

                // Divider
                col.Item().Height(1).Background(CreamHex);
            });
        }

        // ── Content ──────────────────────────────────────────────────
        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(20).Column(column =>
            {
                column.Spacing(24);

                // Summary stat row
                column.Item().Element(ComposeSummaryStats);

                // Assets by college table
                column.Item().Element(ComposeCollegeTableSection);

                // Damaged assets table
                column.Item().Element(ComposeDamagedAssetsSection);

                // Value breakdown
                column.Item().Element(ComposeValueBreakdown);
            });
        }

        // ── Summary stats ─────────────────────────────────────────────
        private void ComposeSummaryStats(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Element(c => StatBox(c,
                    "Total Asset Value",
                    $"GHC {_model.TotalAssetValue:N2}",
                    GoldHex));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => StatBox(c,
                    "Most Active College",
                    _model.MostActiveCollege,
                    TealHex,
                    $"{_model.MostActiveCount} assets"));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => StatBox(c,
                    "Total Damaged Assets",
                    _model.TotalDamaged.ToString(),
                    RoseHex,
                    "Across all colleges"));
            });
        }

        private void StatBox(IContainer container, string label, string value,
                             string accentColor, string? sub = null)
        {
            container
                .Border(1).BorderColor(CreamHex)
                .Background("#ffffff")
                .Padding(14)
                .Column(col =>
                {
                    col.Item().Text(label)
                       .FontSize(9).FontColor(MutedHex);

                    col.Item().PaddingTop(6).Text(value)
                       .FontSize(16).Bold().FontColor(NavyHex);

                    if (sub != null)
                        col.Item().PaddingTop(2).Text(sub)
                           .FontSize(8).FontColor(MutedHex).Italic();

                    // accent bottom bar
                    col.Item().PaddingTop(10).Height(3).Background(accentColor);
                });
        }

        // ── Section heading helper ────────────────────────────────────
        private void SectionHeading(IContainer container, string title, string subtitle)
        {
            container.Column(col =>
            {
                col.Item().Text(title)
                   .FontSize(13).Bold().FontColor(NavyHex);

                col.Item().Text(subtitle)
                   .FontSize(9).FontColor(MutedHex).Italic();

                col.Item().PaddingTop(6).Height(2).Background(CreamHex);
            });
        }

        // ── Table header cell helper ──────────────────────────────────
        private void HeaderCell(IContainer container, string text)
        {
            container
                .Background(NavyHex)
                .Padding(8)
                .Text(text)
                .FontSize(9).Bold().FontColor("#ffffff");
        }

        // ── Table data cell helper ────────────────────────────────────
        private void DataCell(IContainer container, string text,
                              bool highlight = false, string? color = null)
        {
            container
                .BorderBottom(1).BorderColor(CreamHex)
                .Background(highlight ? "#fafaf8" : "#ffffff")
                .Padding(7)
                .Text(text)
                .FontSize(9)
                .FontColor(color ?? NavyHex);
        }

        // ── Assets by College ─────────────────────────────────────────
        private void ComposeCollegeTableSection(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Element(c => SectionHeading(c,
                    "Assets by College",
                    "Distribution of assets across all colleges"));

                col.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Element(c => HeaderCell(c, "College"));
                        header.Cell().Element(c => HeaderCell(c, "Total"));
                        header.Cell().Element(c => HeaderCell(c, "Active"));
                        header.Cell().Element(c => HeaderCell(c, "Damaged"));
                    });

                    // Rows
                    bool alt = false;
                    foreach (var item in _model.AssetByCollege)
                    {
                        table.Cell().Element(c => DataCell(c, item.College, alt));
                        table.Cell().Element(c => DataCell(c, item.Total.ToString(), alt));
                        table.Cell().Element(c => DataCell(c, item.Active.ToString(), alt, TealHex));
                        table.Cell().Element(c => DataCell(c, item.Damaged.ToString(), alt, RoseHex));
                        alt = !alt;
                    }

                    // Totals footer row
                    table.Cell().Element(c =>
                        c.Background(CreamHex).Padding(7)
                         .Text("Total").Bold().FontSize(9).FontColor(NavyHex));
                    table.Cell().Element(c =>
                        c.Background(CreamHex).Padding(7)
                         .Text(_model.GrandTotal.ToString()).Bold().FontSize(9).FontColor(NavyHex));
                    table.Cell().Element(c =>
                        c.Background(CreamHex).Padding(7)
                         .Text(_model.GrandActive.ToString()).Bold().FontSize(9).FontColor(TealHex));
                    table.Cell().Element(c =>
                        c.Background(CreamHex).Padding(7)
                         .Text(_model.GrandDamaged.ToString()).Bold().FontSize(9).FontColor(RoseHex));
                });
            });
        }

        // ── Damaged Assets ────────────────────────────────────────────
        private void ComposeDamagedAssetsSection(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Element(c => SectionHeading(c,
                    "Most Damaged Assets",
                    "Assets requiring immediate attention"));

                col.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn();
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(c => HeaderCell(c, "Asset"));
                        header.Cell().Element(c => HeaderCell(c, "College"));
                        header.Cell().Element(c => HeaderCell(c, "Severity"));
                        header.Cell().Element(c => HeaderCell(c, "Reported"));
                    });

                    bool alt = false;
                    foreach (var asset in _model.DamagedAssets)
                    {
                        var severityColor = asset.Severity switch
                        {
                            "High" => RoseHex,
                            "Medium" => "#c47d1a",
                            _ => MutedHex,
                        };

                        table.Cell().Element(c => DataCell(c, asset.AssetName, alt));
                        table.Cell().Element(c => DataCell(c, asset.CollegeName, alt));
                        table.Cell().Element(c => DataCell(c, asset.Severity, alt, severityColor));
                        table.Cell().Element(c => DataCell(c,
                            asset.ReportedDate.ToString("yyyy-MM-dd"), alt));
                        alt = !alt;
                    }
                });
            });
        }

        // ── Value Breakdown ───────────────────────────────────────────
        private void ComposeValueBreakdown(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Element(c => SectionHeading(c,
                    "Asset Value by Type",
                    "Breakdown of total asset value by category"));

                col.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Element(c => ValueCard(c,
                        "Unique Assets",
                        $"GHC {_model.ValueBreakdown.UniqueValue:N2}",
                        $"{_model.ValueBreakdown.UniquePercent}% of total",
                        NavyHex));

                    row.ConstantItem(10);

                    row.RelativeItem().Element(c => ValueCard(c,
                        "Bulk Assets",
                        $"GHC {_model.ValueBreakdown.BulkValue:N2}",
                        $"{_model.ValueBreakdown.BulkPercent}% of total",
                        GoldHex));

                    row.ConstantItem(10);

                    row.RelativeItem().Element(c => ValueCard(c,
                        "Total Portfolio",
                        $"GHC {_model.ValueBreakdown.Total:N2}",
                        "Combined value",
                        TealHex));
                });
            });
        }

        private void ValueCard(IContainer container, string label,
                               string amount, string pct, string accentColor)
        {
            container
                .Border(1).BorderColor(CreamHex)
                .Padding(14)
                .Column(col =>
                {
                    col.Item().Text(label)
                       .FontSize(9).FontColor(MutedHex);

                    col.Item().PaddingTop(6).Text(amount)
                       .FontSize(13).Bold().FontColor(NavyHex);

                    col.Item().PaddingTop(2).Text(pct)
                       .FontSize(8).FontColor(MutedHex).Italic();

                    col.Item().PaddingTop(8).Height(3).Background(accentColor);
                });
        }

        // ── Footer ───────────────────────────────────────────────────
        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Height(1).Background(CreamHex);

                col.Item().PaddingTop(8).Row(row =>
                {
                    row.RelativeItem()
                       .Text("University Asset Management System — Confidential")
                       .FontSize(8).FontColor(MutedHex).Italic();

                    row.ConstantItem(100).AlignRight()
                       .Text(x =>
                       {
                           x.Span("Page ").FontSize(8).FontColor(MutedHex);
                           x.CurrentPageNumber().FontSize(8).FontColor(NavyHex).Bold();
                           x.Span(" of ").FontSize(8).FontColor(MutedHex);
                           x.TotalPages().FontSize(8).FontColor(NavyHex).Bold();
                       });
                });
            });
        }
    }
}
