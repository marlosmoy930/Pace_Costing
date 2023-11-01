using System;
using System.Collections.Generic;
using CscGet.CommandDelivery.Extensions;
using CscGet.MessageContracts.Api.Enums;
using CscGet.MessageContracts.Api.Models;
using CscGet.MessageContracts.ReportingService.Commands;
using CscGet.MessageContracts.ReportingService.Models;
using Xunit;
using FluentAssertions;

namespace CscGet.CommandDelivery.Tests.Extensions
{
    public class ExportReportCommandExtensionsTests
    {
        [Theory]
        [MemberData(nameof(GetReportExportMessageModelTestData))]
        public void GetReportExportMessageModelTest(ExportReportCommand command, ReportExportMessageModel expectedResult)
        {
            var result = command.GetReportExportMessageModel(Guid.Empty);
            result.Should().BeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> GetReportExportMessageModelTestData()
        {
            yield return new object[]
            {
                new ExportReportCommand
                {
                    Report = new ExportReportModel
                    {
                        ReportTypeId = 1,
                        CostType = ReportCostType.EBITDA
                    }
                },
                new ReportExportMessageModel
                {
                    Id = Guid.Empty,
                    ReportTypeId = 1,
                    CostType = ReportCostType.EBITDA
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetReportColumnMessageModelsTestData))]
        public void GetReportColumnMessageModelsTest(ExportReportCommand command, ReportColumnMessageModel[] expectedResult)
        {
            var result = command.GetReportColumnMessageModels();
            result.Should().BeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> GetReportColumnMessageModelsTestData()
        {
            yield return new object[]
            {
                new ExportReportCommand
                {
                    Columns = new []
                    {
                        new ExportReportColumnDataModel
                        {
                            Level = ExportReportColumnLevel.Year,
                            Range = new []
                            {
                                new ExportReportColumnMonthModel
                                {
                                    Month = 11,
                                    Year = 2017
                                },
                                new ExportReportColumnMonthModel
                                {
                                    Month = 12,
                                    Year = 2017
                                },
                                new ExportReportColumnMonthModel
                                {
                                    Month = 1,
                                    Year = 2018
                                }
                            }
                        }
                    }
                },
                new []
                {
                    new ReportColumnMessageModel
                    {
                        Level = ReportColumnMessageLevel.Year,
                        Range = new[]
                        {
                            new ReportMonthMessageModel
                            {
                                Month = 11,
                                Year = 2017
                            },
                            new ReportMonthMessageModel
                            {
                                Month = 12,
                                Year = 2017
                            },
                            new ReportMonthMessageModel
                            {
                                Month = 1,
                                Year = 2018
                            }
                        }
                    }
                }
            };
        }
    }
}
