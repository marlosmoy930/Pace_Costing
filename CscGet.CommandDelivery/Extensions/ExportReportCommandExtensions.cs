using System;
using System.Linq;
using CscGet.MessageContracts.Api.Enums;
using CscGet.MessageContracts.Api.Models;
using CscGet.MessageContracts.Api.Models.ReportSettings;
using CscGet.MessageContracts.ReportingService.Commands;
using ReportSettings = CscGet.MessageContracts.ReportingService.Models.ReportSettings;

namespace CscGet.CommandDelivery.Extensions
{
    public static class ExportReportCommandExtensions
    {
        public static ReportExportMessageModel GetReportExportMessageModel(this ExportReportCommand model, Guid reportId)
        {
            return new ReportExportMessageModel
            {
                Id = reportId,
                ReportTypeId = model.Report.ReportTypeId,
                CostType = model.Report.CostType
            };
        }

        public static ReportColumnMessageModel[] GetReportColumnMessageModels(this ExportReportCommand model)
        {
            return model.Columns?
            .Select(x => new ReportColumnMessageModel
            {
                Level = (ReportColumnMessageLevel)x.Level,
                Range = x.Range
                    .Select(m => new ReportMonthMessageModel
                    {
                        Year = m.Year,
                        Month = m.Month
                    })
                    .ToArray()
            })
            .ToArray();
        }

        public static ReportSettingsMessageModel GetReportSettingsMessageModel(this ExportReportCommand model)
        {
            return model.Settings == null
                ? null
                : new ReportSettingsMessageModel
                {
                    UnitCost = model.Settings.UnitCost == null
                        ? null
                        : new UnitCostSettingsMessageModel
                        {
                            AverageMonthlyCost = model.Settings.UnitCost.AverageMonthlyCost,
                            AverageMonthlyUnits = model.Settings.UnitCost.AverageMonthlyUnits,
                            AverageUnitCost = model.Settings.UnitCost.AverageUnitCost,
                            AverageUnitsHeadcount = model.Settings.UnitCost.AverageUnitsHeadcount,
                            AverageHeadcount = model.Settings.UnitCost.AverageHeadcount
                        },
                    Inflation = model.Settings.Inflation == null
                        ? null
                        : new InflationSettingsMessageModel
                        {
                            AnnualRate = model.Settings.Inflation.AnnualRate,
                            CompoundRate = model.Settings.Inflation.CompoundRate,
                            Cost = model.Settings.Inflation.Cost
                        },
                    PriceEditor = model.Settings.PriceEditor == null
                        ? null
                        : new PriceEditorSettingsMessageModel
                        {
                            InitialPrice = model.Settings.PriceEditor.InitialPrice,
                            Discount = model.Settings.PriceEditor.Discount,
                            Price = model.Settings.PriceEditor.Price,
                            DirectCost = model.Settings.PriceEditor.DirectCost,
                            Aop = model.Settings.PriceEditor.Aop,
                            IndirectCost = model.Settings.PriceEditor.IndirectCost,
                            Rop = model.Settings.PriceEditor.Rop,
                            Trend = model.Settings.PriceEditor.Trend
                        },
                    UnitPriceEditor = model.Settings.UnitPriceEditor == null
                        ? null
                        : new UnitPriceEditorSettingsMessageModel
                        {
                            UnitInitialPrice = model.Settings.UnitPriceEditor.UnitInitialPrice,
                            UnitDiscount = model.Settings.UnitPriceEditor.UnitDiscount,
                            UnitPrice = model.Settings.UnitPriceEditor.UnitPrice,
                            UnitDirectCost = model.Settings.UnitPriceEditor.UnitDirectCost,
                            Aop = model.Settings.UnitPriceEditor.Aop,
                            UnitIndirectCost = model.Settings.UnitPriceEditor.UnitIndirectCost,
                            Rop = model.Settings.UnitPriceEditor.Rop,
                            Trend = model.Settings.UnitPriceEditor.Trend,
                            Units = model.Settings.UnitPriceEditor.Units
                        },
                    ListPriceEditor = model.Settings.ListPriceEditor == null
                        ? null
                        : new ListPriceEditorSettingsMessageModel
                        {
                            InitialPrice = model.Settings.ListPriceEditor.InitialPrice,
                            Discount = model.Settings.ListPriceEditor.Discount,
                            Price = model.Settings.ListPriceEditor.Price,
                            DirectCost = model.Settings.ListPriceEditor.DirectCost,
                            Aop = model.Settings.ListPriceEditor.Aop,
                            IndirectCost = model.Settings.ListPriceEditor.IndirectCost,
                            Rop = model.Settings.ListPriceEditor.Rop,
                            Trend = model.Settings.ListPriceEditor.Trend,
                            Quantity = model.Settings.ListPriceEditor.Quantity
                        },
                    DashboardSettings = Map(model.Settings.DashboardSettings)
                };
        }

        private static DashboardSettingsModel Map(ReportSettings.DashboardSettingsModel model)
        {
            if (model == null)
                return null;

            return new DashboardSettingsModel()
            {
                Hierarchy = model.Hierarchy,
                YearType = model.YearType,
                Month = model.Month,
                Quarter = model.Quarter,
                YearsAtStart = model.YearsAtStart,
                CostType = model.CostType,
                CostUnitsType = model.CostUnitsType,
                Decimals = model.Decimals,
                FiscalYearStartMonth = model.FiscalYearStartMonth,
                Currency = model.Currency,
                CurrencyRateType = model.CurrencyRateType,
                WithInflation = model.WithInflation,
                WithContingencyRisk = model.WithContingencyRisk,
                WithCurrencyRisk = model.WithCurrencyRisk,
                DisplayOutOfScope = model.DisplayOutOfScope,
                DisplayZeroLines = model.DisplayZeroLines,
                DisplayAllocations = model.DisplayAllocations,
                IncludeAdjustments = model.IncludeAdjustments
            };
        }
    }
}