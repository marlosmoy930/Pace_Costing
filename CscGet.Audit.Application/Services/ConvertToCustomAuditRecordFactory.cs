using System;
using CscGet.Audit.Domain.Models;
using CscGet.Audit.Domain.Models.Enums;
using Dxc.Captn.Costing.Contracts.Operations.ConvertToCustom;

namespace CscGet.Audit.Application.Services
{
    public class ConvertToCustomAuditRecordFactory
    {
        public static ConvertToCustomAuditRecord CreateRecord(ConvertedNodes convertedItem, GroupType groupType,
            int costingVersionId, DateTime timestampUtc, Guid userId, string userName)
        {
            string templateValue, auditExceptionReasons;
            var dependencyTemplateValue = $"{convertedItem.CausedConversionNodeName} was deleted";
            switch (convertedItem.Reason)
            {
                case ConversionReason.ConvertedToCustomByUser:
                {
                    templateValue = convertedItem.CausedConversionNodeName;
                    auditExceptionReasons = AuditExceptionReasons.ConvertedToCustomByUser;

                    break;
                }
                case ConversionReason.CrossDependencyRuleBroken:
                {
                    templateValue = dependencyTemplateValue;
                    auditExceptionReasons = AuditExceptionReasons.CrossDependencyBroken;

                    break;
                }
                case ConversionReason.ChildDependencyRuleBroken:
                {
                    templateValue = dependencyTemplateValue;
                    auditExceptionReasons = AuditExceptionReasons.ChildDependencyBroken;

                    break;
                }
                case ConversionReason.CrossDependencyRuleBrokenAndChildDependencyRuleBroken:
                {
                    templateValue = dependencyTemplateValue;
                    auditExceptionReasons = AuditExceptionReasons.CrossDependencyRuleBrokenAndChildDependencyRuleBroken;

                    break;
                }
                case ConversionReason.ConvertedToCustomWhileShaping:
                {
                    templateValue = convertedItem.CausedConversionNodeName;
                    auditExceptionReasons = AuditExceptionReasons.ConvertedToCustomWhileShaping;

                    break;
                }
                case ConversionReason.ConvertedToCustomWhileMoving:
                {
                    templateValue = convertedItem.CausedConversionNodeName;
                    auditExceptionReasons = AuditExceptionReasons.ConvertedToCustomWhileCopyingOrMoving;

                    break;
                }
                case ConversionReason.ConvertedToCustomWhileCopying:
                {
                    templateValue = convertedItem.CausedConversionNodeName;
                    auditExceptionReasons = AuditExceptionReasons.ConvertedToCustomWhileCopyingOrMoving;

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new ConvertToCustomAuditRecord(convertedItem.NodeId, groupType,
                costingVersionId, timestampUtc, userId, userName,
                templateValue, auditExceptionReasons);
        }
    }
}
